using Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class BulletManager
    {
        private readonly PlayerManager _playerManager;
        private List<Bullet> _bullets = new List<Bullet>();
        //private List<Player> _players = new List<Player>();

        public BulletManager(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public void CreateBullet(float rotationAngle, Player player)
        {
            //if(rotationAngle>=0)
            //Console.WriteLine(rotationAngle);
            Console.WriteLine(player.X);
            var (offsetX, offsetY) = GetGunOffset(rotationAngle, 70f); // 70f = khoảng cách từ tâm nhân vật đến nòng súng
            float centerX = player.X + 50;
            float centerY = player.Y + 35;

            float x = centerX + offsetX;
            float y = centerY + offsetY;

            var bullet = new Bullet
            {
                Id = Guid.NewGuid(),
                OwnerId = player.Id,
                X = x,
                Y = y,
                RotationAngle = rotationAngle,
                Speed = 25f,
                Damage = 10.2f
            };
            //Console.WriteLine(bullet.RotationAngle);
            AddBullet(bullet);
        }
        public void AddBullet(Bullet bullet)
        {
            _bullets.Add(bullet);
        }
        public void RemoveBullet(Bullet bullet)
        {
            _bullets.Remove(bullet);
        }
        public void UpdateBullets()
        {
            foreach(var bullet in _bullets.ToList())
            {
                bullet.BulletCalculation();

                foreach (var player in _playerManager.GetAllPlayer())
                {
                    if (player.Id == bullet.OwnerId) continue;
                    if (bullet.X>= player.X && bullet.X<= player.X+32 &&  bullet.Y>= player.Y && bullet.Y <= player.Y + 32)
                    {
                        Console.WriteLine("nguuuuuu");
                        player.Hp -= bullet.Damage;
                        RemoveBullet(bullet);
                        if (player.Hp <= 0)
                        {
                            Console.WriteLine("hetmau roi cu:" + player.Id);
                        }
                        break;
                    }
                }
                if (bullet.X < 0 || bullet.Y < 0 || bullet.X > 800 || bullet.Y > 600)
                {
                    RemoveBullet(bullet);
                }  
            }
        }
        public IEnumerable<Bullet> GetAllBullets()
        {
            return _bullets;
        }

        private (float offsetX, float offsetY) GetGunOffset(float rotation, float distanceFromCenter)
        {
            float rad = rotation * (float)Math.PI / 180f;
            float offsetX = (float)Math.Cos(rad) * distanceFromCenter;
            float offsetY = (float)Math.Sin(rad) * distanceFromCenter;
            return (offsetX, offsetY);
        }
    }
}
