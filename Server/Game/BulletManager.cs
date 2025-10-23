using Server.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class BulletManager
    {
        private readonly PlayerManager _playerManager;
        private List<Bullet> _bullets = new List<Bullet>();
        private float _distanceFromCenter = 70f;
        private float _speed = 21.022005f;
        private float _damage = 36f;
        //private List<Player> _players = new List<Player>();

        public BulletManager(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public void CreateBullet(float rotationAngle, Player player)
        {
            //if(rotationAngle>=0)
            //Console.WriteLine(rotationAngle);
            Console.WriteLine(player.Width);
            var (offsetX, offsetY) = GetGunOffset(player.X,player.Y,player.Width,player.Height,rotationAngle, _distanceFromCenter); 

            float x = offsetX;
            float y = offsetY;

            var bullet = new Bullet
            {
                Id = Guid.NewGuid(),
                OwnerId = player.Id,
                X = x,
                Y = y,
                RotationAngle = rotationAngle,
                Speed = _speed,
                Damage = _damage
            };
            //Console.WriteLine(bullet.RotationAngle);
            AddBullet(bullet);
        }//done

        public void AddBullet(Bullet bullet)
        {
            _bullets.Add(bullet);
        }//done

        public void RemoveBullet(Bullet bullet)
        {
            _bullets.Remove(bullet);
        }//done

        public void UpdateBullets()
        {
            foreach(var bullet in _bullets.ToList())
            {
                bullet.BulletCalculation();

                foreach (var player in _playerManager.GetAllPlayer())
                {
                    if (player.Id == bullet.OwnerId) continue;
                    var corners = _playerManager.BoxCollider(player.Id.ToString(), player.Width, player.Height);
                    if (_playerManager.IsPointInPolygon4(corners, new PointF(bullet.X, bullet.Y)))
                    {    
                        player.Hp -= bullet.Damage;
                        Console.WriteLine($"Player {player.Id} bi trung dan!");
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
        }//done

        public IEnumerable<Bullet> GetAllBullets()
        {
            return _bullets;
        }//done

        private (float offsetX, float offsetY) GetGunOffset(float x,float y,float width,float height,float rotation, float distanceFromCenter)
        {
            float pivotx= x + width/2f -20f;
            float pivoty = y + height / 2f;

            //Console.WriteLine("check pivotxy: "+pivotx+"  "+pivoty);


            float rad = rotation * (float)Math.PI / 180f;

            float offsetX = pivotx +(float)Math.Cos(rad) * distanceFromCenter;
            float offsetY = pivoty+(float)Math.Sin(rad) * distanceFromCenter;

            return (offsetX, offsetY);
        }//done
    }
}
