using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using Server.Network;
using Server.Game;
using Server.GameWorld;

namespace Server
{
    internal class Program
    {
        //private static string _ipAdress = "26.215.255.178";
        //private static string _ipAdress = "127.0.0.1";
        private static string _ipAdress = "192.168.1.50";
        private static int _port = 9000;
        private static PlayerManager _playerManager= new PlayerManager();
        private static BulletManager _bulletManager = new BulletManager();
        private static RankManager _rankManager = new RankManager(_playerManager);
        private static GameLogic _game = new GameLogic(_playerManager, _bulletManager,_rankManager);


        static async Task Main(string[] args)
        {
            var server= new ServerHost(_ipAdress, _port,_playerManager,_bulletManager,_game,_rankManager);
            await server.StartAsync();
        }
    }
}
