using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace MakisRetakeAllocator.Configs;

public class MakisConfig : BasePluginConfig {
    [JsonPropertyName("Database config")] public DatabaseConfig theDatabaseConfig { get; set; } = new DatabaseConfig();
    [JsonPropertyName("Retakes config")] public RetakesConfig theRetakesConfig { get; set; } = new RetakesConfig();
}

public class DatabaseConfig {
    [JsonPropertyName("Host")] public string theServer { get; set; } = "localhost";
    [JsonPropertyName("Database")] public string theDatabase { get; set; } = "database";
    [JsonPropertyName("Username")] public string theUserId { get; set; } = "username";
    [JsonPropertyName("Password")] public string thePassword { get; set; } = "password";
    [JsonPropertyName("Port")] public string thePort { get; set; } = "3306";
}

public class RetakesConfig {
    [JsonPropertyName("Number of Pistol rounds")] public int theNumPistolRounds { get; set; } = 5;
    [JsonPropertyName("Starting Terrorist money")] public int theTerroristStartingMoney { get; set; } = 4100;
    [JsonPropertyName("Starting Counter-Terrorist money")] public int theCounterTerroristStartingMoney { get; set; } = 4150;
    [JsonPropertyName("Seconds until menu timeout")] public int theSecondsUntilMenuTimeout { get; set; } = 30;
}