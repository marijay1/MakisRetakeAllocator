using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace MakisRetakeAllocator.Configs;

public class MakisConfig : BasePluginConfig {
    [JsonPropertyName("Number of Pistol rounds")] public int myNumPistolRounds { get; set; } = 5;
}