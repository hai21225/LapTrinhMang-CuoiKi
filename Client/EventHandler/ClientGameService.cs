using Client.DTO;
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
        private string _myId = "";

        public event Action<List<PlayerDTO>>? OnPlayerUpdated; 
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

        public List<PlayerDTO> GetPlayers()
        {
            return _playerList;
        }
        public string GetMyId
        {
            get { return _myId; } 
        }
    }
}
