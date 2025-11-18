using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Server.Game;
using Server.GameWorld;
using System.Collections.Concurrent;
using Server.DTO;
using System.Xml.Linq;

namespace Server.Network
{
    public class ServerHost
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly List<ClientHandler> _clients = new List<ClientHandler>();
        private readonly PlayerManager _playerManager;
        private readonly BulletManager _bulletManager;
        private readonly RankManager _rankManager;
        private readonly GameLogic _game;
        private DateTime _lastUpdate = DateTime.Now;
        private readonly ConcurrentQueue<ClientAction> _actionQueue = new ConcurrentQueue<ClientAction>();
        

        public ServerHost (string ip, int port, PlayerManager playerManager, BulletManager bulletManager,GameLogic gameLogic,RankManager rankManager)
        {
            _ip = ip;
            _port = port;
            _playerManager = playerManager;
            _bulletManager = bulletManager;
            _game = gameLogic;
            _rankManager = rankManager;
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
                var handler = new ClientHandler(client, this,_playerManager);// this 
                _clients.Add(handler);
                //_ = Task.Run(() => handler.StartListeningAsync());
                _ = handler.StartListeningAsync();
            }
        }

        public async Task GameLoopAsync()
        {
            var lastRankUpdate = DateTime.Now;
            while (true)
            {
                var now = DateTime.Now;
                float deltaTime = (float)(now - _lastUpdate).TotalSeconds;
                _lastUpdate = now;

                ProcessQueue();
                _game.Update(deltaTime);
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
                if ((DateTime.Now - lastRankUpdate).TotalSeconds >= 2)
                {
                    lastRankUpdate = DateTime.Now;
                    _rankManager.UpdateRank();
                    Broadcast(new
                    {
                        Action = "RANK",
                        Top3 = _rankManager.GetTop3(),
                    });
                }
                await Task.Delay(30);
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

        public void SetActionQueue(ClientAction json)
        {
            _actionQueue.Enqueue(json);
        }


        private void ProcessQueue()
        {
            while (_actionQueue.TryDequeue(out var action))
            {
                var player = _playerManager.GetPlayer(action.PlayerId);
                switch(action.Action)
                {
                    case "PROFILE":
                        _playerManager.Profile(action.PlayerId, action.Name, action.Width, action.Height);
                        break;
                    case "MOVE":
                        if (player != null)
                            _playerManager.MovePlayer(action.PlayerId, action.Direction);
                        break;
                    case "ROTATION":
                        if (player != null)
                            _playerManager.RotationPlayer(action.PlayerId, action.Rotation);
                        break;
                    case "SHOOT":
                        if (player != null && player.AllowShoot())
                            _bulletManager.CreateBullet(action.RotationShoot, player);
                        break;
                    case "DASH":
                        player?.StartDash();
                        break;
                    case "GUNRELOAD":
                        player?.GunReload();
                        break;
                    case "ULTIMATE":
                        player?.StartSkillUltimate();
                        break;
                    case "CHAT":
                        if (player != null)
                            Broadcast(new
                            {
                                Action = "CHAT",
                                Name = action.Name,
                                Message = action.Message
                            });
                        break;
                }
            }
        }

    }
}
