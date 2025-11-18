using System.Text.Json;

public class GameConfig
{
    public string ServerIp { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 9000;

    public static GameConfig Load(string path = "Config.json")
    {
        if (!File.Exists(path))
        {
            var defaultConfig = new GameConfig();
            string json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
            return defaultConfig;
        }
        string fileContent = File.ReadAllText(path);
        return JsonSerializer.Deserialize<GameConfig>(fileContent) ?? new GameConfig();
    }
}
