using System.Text.Json;

public class GameConfig
{
    public string ServerIp { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 9000;

    public static GameConfig Load(string path = "Config.json")
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<GameConfig>(json) ?? new GameConfig();
    }
}
