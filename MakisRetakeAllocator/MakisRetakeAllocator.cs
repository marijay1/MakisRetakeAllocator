using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Utils;
using MakisRetakeAllocator.Configs;
using MakisRetakeAllocator.Database;
using MakisRetakeAllocator.Enums;
using MakisRetakeAllocator.Loadouts;

namespace MakisRetakeAllocator;

[MinimumApiVersion(178)]
public partial class MakisRetakeAllocator : BasePlugin, IPluginConfig<MakisConfig> {
    private const string Version = "1.2.0";

    public override string ModuleName => "Maki's Retake Allocator";
    public override string ModuleVersion => Version;
    public override string ModuleAuthor => "Panduuuh";
    public override string ModuleDescription => "Main Retake Allocator plugin for Maki's";

    public static readonly string LogPrefix = $"[Maki's Retakes Allocator {Version}] ";
    public static readonly string MessagePrefix = $"[{ChatColors.LightPurple}Maki's Retakes{ChatColors.White}]";
    public static MakisRetakeAllocator Plugin;

    private int theCurrentRound;
    private RoundType theRoundType;

    public MakisConfig Config { get; set; } = new MakisConfig();
    private LoadoutFactory theLoadoutFactory;
    private PlayerLoadoutContext thePlayerLoadoutContext;

    public MakisRetakeAllocator() {
        Plugin = this;
    }

    public void OnConfigParsed(MakisConfig aMakiConfig) {
        Config = aMakiConfig;
    }

    public override void Load(bool aHotReload) {
        DatabaseConfig myDatabaseConfig = Config.theDatabaseConfig;
        theLoadoutFactory = new LoadoutFactory();
        SqlDataAccess mySqlDataAccess = new SqlDataAccess(myDatabaseConfig);
        thePlayerLoadoutContext = new PlayerLoadoutContext(mySqlDataAccess, theLoadoutFactory);

        if (aHotReload) {
            Server.ExecuteCommand($"map {Server.MapName}");
        }

        Console.WriteLine($"{LogPrefix}Plugin loaded!");
    }
}