using Client.Models;
using Client.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Game.Renderer
{
    public class SkillRenderer
    {
        private readonly PlayerService _player;
        private Player? _me;
        private readonly Font _font = new Font("Arial", 10, FontStyle.Bold);
        private readonly Brush _textBrush = Brushes.White;
        private readonly Brush _cooldownBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
        private int _baseX = 80;
        private int _baseY = 580;


        public SkillRenderer(PlayerService client)
        {
            _player = client;
            _player.OnGetMyPlayer += me =>
            {
                _me = me;
            };
        }

        public void Render(Graphics g)
        {
            if (_me == null) return;
            DrawSkillBox(g,"E",_me.DashCooldownLeft,_me.DashCooldown,_baseX*6,_baseY);
            DrawSkillBox(g,"Space",_me.UltimateCooldownLeft,_me.UltimateCooldown,_baseX*8,_baseY);
            DrawAmmo(g,_me.Ammo, _baseX*4,_baseY);    
        }

        private void DrawSkillBox(Graphics g, string skillName, float cooldownLeft, float totalCooldown, int x, int y)
        {
            int size = 120;
            Rectangle box = new Rectangle(x, y, size, size);

            // nền ô kỹ năng
            g.FillRectangle(Brushes.DimGray, box);
            g.DrawRectangle(Pens.White, box);

            // nếu đang cooldown → phủ lớp mờ
            if (cooldownLeft > 0)
            {
                float percent = cooldownLeft / totalCooldown;
                int height = (int)(size * percent);
                Rectangle overlay = new Rectangle(x, y + (size - height), size, height);
                g.FillRectangle(_cooldownBrush, overlay);

                // vẽ số giây còn lại
                string text = $"{cooldownLeft:0.0}";
                var textSize = g.MeasureString(text, _font);
                g.DrawString(text, _font, _textBrush, x + (size - textSize.Width) / 2, y + (size - textSize.Height) / 2);
            }
            else
            {
                // khi sẵn sàng → vẽ border sáng
                g.DrawRectangle(new Pen(Color.LimeGreen, 2), box);
            }

            // vẽ tên skill dưới ô
            var nameSize = g.MeasureString(skillName, _font);
            g.DrawString(skillName, _font, Brushes.White, x + (size - nameSize.Width) / 2, y + size + 5);
        }

        private void DrawAmmo(Graphics g, int ammo,int x, int y)
        {
            int maxAmmo = 6; 
            int bulletWidth = 10;
            int spacing = 3;

            for (int i = 0; i < maxAmmo; i++)
            {
                Brush b = i < ammo ? Brushes.Yellow : Brushes.Gray;
                g.FillRectangle(b, x + i * (bulletWidth + spacing), y, bulletWidth, 20);
                g.DrawRectangle(Pens.Black, x + i * (bulletWidth + spacing), y, bulletWidth, 20);
            }

            g.DrawString("Q", new Font("Arial", 10, FontStyle.Bold), Brushes.White, x, y + 25);
        }
    }
}
