using Client.Game.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Game.Renderer
{
    public class GameUI
    {
        private readonly PlayerInputHandler _playerInputHandler;
        private readonly RankRenderer _rankRenderer;
        private readonly ChatRenderer _chatRenderer;
        private readonly SkillRenderer _skillRenderer;
        public GameUI(PlayerInputHandler playerInputHandler,RankRenderer rankRenderer, SkillRenderer skillRenderer, ChatRenderer chatRenderer)
        {
            _playerInputHandler = playerInputHandler;
            _rankRenderer = rankRenderer;
            _skillRenderer = skillRenderer;
            _chatRenderer = chatRenderer;
        }

        public void Render(Graphics g)
        {
            if(_playerInputHandler.IsChatting)
            {
                DrawChatBox(g, _playerInputHandler.ChatInput);
            }
            _rankRenderer.Render(g);
            _skillRenderer.Render(g);
            _chatRenderer.Render(g);
        }

        private void DrawChatBox(Graphics g, string text)
        {
            var rect = new Rectangle(50, 500, 400, 40);
            g.FillRectangle(Brushes.Black, rect);
            g.DrawRectangle(Pens.White, rect);
            g.DrawString($"> {text}", new Font("Arial", 12), Brushes.White, rect.X + 5, rect.Y + 10);
        }
    }
}
