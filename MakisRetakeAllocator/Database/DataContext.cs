using CounterStrikeSharp.API.Modules.Utils;
using Dapper;
using MakisRetakeAllocator.Enums;
using MakisRetakeAllocator.Loadouts;

using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
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
        using (var connection = createConnection()) {
            await connection.OpenAsync();

            await insertLoadout(aPlayerLoadout, connection, CsTeam.CounterTerrorist, RoundType.Pistol);
            await insertLoadout(aPlayerLoadout, connection, CsTeam.CounterTerrorist, RoundType.FullBuy);
            await insertLoadout(aPlayerLoadout, connection, CsTeam.Terrorist, RoundType.Pistol);
            await insertLoadout(aPlayerLoadout, connection, CsTeam.Terrorist, RoundType.FullBuy);
        }
    }

    private async Task insertLoadout(PlayerLoadout aPlayerLoadout, MySqlConnection connection, CsTeam aTeam, RoundType aRoundType) {
        string tableName = getTableName(aTeam, aRoundType);

        string sql = $@"INSERT INTO {tableName} (SteamId, {getColumnNames(aTeam)}) VALUES (@SteamId, {getColumnValues(aPlayerLoadout, aTeam, aRoundType)})";

        using (var command = new MySqlCommand(sql, connection)) {
            command.Parameters.AddWithValue("@SteamId", aPlayerLoadout.getSteamId());
            await command.ExecuteNonQueryAsync();
        }
    }

    private string getTableName(CsTeam aTeam, RoundType aRoundType) {
        return aRoundType.Equals(RoundType.Pistol) ? "maki_pistol_weapons" : "maki_fullbuy_weapons";
    }

    private string getColumnNames(CsTeam aTeam) {
        return aTeam switch {
            CsTeam.CounterTerrorist => "ct_primary, ct_secondary, ct_armor, ct_grenades, ct_kit",
            CsTeam.Terrorist => "t_primary, t_secondary, t_armor, t_grenades",
            _ => throw new ArgumentOutOfRangeException(nameof(aTeam))
        };
    }

    private string getColumnValues(PlayerLoadout aPlayerLoadout, CsTeam aTeam, RoundType aRoundType) {
        PlayerItems myPlayerItems = aPlayerLoadout.getLoadouts(aTeam)[aRoundType];
        string myGrenades = string.Join(",", myPlayerItems.theGrenadePreference.Select(aGrenade => aGrenade.theName));

        return $"'{myPlayerItems.thePrimaryWeapon?.theName}', '{myPlayerItems.theSecondaryWeapon?.theName}', '{myPlayerItems.theArmor?.theName}', '{myGrenades}', {(aTeam == CsTeam.CounterTerrorist ? myPlayerItems.theIsKitEnabled.ToString() : "NULL")}";
    }

    public async Task<PlayerLoadout> loadPlayerLoadout(int aSteamId) {
        using (MySqlConnection myConnection = createConnection()) {
            await myConnection.OpenAsync();

            Dictionary<RoundType, PlayerItems> myCounterTerroristLoadouts = await loadLoadouts(myConnection, aSteamId, "maki_pistol_weapons", "maki_fullbuy_weapons", "ct");
            Dictionary<RoundType, PlayerItems> myTerroristLoadouts = await loadLoadouts(myConnection, aSteamId, "maki_pistol_weapons", "maki_fullbuy_weapons", "t");

            return new PlayerLoadout(aSteamId, myCounterTerroristLoadouts, myTerroristLoadouts);
        }
    }

    private async Task<Dictionary<RoundType, PlayerItems>> loadLoadouts(MySqlConnection aConnection, int aSteamId, string aPistolTable, string aFullbuyTable, string aFieldPrefix) {
        string mySql = $@"
        SELECT * FROM {aPistolTable} WHERE SteamId = @SteamId
        UNION
        SELECT * FROM {aFullbuyTable} WHERE SteamId = @SteamId
    ";

        var myLoadouts = new Dictionary<RoundType, PlayerItems>();

        using (MySqlCommand mySqlCommand = new MySqlCommand(mySql, aConnection)) {
            mySqlCommand.Parameters.AddWithValue("@SteamId", aSteamId);

            using (MySqlDataReader mySqlReader = mySqlCommand.ExecuteReader()) {
                while (await mySqlReader.ReadAsync()) {
                    RoundType myRoundType = mySqlReader.GetString("RoundType") == "Pistol" ? RoundType.Pistol : RoundType.FullBuy;
                    PlayerItems items = createPlayerItems(mySqlReader, aFieldPrefix);
                    myLoadouts.Add(myRoundType, items);
                }
            }
        }

        return myLoadouts;
    }

    private PlayerItems createPlayerItems(MySqlDataReader aSqlReader, string aFieldPrefix) {
        return new PlayerItems(
            theLoadoutFactory.getLoadoutItem(aSqlReader[$"{aFieldPrefix}_primary"] as string),
            theLoadoutFactory.getLoadoutItem(aSqlReader[$"{aFieldPrefix}_secondary"] as string),
            theLoadoutFactory.getLoadoutItem(aSqlReader[$"{aFieldPrefix}_armor"] as string),
            getGrenades(aSqlReader, $"{aFieldPrefix}_grenade"),
            aSqlReader.IsDBNull(aSqlReader.GetOrdinal("ct_kit")) ? null : (bool?)aSqlReader["ct_kit"]
        );
    }

    private List<LoadoutItem> getGrenades(MySqlDataReader aSqlReader, string aColumnName) {
        string myGrenadesString = aSqlReader[aColumnName] as string;
        if (string.IsNullOrEmpty(myGrenadesString)) {
            return new List<LoadoutItem>();
        }

        List<string> myGrenadesStringList = myGrenadesString.Split(',').ToList();
        return myGrenadesStringList.Select(aGrenadeName => theLoadoutFactory.getLoadoutItem(aGrenadeName)).ToList();
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
                        `ct_grenades` VARCHAR(32) NOT NULL,
                        `ct_kit` BOOLEAN NOT NULL,
                        `t_secondary` VARCHAR(32) NOT NULL,
                        `t_armor` VARCHAR(32) NOT NULL,
                        `t_grenades` VARCHAR(32) NOT NULL,
                        UNIQUE ('SteamId')
                    """;

        var myFullBuySql = """
                    CREATE TABLE IF NOT EXISTS maki_fullbuy_weapons (
                        'SteamId' VARCHAR(64) COLLATE 'utf8mb4_unicode_ci' UNIQUE NOT NULL,
                        'ct_primary' VARCHAR(32),
                        `ct_secondary` VARCHAR(32) NOT NULL,
                        `ct_armor` VARCHAR(32) NOT NULL,
                        `ct_grenades` VARCHAR(32) NOT NULL,
                        `ct_kit` BOOLEAN NOT NULL,
                        't_primary' VARCHAR(32),
                        `t_secondary` VARCHAR(32) NOT NULL,
                        `t_armor` VARCHAR(32) NOT NULL,
                        `t_grenades` VARCHAR(32) NOT NULL,
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