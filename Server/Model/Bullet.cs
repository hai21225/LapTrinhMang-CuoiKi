using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Model
{
    public class Bullet
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; } // id nguoi ban 
        public float Speed { get; set; } = 10f;
        public float RotationAngle{ get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Damage { get; set; } = 10.2f;
        public float TimeExist { get; set; } = 3.6f;
        public Stopwatch LifeTimer { get; set; } = Stopwatch.StartNew();
        public void BulletCalculation()
        {
            float rad = (float)(RotationAngle * Math.PI / 180);// tranform deg to rad
            X += Speed * (float)Math.Cos(rad); // toa do x, dung cos
            Y += Speed * (float)Math.Sin(rad);// toa do y, dung sin
            // cos nam sin dung
        }
    }
}
