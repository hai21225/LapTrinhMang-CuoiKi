
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

using System.Text.Json;
using Client.Models;

namespace Client.Network
{
    public class ConnectToServer
    {
        //private Image _playerImage = Properties.Resources.right;

        private readonly  string _ipAddress;
        private  readonly int _port;

        private Socket? _client;
        private CancellationTokenSource? _cts;
        private StringBuilder _recvBuffer = new StringBuilder();
        private readonly ConcurrentQueue<string> _messageQueue = new();


        public event Action<string>? OnInitReceived; // event nhan tin tu server, sau do gui len winform dder xuw li
        public event Action<List<Player>>? OnPlayersReceived;
        public event Action<List<Bullet>>? OnBulletReceived;
        public event Action<List<Rank>>? OnRankReceived;
        public event Action<ChatClient>? OnChatClientReceived;

        public bool IsConnected => _client != null && _client.Connected;

        public ConnectToServer(string ipAddress, int port)
        {
            _ipAddress=ipAddress;
            _port=port;
        }

        public void ConnectServer()
        {
            IPEndPoint clientIp = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _client.Connect(clientIp);
            Console.WriteLine("ket noi toi server");

            _cts = new CancellationTokenSource();
            Task.Run(() => ListenServer(_cts.Token)); // tao luong rieng bang task+ canceltoken
            Task.Run(() => ProcessMessage(_cts.Token));
        }

        public async Task ListenServer(CancellationToken token)
        {
            try
            {
                byte[] buffer = new byte[1024];

                while (!token.IsCancellationRequested && _client != null && _client.Connected)
                {
                    int received = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    if (received == 0)
                    {
                        Console.WriteLine("server dong ket noi.");
                        break;
                    }

                    string chunk = Encoding.UTF8.GetString(buffer, 0, received);
                    _recvBuffer.Append(chunk);

                    while (true)
                    {
                        string current = _recvBuffer.ToString();
                        int idx = current.IndexOf('\n'); // moi message ket thuc bang \n
                        if (idx < 0) break;

                        string line = current[..idx].Trim();
                        //Console.WriteLine("checkkkkkkkkkk"+line);
                        _recvBuffer.Remove(0, idx + 1);

                        if (string.IsNullOrWhiteSpace(line)) continue;
                        _messageQueue.Enqueue(line);
                        //try
                        //{
                        //    using var doc = JsonDocument.Parse(line);
                        //    if (!doc.RootElement.TryGetProperty("Action", out var actionProp))
                        //        continue;

                        //    string action = actionProp.GetString() ?? "";
                        //    var message = JsonSerializer.Deserialize<ServerMessage>(line);
                        //    switch (action)
                        //    {
                        //        case "INIT":
                        //            string playerId = doc.RootElement.GetProperty("PlayerId").GetString() ?? "";
                        //            OnInitReceived?.Invoke(playerId);
                        //            break;

                        //        case "PLAYERS":
                        //            // var message = JsonSerializer.Deserialize<ServerMessage>(line);
                        //            if (message?.Players != null)
                        //                OnPlayersReceived?.Invoke(message.Players);
                        //            break;
                        //        case "BULLETS":
                        //            //var msgBullet= JsonSerializer.Deserialize<ServerMessage>(line);
                        //            if (message?.Bullets != null)
                        //            {
                        //                //Console.WriteLine(message.Bullets.ToList());
                        //                OnBulletReceived?.Invoke(message.Bullets);
                        //            }
                        //            break;

                        //        case "RANK":
                        //            if(message?.Top3 != null)
                        //            {
                        //                OnRankReceived?.Invoke(message.Top3);
                        //            }
                        //            break;

                        //        case "CHAT":
                        //            string name = doc.RootElement.GetProperty("Name").GetString() ?? "";
                        //            string messagechat = doc.RootElement.GetProperty("Message").GetString() ?? "";
                        //            var chat = new ChatClient
                        //            {
                        //                Name = name,
                        //                Message = messagechat,
                        //            };
                        //            OnChatClientReceived?.Invoke(chat);
                        //            break;
                        //        default:
                        //            Console.WriteLine("Unknown action: " + action);
                        //            break;
                        //    }
                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine("Parse error: " + ex.Message);
                        //}
                    }
                }
            }
            catch (SocketException sex)
            {
                Console.WriteLine($"loi socket: {sex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"mat ket noi server: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("ngat ket noi voi server.");
                _client?.Close();
            }
        }

        private async Task ProcessMessage(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_messageQueue.TryDequeue(out var line))
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(line);
                        if (!doc.RootElement.TryGetProperty("Action", out var actionProp))
                            continue;

                        string action = actionProp.GetString() ?? "";
                        var message = JsonSerializer.Deserialize<ServerMessage>(line);
                        switch (action)
                        {
                            case "INIT":
                                string playerId = doc.RootElement.GetProperty("PlayerId").GetString() ?? "";
                                OnInitReceived?.Invoke(playerId);
                                break;

                            case "PLAYERS":
                                if (message?.Players != null)
                                    OnPlayersReceived?.Invoke(message.Players);
                                break;
                            case "BULLETS":
                      
                                if (message?.Bullets != null)
                                {                       
                                    OnBulletReceived?.Invoke(message.Bullets);
                                }
                                break;

                            case "RANK":
                                if (message?.Top3 != null)
                                {
                                    OnRankReceived?.Invoke(message.Top3);
                                }
                                break;

                            case "CHAT":
                                string name = doc.RootElement.GetProperty("Name").GetString() ?? "";
                                string messagechat = doc.RootElement.GetProperty("Message").GetString() ?? "";
                                var chat = new ChatClient
                                {
                                    Name = name,
                                    Message = messagechat,
                                };
                                OnChatClientReceived?.Invoke(chat);
                                break;
                            default:
                                Console.WriteLine("Unknown action: " + action);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Parse error: " + ex.Message);
                    }
                }
                else
                {
                    await Task.Delay(1, token); // tránh busy loop
                }
            }
        }


    public void SendData(string message)
        {
            if (!IsConnected || _client == null) return;

            string framed = message.EndsWith("\n") ? message : message + "\n";

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(framed);
                _client.Send(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendData failed: " + ex.Message);
            }
        }

    public void DisconnectServer()
        {
            if (IsConnected && _client != null)
            {
                _cts?.Cancel();
                try { _client.Shutdown(SocketShutdown.Both); } catch { }
                try { _client.Close(); } catch { }
                Console.WriteLine(" ngat ket noi.");
            }
        }
    }
}