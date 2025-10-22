using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Client.DTO;

namespace Client.Network
{
    public class ConnectToServer
    {
        private const string _ipAddress = "127.0.0.1";
        private const int _port = 9000;

        private Socket? _client;
        private CancellationTokenSource? _cts;
        private StringBuilder _recvBuffer = new StringBuilder();

        public event Action<string>? OnInitReceived; // event nhan tin tu server, sau do gui len winform dder xuw li
        public event Action<List<PlayerDTO>>? OnPlayersReceived;
        public event Action<List<Bullet>>? OnBulletReceived;

        public bool IsConnected => _client != null && _client.Connected;

        public void ConnectServer()
        {
            IPEndPoint clientIp = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _client.Connect(clientIp);
            Console.WriteLine("ket noi toi server");

            _cts = new CancellationTokenSource();
            Task.Run(() => ListenServer(_cts.Token)); // tao luong rieng bang task+ canceltoken
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
                                    // var message = JsonSerializer.Deserialize<ServerMessage>(line);
                                    if (message?.Players != null)
                                        OnPlayersReceived?.Invoke(message.Players);
                                    break;
                                case "BULLETS":
                                    //var msgBullet= JsonSerializer.Deserialize<ServerMessage>(line);
                                    if (message?.Bullets != null)
                                    {
                                        //Console.WriteLine(message.Bullets.ToList());
                                        OnBulletReceived?.Invoke(message.Bullets);
                                    }
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