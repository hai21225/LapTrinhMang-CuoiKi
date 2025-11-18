using Server.DTO;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server.Network
{
    public class ClientHandler
    {
        private readonly Socket _client;
        private readonly ServerHost _server;
        private readonly StringBuilder _recvBuffer = new StringBuilder();
        private readonly PlayerManager _playerManager;
        public Player Player { get; private set; }
        private Random rand = new Random();

        public ClientHandler (Socket client, ServerHost server,PlayerManager playerManager)
        {
            _client = client;
            _server = server;
            _playerManager = playerManager;

            Player = new Player
            {
                Id = Guid.NewGuid(),
                X = rand.Next(0, 500),
                Y = rand.Next(0, 500),
                Hp = 100f,
                Rotation = rand.Next(-180, 180),
                State = PlayerState.Alive,
                RespawnTime = 5f
            };
        }

        public async Task StartListeningAsync()
        {
            try
            {
                // init player and send it to player 
                await SendJsonAsync(new 
                { 
                    Action = "INIT", 
                    PlayerId = Player.Id
                });
                _playerManager.AddPlayer(Player);

                byte[] buffer = new byte[1024];
                while (_client.Connected)
                {
                    int received = await _client.ReceiveAsync(buffer, SocketFlags.None);
                    if (received == 0) break;

                    string chunk = Encoding.UTF8.GetString(buffer, 0, received);
                    _recvBuffer.Append(chunk);

                    while (true)
                    {
                        string current = _recvBuffer.ToString();
                        int idx = current.IndexOf('\n');
                        if (idx < 0) break;

                        string line = current[..idx].Trim();
                        _recvBuffer.Remove(0, idx + 1);
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        ProcessMessage(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"client error: {ex.Message}");

            }
            finally
            {
                _server.RemoveClient(this);
                _playerManager.RemovePlayer(Player.Id.ToString());
                Console.WriteLine("client thoat");
                _client.Close();
            }
        }

        private void ProcessMessage(string json)
        {
            try
            {
                var action = JsonSerializer.Deserialize<ClientAction>(json);
                if (action!= null)
                {
                    _server.SetActionQueue(action);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Parse error: {ex.Message}");
            }
        }
        public void Send(byte[] data)
        {
            _client.Send(data);
        }

        private async Task SendJsonAsync(object msg)
        {
            string json = JsonSerializer.Serialize(msg) + "\n";
            byte[] data = Encoding.UTF8.GetBytes(json);
            await _client.SendAsync(data, SocketFlags.None);
        }

    }
}
