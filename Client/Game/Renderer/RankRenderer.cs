using Client.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Game.Renderer
{
    public class RankRenderer
    {
        private readonly RankService _rankService;
        private readonly Font _fontTitle = new Font("Arial", 16, FontStyle.Bold);
        private readonly Font _fontEntry = new Font("Arial", 12, FontStyle.Bold);
        private readonly Brush _textBrush = Brushes.White;
        private readonly Brush _bgBrush = new SolidBrush(Color.FromArgb(120, Color.Black));

        private readonly int _x = 900; // góc phải
        private readonly int _y = 50;
        private readonly int _width = 250;
        private readonly int _lineHeight = 30;

        public RankRenderer(RankService rankService)
        {
            _rankService = rankService;
        }

        public void Render(Graphics g)
        {
            var _top3 = _rankService.GetRanks();
            // nền bảng
            g.FillRectangle(_bgBrush, _x, _y, _width, _lineHeight * 5);
            g.DrawRectangle(Pens.White, _x, _y, _width, _lineHeight * 5);

            // tiêu đề
            g.DrawString("Rank: ", _fontTitle, Brushes.Gold, _x + 10, _y + 5);

            // hiển thị top
            for (int i = 0; i < _top3?.Count; i++)
            {
                var entry = _top3[i];
                string text = $"{i + 1}. {entry.Name} - {entry.Score}";
                Brush color = i == 0 ? Brushes.Gold : (i == 1 ? Brushes.Silver : Brushes.OrangeRed);
                g.DrawString(text, _fontEntry, color, _x + 10, _y + 40 + i * _lineHeight);
            }

            // nếu ít hơn 3 người thì ghi trống
            if (_top3?.Count == 0)
            {
                g.DrawString("Chưa có người chơi...", _fontEntry, Brushes.Gray, _x + 10, _y + 40);
            }
        }
    }

}
