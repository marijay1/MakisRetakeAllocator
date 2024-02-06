using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MakisRetakeAllocator.Configs;
public class MakisConfig : BasePluginConfig {
    [JsonPropertyName("Number of Pistol rounds")] public int myNumPistolRounds { get; set; } = 5;
}
