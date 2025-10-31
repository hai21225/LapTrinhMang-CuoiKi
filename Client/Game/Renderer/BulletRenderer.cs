using Client.Models;
using Client.Service;
using System.Drawing.Drawing2D;

namespace Client.Game.Renderer
{
    public class BulletRenderer
    {
        private readonly BulletService _bullet;
        private readonly Camera _camera;
        private float _bulletSize = 10f;      // kích thước đầu đạn
        private float _trailLength = 35f;
        public BulletRenderer(BulletService bullet,Camera camera)
        {
            _bullet = bullet;
            _camera = camera;

        }
        public void Render(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            foreach (var b in _bullet.GetBullets())
            {
                var bulletScreenPos = _camera.WorldToScreen(b.X, b.Y);

                float rad = (float)(b.RotationAngle * Math.PI / 180f);

                // Tính đầu và đuôi của vệt sáng
                float trailEndX = bulletScreenPos.X - (float)Math.Cos(rad) * _trailLength;
                float trailEndY = bulletScreenPos.Y - (float)Math.Sin(rad) * _trailLength;

                // Vẽ vệt sáng (gradient vàng -> cam, trong suốt dần)
                using (var trailBrush = new LinearGradientBrush(
                    new PointF(bulletScreenPos.X, bulletScreenPos.Y),
                    new PointF(trailEndX, trailEndY),
                    Color.FromArgb(200, Color.Yellow),
                    Color.FromArgb(0, Color.Orange)))
                using (var trailPen = new Pen(trailBrush, 5))
                {
                    trailPen.StartCap = LineCap.Round;
                    trailPen.EndCap = LineCap.Round;
                    g.DrawLine(trailPen, bulletScreenPos.X, bulletScreenPos.Y, trailEndX, trailEndY);
                }
                // Vẽ đầu đạn (hình tròn vàng)
                float bulletX = bulletScreenPos.X - _bulletSize / 2;
                float bulletY = bulletScreenPos.Y - _bulletSize / 2;

                using (var brush = new SolidBrush(Color.Yellow))
                {
                    g.FillEllipse(brush, bulletX, bulletY, _bulletSize, _bulletSize);
                }
            }
        }

    }
}
