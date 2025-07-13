using Newtonsoft.Json;

public class BatteryStorageConfig
{
    public string OpenBatteryStorageKey { get; set; } = "G";

    public static BatteryStorageConfig Load(string path)
    {
        if (!File.Exists(path))
        {
            var defaultConfig = new BatteryStorageConfig();
            File.WriteAllText(path, JsonConvert.SerializeObject(defaultConfig, Formatting.Indented));
            return defaultConfig;
        }

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<BatteryStorageConfig>(json);
    }
}