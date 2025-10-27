using Client.Models;
using Client.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Game.Renderer
{
    public class PlayerRenderer
    {
        private readonly PlayerService _client;
        private readonly Image _playerImage = Properties.Resources.right;
        private Player? _me;

        public PlayerRenderer(PlayerService client)
        {
            _client = client;
            _client.OnGetMyPlayer += me =>
            {
                _me = me;
            };

        }
        public void Render(Graphics g)
        {
            foreach (var p in _client.GetPlayers())
            {
                if (p.State == PlayerState.Dead) continue;
                float imageWidth = _playerImage.Width;
                float imageHeight = _playerImage.Height;
                float offsetX = imageWidth / 2f - 20f;
                float offsetY = imageHeight / 2f;
                float centerX = p.X + offsetX;
                float centerY = p.Y + offsetY;

                var matrix = g.Transform;

                g.TranslateTransform(centerX, centerY);
                g.RotateTransform(p.Rotation);
                g.TranslateTransform(-centerX, -centerY);

                // sprite player
                g.DrawImage(_playerImage, p.X, p.Y, imageWidth, imageHeight);

                // Vẽ viền hình chữ nhật quanh player (debug)
                g.DrawRectangle(Pens.Red, p.X, p.Y, imageWidth, imageHeight);
                // Vẽ tâm xoay
                g.FillEllipse(Brushes.Blue, centerX - 2, centerY - 2, 4, 4);

                g.Transform = matrix;
                // Vẽ HP
                Brush hpBrush = p.Id == _me?.Id ? Brushes.LimeGreen : Brushes.Red;

                // Tính kích thước thanh máu
                float maxHpWidth = _playerImage.Height;
                float hpHeight = 6f;
                float hpPercent = Math.Max(0, Math.Min(1, p.Hp / 100f)); // Giới hạn từ 0–1

                float hpWidth = maxHpWidth * hpPercent;

                // Vị trí thanh máu (trên đầu player)
                float hpX = p.X;
                float hpY = p.Y - hpHeight - 10; // cách đầu player 10px

                // Vẽ nền thanh máu (xám)
                g.FillRectangle(Brushes.Gray, hpX, hpY, maxHpWidth, hpHeight);
                // Vẽ phần máu
                g.FillRectangle(hpBrush, hpX, hpY, hpWidth, hpHeight);

                // (Tùy chọn) Vẽ khung viền cho thanh máu
                g.DrawRectangle(Pens.Black, hpX, hpY, maxHpWidth, hpHeight);
            }
        }

    }
}
