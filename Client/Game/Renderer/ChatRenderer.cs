using Client.Game.Input;
using Client.Models;
using Client.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Game.Renderer
{
    public class ChatRenderer
    {
        private readonly List<ChatClient> _messages = new List<ChatClient>();
        private readonly ChatService _chatService;
        private readonly Font _font = new Font("Arial", 16, FontStyle.Bold);
        private readonly Brush _textBrush = Brushes.White;
        private readonly int _maxMessages = 5;
        private readonly double _messageLifetime = 5.0; // 5 giây tồn tại

        public ChatRenderer(ChatService chatService)
        {
            _chatService = chatService;
            _chatService.OnChatClientReceiving += AddMessage;
        }

        private void AddMessage(ChatClient chat)
        {
            _messages.Add(chat);
            if (_messages.Count > _maxMessages)
                _messages.RemoveAt(0);
        }

        public void Update()
        {
            _messages.RemoveAll(m => (DateTime.Now - m.Time).TotalSeconds > _messageLifetime);
        }

        public void Render(Graphics g)
        {
            Update();

            int startX = 20;
            int startY = 400;
            int lineHeight = 28; // tăng chút vì font to hơn

            foreach (var msg in _messages)
            {
                string text = $"{msg.Name}: {msg.Message}";

                // viền đen
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        g.DrawString(text, _font, Brushes.Black, startX + dx, startY + dy);
                    }
                }

                // chữ trắng đậm ở giữa
                g.DrawString(text, _font, Brushes.White, startX, startY);
                startY += lineHeight;
            }
        }



    }
}
