using Client.Models;
using Client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.EventHandler
{
    public class BulletHandler
    {
        private readonly ConnectToServer _connectToServer;
        private List<Bullet> _bulletList = new();
        public event Action<List<Bullet>>? OnBulletUpdated;
        public BulletHandler(ConnectToServer connectToServer)
        {
            _connectToServer = connectToServer;

            _connectToServer.OnBulletReceived += bullets =>
            {
                _bulletList = bullets;
                OnBulletUpdated?.Invoke(_bulletList);
            };
        }
        public void SendShoot(string id,float rotationShoot)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }
            var shootMsg = new
            {
                Action = "SHOOT",
                PlayerId = id,
                RotationShoot = rotationShoot
            };
            string json = JsonSerializer.Serialize(shootMsg) + "\n";
            _connectToServer.SendData(json);
        }
        public List<Bullet> GetBullets()
        {
            return _bulletList;
        }
    }
}
