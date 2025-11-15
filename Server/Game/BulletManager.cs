using Server.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class BulletManager
    {
        private List<Bullet> _bullets = new List<Bullet>();
        private float _distanceFromCenter = 70f;
        private float _speed = 50f;
        private float _damage = 36f;
        private float _timeExist = 0.36f;
        //private List<Player> _players = new List<Player>();

        public void CreateBullet(float rotationAngle, Player player)
        {
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
                Damage = _damage,
                TimeExist = _timeExist,
                LifeTimer = Stopwatch.StartNew(),
            };
            //Console.WriteLine(bullet.RotationAngle);
            AddBullet(bullet);
        }//done

        private void AddBullet(Bullet bullet)
        {
            _bullets.Add(bullet);
        }//done

        public void RemoveBullet(Bullet bullet)
        {
            _bullets.Remove(bullet);
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
