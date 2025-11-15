using Client.Models;
using Client.Network;
using System.Text.Json;


namespace Client.Service
{
    public  class PlayerService
    {
        private readonly Image _playerImage = Properties.Resources.right;
        private readonly ConnectToServer _connectToServer;
        private readonly string _playerName;
        private List<Player> _playerList = new();
        private string _myId = "";
        public event Action<List<Player>>? OnPlayerUpdated; 
        public event Action<Player>? OnGetMyPlayer;

        public PlayerService(ConnectToServer connectToServer,string playerName)
        {
            _connectToServer = connectToServer;
            _playerName = playerName;
            _connectToServer.OnInitReceived += id =>
            {
                _myId = id;
                Console.WriteLine("my id: "+ _myId );
                SendProfile(_playerImage.Width, _playerImage.Height);
            };
            _connectToServer.OnPlayersReceived += players =>
            {
                _playerList = players;
                var player = _playerList.FirstOrDefault(p=>p.Id.ToString() == _myId);
                if (player != null)
                {
                    OnGetMyPlayer?.Invoke(player);
                }
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
        public void SendDash()
        {
            if (string.IsNullOrEmpty(_myId))
            {
                return;
            }
            var dashMsg = new
            {
                Action = "DASH",
                PlayerId = _myId
            };
            string json = JsonSerializer.Serialize(dashMsg) + "\n";
            _connectToServer.SendData(json);
        }
        public void SendGunReload()
        {
            if (string.IsNullOrEmpty(_myId))
            {
                return;
            }
            var gunreloadMsg = new
            {
                Action = "GUNRELOAD",
                PlayerId = _myId
            };
            string json = JsonSerializer.Serialize(gunreloadMsg) + "\n";
            _connectToServer.SendData(json);
        }
        public void SendUltimate()
        {
            if (string.IsNullOrEmpty(_myId))
            {
                return;
            }
            var ultiMsg = new
            {
                Action = "ULTIMATE",
                PlayerId = _myId
            };
            string json = JsonSerializer.Serialize(ultiMsg) + "\n";
            _connectToServer.SendData(json);
        }
        public void SendChat(string text)
        {
            if (string.IsNullOrEmpty(_myId))
            {
                return;
            }
            var chatMsg = new
            {
                Action = "CHAT",
                PlayerId= _myId,
                Message= text
            };
            string json = JsonSerializer.Serialize(chatMsg) + "\n";
            _connectToServer.SendData(json);
        }
        public List<Player> GetPlayers()
        {
            return _playerList;
        }
        public string GetMyId
        {
            get { return _myId; } 
        }

        public void SendProfile(float width,float height)
        {
            if(string.IsNullOrEmpty( _myId )) return;
            Console.WriteLine("checkkkkk"+_playerName);
            var msg = new
            {
                Action = "PROFILE",
                PlayerId= _myId,
                Name=_playerName,
                Width= width,
                Height= height
            };
            var json =JsonSerializer.Serialize(msg) + "\n";
            _connectToServer.SendData(json);
        }

    }
}
