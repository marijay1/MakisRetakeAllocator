using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace MakisRetakeAllocator.Configs;

public class MakisConfig : BasePluginConfig {

    //TODO Database variables
    [JsonPropertyName("Number of Pistol rounds")] public int theNumPistolRounds { get; set; } = 5;

    [JsonPropertyName("Starting money")] public int theStartingMoney { get; set; } = 4850;
    [JsonPropertyName("Seconds until menu timeout")] public int theSecondsUntilMenuTimeout { get; set; } = 30;
}