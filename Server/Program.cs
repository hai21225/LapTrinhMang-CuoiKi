using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using Server.Network;
using Server.Game;

namespace Server
{
    internal class Program
    {
        private static string _ipAdress = "127.0.0.1";
        private static int _port = 9000;

        private static List<Socket> _clientList = new List<Socket>(); // done

        private static Dictionary<Socket, Guid> _clientToPlayer = new Dictionary<Socket, Guid>(); // done

        private static List<Player> _players = new List<Player>(); // done

        private static PlayerManager _playerManager= new PlayerManager();


        static async Task Main(string[] args)
        {
            //IPEndPoint serverIp = new IPEndPoint(IPAddress.Parse(_ipAdress), _port);// khai bao 1 ip adress va 1 port
            //Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //socket tcp

            //server.Bind(serverIp);

            //server.Listen(10);
            //Console.WriteLine("server da chay tai port " + _port + "...");

            var server= new ServerHost(_ipAdress, _port,_playerManager);
            await server.StartAsync();
            //while (true)
            //{
            //    Socket client = await server.AcceptAsync();
            //    lock (_clientList)
            //    {
            //        _clientList.Add(client);
            //    }

            //    Console.WriteLine($"Client {client.RemoteEndPoint} da ket noi!");
            //    _ =Task.Run(() =>  ClientHandle(client));
            //}
        }

        //private static async Task ClientHandle(Socket client)
        //{
        //    try
        //    {
        //        var newPlayer = CreateNewPlayer();
        //        _clientToPlayer[client] = newPlayer.Id;// match client socket voi id

        //        var initMsg = new
        //        {
        //            Action = "INIT",
        //            PlayerId = newPlayer.Id
        //        };
        //        string json = JsonSerializer.Serialize(initMsg) + "\n";
        //        client.Send(Encoding.UTF8.GetBytes(json));

        //        BroadcastMessage();
        //        while (true)
        //        {
        //             await ListenClient(client);
        //        }
        //    }
        //    catch
        //    {
        //        Console.WriteLine($"{client.RemoteEndPoint} da roi khoi phong");
        //    }
        //    finally
        //    {
        //        DisconnectClient(client);
        //    }
        //}
        //private static void BroadcastMessage()
        //{
        //    var msg = new
        //    {
        //        Action = "PLAYERS",
        //        Players = _players
        //    };

        //    var jsonPlayerList = JsonSerializer.Serialize(msg) + "\n";
        //    //Console.WriteLine(jsonPlayerList);
        //    byte[] data = Encoding.UTF8.GetBytes(jsonPlayerList );// ma hoa thanh byte 
        //    lock (_clientList)
        //    {
        //        foreach (var client in _clientList)
        //        {
        //            try
        //            {
        //                client.Send(data);
        //            }
        //            catch
        //            {
        //                DisconnectClient(client);
        //            }
        //        }
        //    }
        //}
        //private static async Task ListenClient(Socket client)
        //{
        //    byte[] buffer = new byte[1024];
        //    StringBuilder recvBuffer = new StringBuilder();

        //    try
        //    {
        //        while (client.Connected)
        //        {
        //            int received = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
        //            if (received == 0)
        //            {
        //                Console.WriteLine("Client đã ngắt kết nối.");
        //                break;
        //            }

        //            string chunk = Encoding.UTF8.GetString(buffer, 0, received);
        //            recvBuffer.Append(chunk);

        //            while (true)
        //            {
        //                string current = recvBuffer.ToString();
        //                int idx = current.IndexOf('\n');
        //                if (idx < 0) break;

        //                string line = current[..idx].Trim();
        //                recvBuffer.Remove(0, idx + 1);

        //                if (string.IsNullOrWhiteSpace(line)) continue;

        //                try
        //                {
        //                    using var doc = JsonDocument.Parse(line);
        //                    if (!doc.RootElement.TryGetProperty("Action", out var actionProp))
        //                        continue;

        //                    string action = actionProp.GetString() ?? "";

        //                    switch (action)
        //                    {
        //                        case "MOVE":
        //                            MovePlayer(line);
        //                            break;
        //                        default:
        //                            Console.WriteLine("Unknown action: " + action);
        //                            break;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine("Parse error: " + ex.Message);
        //                    Console.WriteLine("Dữ liệu lỗi: " + line);
        //                }
        //            }
        //        }
        //    }
        //    catch (SocketException sex)
        //    {
        //        Console.WriteLine("⚠️ Lỗi socket: " + sex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("❌ Lỗi trong quá trình nhận dữ liệu: " + ex.Message);
        //    }
        //    finally
        //    {
        //        Console.WriteLine("⛔ Client đã đóng kết nối.");
        //        client.Close();
        //    }
        //}


        //private static Player CreateNewPlayer()
        //{
        //    Random rand = new Random();
        //    var player = new Player
        //    {
        //        Id = Guid.NewGuid(),
        //        X = rand.Next(50, 100),   // ngẫu nhiên từ 50 -> 800 (tùy kích thước form)
        //        Y = rand.Next(50, 100)
        //    };
        //    _players.Add(player);
        //    return player;
        //}

        //private static void MovePlayer(string message)
        //{
        //    try
        //    {
        //        var doc = JsonDocument.Parse(message);
        //        string action = doc.RootElement.GetProperty("Action").GetString() ?? "";

        //        if (action == "MOVE")
        //        {
        //            Guid id = doc.RootElement.GetProperty("PlayerId").GetGuid();
        //            string dir = doc.RootElement.GetProperty("Direction").GetString() ?? "";

        //            var p = _players.FirstOrDefault(x => x.Id == id);
        //            if (p != null)
        //            {
        //                switch (dir)
        //                {
        //                    case "UP": p.Y -= 10; break;
        //                    case "DOWN": p.Y += 10; break;
        //                    case "LEFT": p.X -= 10; break;
        //                    case "RIGHT": p.X += 10; break;
        //                }

        //                BroadcastMessage();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("loi MovePlayer: " + ex.Message);
        //        Console.WriteLine("du lieu nhan: " + message);
        //    }
        //}
        //private static void DisconnectClient(Socket client)
        //{
        //    lock (_clientList)
        //    {
        //        _clientList.Remove(client);
        //    }

        //    if (_clientToPlayer.TryGetValue(client, out Guid playerId))
        //    {
        //        _clientToPlayer.Remove(client);

        //        var playerToRemove = _players.FirstOrDefault(p => p.Id == playerId);
        //        if (playerToRemove != null)
        //        {
        //            _players.Remove(playerToRemove);
        //            Console.WriteLine($"Player {playerId} has leave.");
        //        }
        //    }

        //    try { client.Close(); } catch { }
        //    BroadcastMessage();
        //}
    }
}
