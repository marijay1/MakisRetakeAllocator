using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Utils;
using MakisRetakeAllocator.Configs;
using MakisRetakeAllocator.Database;
using MakisRetakeAllocator.Enums;
using MakisRetakeAllocator.Loadouts;
using static MakisRetakeAllocator.Database.DataContext;

namespace MakisRetakeAllocator;

[MinimumApiVersion(172)]
public partial class MakisRetakeAllocator : BasePlugin, IPluginConfig<MakisConfig> {
    private const string Version = "1.0.0";

    public override string ModuleName => "Maki's Retake Allocator";
    public override string ModuleVersion => Version;
    public override string ModuleAuthor => "Panduuuh";
    public override string ModuleDescription => "Main Retake Allocator plugin for Maki's";

    public static readonly string LogPrefix = $"[Maki's Retakes Allocator {Version}] ";
    public static readonly string MessagePrefix = $"[{ChatColors.LightPurple}Maki's Retakes{ChatColors.White}] ";
    public static MakisRetakeAllocator thePlugin = null!;

    private int theCurrentRound { get; set; }
    private RoundType theRoundType { get; set; }

    public MakisConfig Config { get; set; } = new MakisConfig();
    private LoadoutFactory theLoadoutFactory;
    private DataContext theDataContext;

    public MakisRetakeAllocator() {
        thePlugin = this;
        theLoadoutFactory = new LoadoutFactory();
    }

    public void OnConfigParsed(MakisConfig aMakiConfig) {
        Config = aMakiConfig;
    }

    public override void Load(bool aHotReload) {
        DatabaseConfig myDatabaseConfig = Config.theDatabaseConfig;
        DbSettings myDbSettings = new DbSettings(myDatabaseConfig.theServer, myDatabaseConfig.theDatabase, myDatabaseConfig.theUserId, myDatabaseConfig.thePassword, myDatabaseConfig.thePort);
        theDataContext = new DataContext(myDbSettings, theLoadoutFactory);

        if (aHotReload) {
            Server.ExecuteCommand($"map {Server.MapName}");
        }

        Console.WriteLine($"{LogPrefix}Plugin loaded!");
    }
}