using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Camera
    {
        public float X { get;private set; }
        public float Y { get; private set; }

        private readonly int _screenWidth;
        private readonly int _screenHeight;

        public Camera(int  screenWidth, int screenHeight)
        {
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
        }

        public void Follow(Player player)
        {
            float targetX = player.X - _screenWidth / 2;
            float targetY = player.Y - _screenHeight / 2;

            X += (targetX - X);
            Y += (targetY - Y) ;
            //Console.WriteLine($"Camera: X={X}, Y={Y}, Player=({player.X},{player.Y})");
            float _worldWidth = 1280 * 3;
            float _worldHeight = 720 * 3;

            X = Math.Clamp(X, 0, _worldWidth - _screenWidth);
            Y = Math.Clamp(Y, 0, _worldHeight - _screenHeight);
        }

        public PointF WorldToScreen(float worldX, float worldY)
        {
            return new PointF((worldX - X), (worldY - Y));
        }
        public PointF ScreenToWorld(float screenX, float screenY)
        {
            return new PointF(screenX  + X, screenY  + Y);
        }

    }
}
