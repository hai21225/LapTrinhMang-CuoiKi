using Client.Models;
using Client.Service;
using System.Drawing.Drawing2D;


namespace Client.Game.Renderer
{
    public class PlayerRenderer
    {
        private readonly PlayerService _client;
        private readonly Camera _camera;
        private readonly Image _playerImage = Properties.Resources.right;
        private readonly Image _playerDead = Properties.Resources.dead;
        private Player? _me;

        public PlayerRenderer(PlayerService client, Camera camera)
        {
            _client = client;
            _client.OnGetMyPlayer += me =>
            {
                _me = me;
            };
            _camera = camera;
        }
        public void Render(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (_me!=null)
            {
                _camera.Follow(_me);
            }
            foreach (var p in _client.GetPlayers())
            {

                if (p.State == PlayerState.Dead) continue;

                var pos= _camera.WorldToScreen(p.X, p.Y);

                float imageWidth = _playerImage.Width;
                float imageHeight = _playerImage.Height;
                float offsetX = imageWidth / 2f - 20f;
                float offsetY = imageHeight / 2f;
                float centerX = pos.X + offsetX;
                float centerY = pos.Y + offsetY;

                var matrix = g.Transform;

                g.TranslateTransform(centerX, centerY);
                g.RotateTransform(p.Rotation);
                g.TranslateTransform(-centerX, -centerY);

                // sprite player
                g.DrawImage(_playerImage, pos.X, pos.Y, imageWidth, imageHeight);

                // Vẽ viền hình chữ nhật quanh player (debug)
                //g.DrawRectangle(Pens.Red, pos.X, pos.Y, imageWidth, imageHeight);
                // Vẽ tâm xoay
                //g.FillEllipse(Brushes.Blue, centerX - 2, centerY - 2, 4, 4);

                g.Transform = matrix;
                // Vẽ HP
                Brush hpBrush = p.Id == _me?.Id ? Brushes.LimeGreen : Brushes.Red;

                // Tính kích thước thanh máu
                float maxHpWidth = _playerImage.Height;
                float hpHeight = 6f;
                float hpPercent = Math.Max(0, Math.Min(1, p.Hp / 100f)); // Giới hạn từ 0–1

                float hpWidth = maxHpWidth * hpPercent;

                // Vị trí thanh máu (trên đầu player)
                float hpX = pos.X;
                float hpY = pos.Y - hpHeight - 10; // cách đầu player 10px

                // Vẽ nền thanh máu (xám)
                g.FillRectangle(Brushes.Gray, hpX, hpY, maxHpWidth, hpHeight);
                // Vẽ phần máu
                g.FillRectangle(hpBrush, hpX, hpY, hpWidth, hpHeight);

                // (Tùy chọn) Vẽ khung viền cho thanh máu
                g.DrawRectangle(Pens.Black, hpX, hpY, maxHpWidth, hpHeight);

                string playerName = p.Name ?? "Unknown";
                using (Font font = new Font("Segoe UI", 10, FontStyle.Bold))
                {
                    SizeF textSize = g.MeasureString(playerName, font);
                    float nameX = hpX + (maxHpWidth - textSize.Width) / 2;
                    float nameY = hpY - textSize.Height - 2; // Cách thanh máu 2px

                    // Viền chữ (hiệu ứng outline)
                    using (var outlineBrush = new SolidBrush(Color.Black))
                    {
                        // 4 hướng viền
                        g.DrawString(playerName, font, outlineBrush, nameX - 1, nameY);
                        g.DrawString(playerName, font, outlineBrush, nameX + 1, nameY);
                        g.DrawString(playerName, font, outlineBrush, nameX, nameY - 1);
                        g.DrawString(playerName, font, outlineBrush, nameX, nameY + 1);
                    }

                    // Chữ chính
                    using (var textBrush = new SolidBrush(Color.White))
                    {
                        g.DrawString(playerName, font, textBrush, nameX, nameY);
                    }
                }
            }
        }

        public void RenderPlayerDead (Graphics g)
        {
            if (_me != null)
            {
                _camera.Follow(_me);
            }
            foreach (var p in _client.GetPlayers())
            {
                if (p.State == PlayerState.Alive) continue;
                var pos = _camera.WorldToScreen(p.X, p.Y);
                g.DrawImage(_playerDead, pos.X, pos.Y, _playerDead.Width, _playerDead.Height);
            }
        }
    }
}
