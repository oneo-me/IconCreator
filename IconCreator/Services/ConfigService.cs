using IconCreator.Models;
using Newtonsoft.Json;

namespace IconCreator.Services;

public class ConfigService
{
    readonly string _configFile;

    public Config Config { get; }

    public ConfigService(string dataFolder)
    {
        _configFile = Path.Combine(dataFolder, "config.json");
        Directory.CreateDirectory(dataFolder);

        if (File.Exists(_configFile))
        {
            var jsonStr = File.ReadAllText(_configFile);
            Config = JsonConvert.DeserializeObject<Config>(jsonStr) ?? new Config();
        }

        Config ??= new();
    }

    public void Save()
    {
        var jsonStr = JsonConvert.SerializeObject(Config, Formatting.Indented);
        File.WriteAllText(_configFile, jsonStr);
    }
}
