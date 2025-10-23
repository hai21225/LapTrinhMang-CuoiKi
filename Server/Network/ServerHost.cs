using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Server.Game;

namespace Server.Network
{
    public class ServerHost
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly List<ClientHandler> _clients = new List<ClientHandler>();
        private readonly PlayerManager _playerManager;
        private readonly BulletManager _bulletManager;

        public ServerHost (string ip, int port, PlayerManager playerManager, BulletManager bulletManager)
        {
            _ip = ip;
            _port = port;
            _playerManager = playerManager;
            _bulletManager = bulletManager;
        }

        public async Task StartAsync()
        {
            IPEndPoint endPoint = new(IPAddress.Parse(_ip), _port);
            Socket listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(endPoint);
            listener.Listen(10);

            Console.WriteLine("server dang chay tren port: " + _port);

            _ = GameLoopAsync();
            while (true)
            {
                Socket client = await listener.AcceptAsync();
                var handler = new ClientHandler(client, this,_playerManager,_bulletManager);// this 
                _clients.Add(handler);
                _ = handler.StartListeningAsync();
            }
        }

        public async Task GameLoopAsync()
        {
            while (true)
            {
                _bulletManager.UpdateBullets();
                Broadcast(new
                {
                    Action= "BULLETS",
                    Bullets= _bulletManager.GetAllBullets()
                });
                Broadcast(new
                {
                    Action = "PLAYERS",
                    Players = _playerManager.GetAllPlayer()
                });
                await Task.Delay(50);
            }
        }
        public void Broadcast(object message)
        {
            string endline = "\n";
            string json= JsonSerializer.Serialize(message)+ endline;
            byte[] data = Encoding.UTF8.GetBytes(json);
            lock (_clients.ToList())
            {
                foreach (var c in _clients.ToList())
                {
                    try
                    {
                        c.Send(data);
                    }
                    catch
                    {
                        _clients.Remove(c);
                    }
                }
            }
        }
        public void RemoveClient(ClientHandler client)
        {
            _clients.Remove(client);
        }
    }
}
