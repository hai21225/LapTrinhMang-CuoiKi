using Client.DTO;
using Client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Logic
{
    public  class ClientGameService
    {

        private readonly ConnectToServer _connectToServer;
        private List<PlayerDTO> _playerList = new();
        private List<Bullet> _bulletList = new();
        private string _myId = "";

        public event Action<List<PlayerDTO>>? OnPlayerUpdated; 
        public event Action <List<Bullet>>? OnBulletUpdated;
        public ClientGameService(ConnectToServer connectToServer)
        {
            _connectToServer = connectToServer;
            _connectToServer.OnInitReceived += id =>
            {
                _myId = id;
                Console.WriteLine("my id: "+ _myId );
            };

            _connectToServer.OnPlayersReceived += players =>
            {
                _playerList = players;
                OnPlayerUpdated?.Invoke(_playerList);
            };
            _connectToServer.OnBulletReceived += bullets =>
            {
                _bulletList = bullets;
                OnBulletUpdated?.Invoke(_bulletList);
            };
        }

        public void SendMove(string direction)
        {
            if (string.IsNullOrEmpty( _myId )) { return; }
            var moveMsg = new
            {
                Action = "MOVE",
                PlayerId = _myId,
                Direction = direction
            };
            string json = JsonSerializer.Serialize( moveMsg )+"\n";
            _connectToServer.SendData( json );
        }
        public void SendRotation(float rotation)
        {
            if (string.IsNullOrEmpty( _myId )) return;
            var rotationMsg = new
            {
                Action = "ROTATION",
                PlayerId = _myId,
                Rotation = rotation
            };
            string json = JsonSerializer.Serialize(rotationMsg) + "\n";
            _connectToServer.SendData(json);
        }

        public void SendShoot(float rotationShoot)
        {
            if (string.IsNullOrEmpty( _myId ))
            {
                return;
            }
            var shootMsg = new
            {
                Action= "SHOOT",
                PlayerId = _myId,
                RotationShoot = rotationShoot
            };
            string json = JsonSerializer.Serialize(shootMsg) + "\n";
            _connectToServer.SendData(json);
        }

        public List<PlayerDTO> GetPlayers()
        {
            return _playerList;
        }
        public string GetMyId
        {
            get { return _myId; } 
        }

        public List<Bullet> GetBullets()
        {
            return _bulletList;
        }
    }
}
