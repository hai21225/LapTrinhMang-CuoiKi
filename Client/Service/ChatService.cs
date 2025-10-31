using Client.Models;
using Client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Service
{
    public class ChatService
    {
        private readonly ConnectToServer _client;
        private ChatClient? _chat;
        public event Action<ChatClient>? OnChatClientReceiving;
        public ChatService(ConnectToServer client)
        {
            _client = client;
            _client.OnChatClientReceived += chat =>
            {
                _chat = chat;
                OnChatClientReceiving?.Invoke(_chat);
            };
        }
    }
}
