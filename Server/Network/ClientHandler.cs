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
        private readonly BulletManager _bulletManager;
        public Player Player { get; private set; }
        private Random rand = new Random();

        public ClientHandler (Socket client, ServerHost server,PlayerManager playerManager,BulletManager bulletManager)
        {
            _client = client;
            _server = server;
            _playerManager = playerManager;
            _bulletManager = bulletManager;

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
                BroadcastPlayers();

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
                BroadcastPlayers();
                _client.Close();
            }
        }

        private void ProcessMessage(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                string action = doc.RootElement.GetProperty("Action").GetString() ?? "";
                string playerId = doc.RootElement.GetProperty("PlayerId").GetString() ?? "";
                var player = _playerManager.GetPlayer(playerId);
                switch (action)
                {
                    case "PROFILE":
                        Console.WriteLine("check");
                        float width = 0;
                        float height= 0;
                        if (doc.RootElement.TryGetProperty("Width", out var wProp))
                        {
                            width = wProp.GetSingle();
                        }
                        if (doc.RootElement.TryGetProperty("Height", out var hProp))
                        {
                            height = hProp.GetSingle();
                        }
                        string name = doc.RootElement.GetProperty("Name").ToString() ?? "";
                        _playerManager.Profile(playerId,name,width,height);
                        break;

                    case "MOVE":
                        string dir = doc.RootElement.GetProperty("Direction").GetString() ?? "";
                        _playerManager.MovePlayer(playerId, dir);
                       BroadcastPlayers();
                        break;

                    case "ROTATION":
                        float rotation = 0f;
                        if (doc.RootElement.TryGetProperty("Rotation", out var rotProp))
                        {
                            rotation = rotProp.GetSingle();
                        }
                        _playerManager.RotationPlayer(playerId, rotation);
                       BroadcastPlayers();
                        break;

                    case "SHOOT":
                        float rotationShoot = 0f;
                        if (doc.RootElement.TryGetProperty("RotationShoot", out var rotShotProp))
                        {
                            rotationShoot = rotShotProp.GetSingle();
                        }
                        var playerShoot=_playerManager.GetPlayer(playerId);
                        if (playerShoot != null)
                        {
                            //Console.WriteLine(rotationShoot);
                            if (!playerShoot.AllowShoot()) return;
                            _bulletManager.CreateBullet(rotationShoot, playerShoot);
                            BroadcastBullet();// done
                        }
                        break;

                    case "DASH":
                        if (player != null)
                        {
                            player.StartDash();
                            BroadcastPlayers();
                        }
                        break;
                    case "GUNRELOAD":
                        if (player != null)
                        {
                            player.GunReload();
                        }
                        break;
                    case "ULTIMATE":
                        if (player != null)
                        {
                            player.StartSkillUltimate();
                            BroadcastPlayers();
                        }
                        break;
                    case "CHAT":
                        if(player != null)
                        {
                            string message= doc.RootElement.GetProperty("Message").GetString() ?? "";
                            BroadcastMessage(player.Name, message);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Parse error: {ex.Message}");
            }
        }

        private void BroadcastPlayers()
        {
            _server.Broadcast(new
            {
                Action = "PLAYERS",
                Players = _playerManager.GetAllPlayer()
            });
        }
        private void BroadcastMessage(string name,string msg)
        {
            _server.Broadcast(new
            {
                Action="CHAT",
                Name= name,
                Message = msg
            });
        }
        private void BroadcastBullet()
        {
            _server.Broadcast(new
            {
                Action = "BULLETS",
                Bullets = _bulletManager.GetAllBullets()
            });
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
