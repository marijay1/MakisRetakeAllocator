using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace MakisRetakeAllocator.Configs;

public class MakisConfig : BasePluginConfig {

    //TODO Database variables
    [JsonPropertyName("Number of Pistol rounds")] public int theNumPistolRounds { get; set; } = 5;

    [JsonPropertyName("Starting Terrorist money")] public int theTerroristStartingMoney { get; set; } = 4850;
    [JsonPropertyName("Starting Counter-Terrorist money")] public int theCounterTerroristStartingMoney { get; set; } = 4450;
    [JsonPropertyName("Seconds until menu timeout")] public int theSecondsUntilMenuTimeout { get; set; } = 30;
}