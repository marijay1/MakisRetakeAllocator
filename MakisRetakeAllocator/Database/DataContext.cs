using CounterStrikeSharp.API.Modules.Utils;
using Dapper;
using MakisRetakeAllocator.Enums;
using MakisRetakeAllocator.Loadouts;

using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using static MakisRetakeAllocator.Loadouts.PlayerLoadout;

namespace MakisRetakeAllocator.Database;

public class DataContext {
    private DbSettings theDatabaseSettings;
    private LoadoutFactory theLoadoutFactory;

    public DataContext(IOptions<DbSettings> aDatabaseSettings, LoadoutFactory aLoadoutFactory) {
        theDatabaseSettings = aDatabaseSettings.Value;
        theLoadoutFactory = aLoadoutFactory;
    }

    public async Task InsertPlayerLoadout(PlayerLoadout aPlayerLoadout) {
        using (MySqlConnection myConnection = createConnection()) {
            await myConnection.OpenAsync();

            await insertPistolWeapons(aPlayerLoadout, myConnection);
            await insertFullbuyWeapons(aPlayerLoadout, myConnection);
        }
    }

    private async Task insertPistolWeapons(PlayerLoadout aPlayerLoadout, MySqlConnection aConnection) {
        string myPistolSql = @"INSERT INTO maki_pistol_weapons (SteamId, ct_primary, ct_secondary, ct_armor, ct_grenade, ct_kit, t_primary, t_secondary, t_armor, t_grenade)
                       VALUES (@SteamId, @CTSeconday, @CTArmor, @CTGrenade, @CTKit, @TSecondary, @TArmor, @TGrenade)";

        using (MySqlCommand myCommand = new MySqlCommand(myPistolSql, aConnection)) {
            myCommand.Parameters.AddWithValue("@SteamId", aPlayerLoadout.getSteamId());
            myCommand.Parameters.AddWithValue("@CTSeconday", aPlayerLoadout.getLoadouts(CsTeam.CounterTerrorist)[RoundType.Pistol].theSecondaryWeapon);
            myCommand.Parameters.AddWithValue("@CTArmor", aPlayerLoadout.getLoadouts(CsTeam.CounterTerrorist)[RoundType.Pistol].theArmor);
            myCommand.Parameters.AddWithValue("@CTGrenade", aPlayerLoadout.getLoadouts(CsTeam.CounterTerrorist)[RoundType.Pistol].theGrenadePreference);
            myCommand.Parameters.AddWithValue("@CTKit", aPlayerLoadout.getLoadouts(CsTeam.CounterTerrorist)[RoundType.Pistol].theIsKitEnabled);
            myCommand.Parameters.AddWithValue("@TSecondary", aPlayerLoadout.getLoadouts(CsTeam.Terrorist)[RoundType.Pistol].theSecondaryWeapon);
            myCommand.Parameters.AddWithValue("@TArmor", aPlayerLoadout.getLoadouts(CsTeam.Terrorist)[RoundType.Pistol].theArmor);
            myCommand.Parameters.AddWithValue("@TGrenade", aPlayerLoadout.getLoadouts(CsTeam.Terrorist)[RoundType.Pistol].theGrenadePreference);

            await myCommand.ExecuteNonQueryAsync();
        }
    }

    private async Task insertFullbuyWeapons(PlayerLoadout aPlayerLoadout, MySqlConnection aConnection) {
        string myFullbuySql = @"INSERT INTO maki_fullbuy_weapons (SteamId, ct_primary, ct_secondary, ct_armor, ct_grenade, ct_kit, t_primary, t_secondary, t_armor, t_grenade)
                       VALUES (@SteamId, @CTPrimary, @CTSecondary, @CTArmor, @CTGrenade, @CTKit, @TPrimary, @TSecondary, @TArmor, @TGrenade)"
        ;

        using (MySqlCommand myCommand = new MySqlCommand(myFullbuySql, aConnection)) {
            myCommand.Parameters.AddWithValue("@SteamId", aPlayerLoadout.getSteamId());
            myCommand.Parameters.AddWithValue("@CTPrimary", aPlayerLoadout.getLoadouts(CsTeam.CounterTerrorist)[RoundType.FullBuy].thePrimaryWeapon);
            myCommand.Parameters.AddWithValue("@CTSecondary", aPlayerLoadout.getLoadouts(CsTeam.CounterTerrorist)[RoundType.FullBuy].theSecondaryWeapon);
            myCommand.Parameters.AddWithValue("@CTArmor", aPlayerLoadout.getLoadouts(CsTeam.CounterTerrorist)[RoundType.FullBuy].theArmor);
            myCommand.Parameters.AddWithValue("@CTGrenade", aPlayerLoadout.getLoadouts(CsTeam.CounterTerrorist)[RoundType.FullBuy].theGrenadePreference);
            myCommand.Parameters.AddWithValue("@CTKit", aPlayerLoadout.getLoadouts(CsTeam.CounterTerrorist)[RoundType.FullBuy].theIsKitEnabled);
            myCommand.Parameters.AddWithValue("@TPrimary", aPlayerLoadout.getLoadouts(CsTeam.Terrorist)[RoundType.FullBuy].thePrimaryWeapon);
            myCommand.Parameters.AddWithValue("@TSecondary", aPlayerLoadout.getLoadouts(CsTeam.Terrorist)[RoundType.FullBuy].theSecondaryWeapon);
            myCommand.Parameters.AddWithValue("@TArmor", aPlayerLoadout.getLoadouts(CsTeam.Terrorist)[RoundType.FullBuy].theArmor);
            myCommand.Parameters.AddWithValue("@TGrenade", aPlayerLoadout.getLoadouts(CsTeam.Terrorist)[RoundType.FullBuy].theGrenadePreference);

            await myCommand.ExecuteNonQueryAsync();
        }
    }

    public async Task<PlayerLoadout> loadPlayerLoadout(int steamId) {
        using (MySqlConnection connection = createConnection()) {
            await connection.OpenAsync();
            Dictionary<RoundType, PlayerItems> counterTerroristLoadouts = loadLoadouts(connection, steamId, "maki_pistol_weapons", "maki_fullbuy_weapons", "ct");
            Dictionary<RoundType, PlayerItems> terroristLoadouts = loadLoadouts(connection, steamId, "maki_pistol_weapons", "maki_fullbuy_weapons", "t");
            return new PlayerLoadout(steamId, counterTerroristLoadouts, terroristLoadouts);
        }
    }

    private Dictionary<RoundType, PlayerItems> loadLoadouts(MySqlConnection aConnection, int aSteamId, string aPistolTable, string aFullbuyTable, string aFieldPrefix) {
        string pistolSql = $@"SELECT * FROM {aPistolTable} WHERE SteamId = @SteamId";
        string fullbuySql = $@"SELECT * FROM {aFullbuyTable} WHERE SteamId = @SteamId";

        var myPlayerloadout = new Dictionary<RoundType, PlayerItems>();

        using (MySqlCommand mySqlCommand = new MySqlCommand(pistolSql, aConnection)) {
            mySqlCommand.Parameters.AddWithValue("@SteamId", aSteamId);
            using (MySqlDataReader mySqlReader = mySqlCommand.ExecuteReader()) {
                if (mySqlReader.Read()) {
                    var myPistolItems = createPlayerItems(mySqlReader, aFieldPrefix);
                    myPlayerloadout.Add(RoundType.Pistol, myPistolItems);
                }
            }
        }

        using (MySqlCommand command = new MySqlCommand(fullbuySql, aConnection)) {
            command.Parameters.AddWithValue("@SteamId", aSteamId);
            using (MySqlDataReader mySqlReader = command.ExecuteReader()) {
                if (mySqlReader.Read()) {
                    var fullbuyItems = createPlayerItems(mySqlReader, aFieldPrefix);
                    myPlayerloadout.Add(RoundType.FullBuy, fullbuyItems);
                }
            }
        }

        return myPlayerloadout;
    }

    private PlayerItems createPlayerItems(MySqlDataReader aSqlReader, string aFieldPrefix) {
        //TODO
        return new PlayerItems(
            getLoadoutItem(aSqlReader, $"{aFieldPrefix}_primary"),
            getLoadoutItem(aSqlReader, $"{aFieldPrefix}_secondary"),
            getLoadoutItem(aSqlReader, $"{aFieldPrefix}_armor"),
            getLoadoutItem(aSqlReader, $"{aFieldPrefix}_grenade"),
            aFieldPrefix == "ct" ? Convert.ToBoolean(aSqlReader["ct_kit"]) : null
        );
    }

    private LoadoutItem? getLoadoutItem(MySqlDataReader reader, string columnName) {
        var value = reader[columnName];
        return value != DBNull.Value ? theLoadoutFactory.getLoadoutItem(value.ToString()) : null;
    }

    private MySqlConnection createConnection() {
        String myConnectionString = $"Server={theDatabaseSettings.theServer}:{theDatabaseSettings.thePort}; Database={theDatabaseSettings.theDatabase}; Uid={theDatabaseSettings.theUserId}; Pwd={theDatabaseSettings.thePassword}";
        return new MySqlConnection(myConnectionString);
    }

    private async Task initTables() {
        using var myConnection = createConnection();
        var myPistolSql = """
                    CREATE TABLE IF NOT EXISTS maki_pistol_weapons (
                        'SteamId' VARCHAR(64) COLLATE 'utf8mb4_unicode_ci' UNIQUE NOT NULL,
                        `ct_secondary` VARCHAR(32) NOT NULL,
                        `ct_armor` VARCHAR(32) NOT NULL,
                        `ct_grenade` VARCHAR(32) NOT NULL,
                        `ct_kit` BOOLEAN NOT NULL,
                        `t_secondary` VARCHAR(32) NOT NULL,
                        `t_armor` VARCHAR(32) NOT NULL,
                        `t_grenade` VARCHAR(32) NOT NULL,
                        UNIQUE ('SteamId')
                    """;

        var myFullBuySql = """
                    CREATE TABLE IF NOT EXISTS maki_fullbuy_weapons (
                        'SteamId' VARCHAR(64) COLLATE 'utf8mb4_unicode_ci' UNIQUE NOT NULL,
                        'ct_primary' VARCHAR(32),
                        `ct_secondary` VARCHAR(32) NOT NULL,
                        `ct_armor` VARCHAR(32) NOT NULL,
                        `ct_grenade` VARCHAR(32) NOT NULL,
                        `ct_kit` BOOLEAN NOT NULL,
                        't_primary' VARCHAR(32),
                        `t_secondary` VARCHAR(32) NOT NULL,
                        `t_armor` VARCHAR(32) NOT NULL,
                        `t_grenade` VARCHAR(32) NOT NULL,
                        UNIQUE ('SteamId')
                    """;

        await myConnection.ExecuteAsync(myPistolSql);
        await myConnection.ExecuteAsync(myFullBuySql);
    }

    public class DbSettings {
        public string? theServer { get; set; }
        public string? theDatabase { get; set; }
        public string? theUserId { get; set; }
        public string? thePassword { get; set; }
        public int? thePort { get; set; } = 3306;
    }
}