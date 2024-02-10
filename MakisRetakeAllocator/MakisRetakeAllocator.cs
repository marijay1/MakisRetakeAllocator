using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Utils;
using MakisRetakeAllocator.Configs;

namespace MakisRetakeAllocator;

[MinimumApiVersion(159)]
public partial class MakisRetakeAllocator : BasePlugin, IPluginConfig<MakisConfig> {
    private const string Version = "0.0.1";

    public override string ModuleName => "Maki's Retake Allocator";
    public override string ModuleVersion => Version;
    public override string ModuleAuthor => "Panduuuh";
    public override string ModuleDescription => "Main Retake Allocator plugin for Maki's";

    public static readonly string LogPrefix = $"[Maki's Retakes Allocator {Version}] ";
    public static readonly string MessagePrefix = $"[{ChatColors.LightPurple}Maki's Retakes{ChatColors.White}] ";

    public MakisConfig Config { get; set; } = null!;

    public MakisRetakeAllocator() {
    }

    public void OnConfigParsed(MakisConfig aMakiConfig) {
        Config = aMakiConfig;
    }

    public override void Load(bool aHotReload) {
        if (aHotReload) {
            Server.ExecuteCommand($"map {Server.MapName}");
        }

        Console.WriteLine($"{LogPrefix}Plugin loaded!");
    }
}