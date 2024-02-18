using CounterStrikeSharp.API.Modules.Utils;
using Dapper;
using MakisRetakeAllocator.Enums;
using MakisRetakeAllocator.Loadouts;
using MySqlConnector;
using static MakisRetakeAllocator.Loadouts.PlayerLoadout;

namespace MakisRetakeAllocator.Database;

public class DataContext {
    private DbSettings theDatabaseSettings;
    private LoadoutFactory theLoadoutFactory;

    public DataContext(DbSettings aDatabaseSettings, LoadoutFactory aLoadoutFactory) {
        theDatabaseSettings = aDatabaseSettings;
        theLoadoutFactory = aLoadoutFactory;

        initTables();
    }

    //TODO USE
    public void InsertPlayerLoadout(PlayerLoadout aPlayerLoadout) {
        using (var connection = createConnection()) {
            connection.OpenAsync();

            insertLoadout(aPlayerLoadout, connection, CsTeam.CounterTerrorist, RoundType.Pistol);
            insertLoadout(aPlayerLoadout, connection, CsTeam.CounterTerrorist, RoundType.FullBuy);
            insertLoadout(aPlayerLoadout, connection, CsTeam.Terrorist, RoundType.Pistol);
            insertLoadout(aPlayerLoadout, connection, CsTeam.Terrorist, RoundType.FullBuy);
        }
    }

    private void insertLoadout(PlayerLoadout aPlayerLoadout, MySqlConnection connection, CsTeam aTeam, RoundType aRoundType) {
        string tableName = getTableName(aTeam, aRoundType);

        string sql = $@"INSERT INTO {tableName} (SteamId, {getColumnNames(aTeam)}) VALUES (@SteamId, {getColumnValues(aPlayerLoadout, aTeam, aRoundType)})";

        using (var command = new MySqlCommand(sql, connection)) {
            command.Parameters.AddWithValue("@SteamId", aPlayerLoadout.getSteamId());
            command.ExecuteNonQueryAsync();
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

    public PlayerLoadout loadPlayerLoadout(ulong aSteamId) {
        using (MySqlConnection myConnection = createConnection()) {
            myConnection.OpenAsync();

            Dictionary<RoundType, PlayerItems> myCounterTerroristLoadouts = loadLoadouts(myConnection, aSteamId, "maki_pistol_weapons", "maki_fullbuy_weapons", "ct");
            Dictionary<RoundType, PlayerItems> myTerroristLoadouts = loadLoadouts(myConnection, aSteamId, "maki_pistol_weapons", "maki_fullbuy_weapons", "t");

            return new PlayerLoadout(aSteamId, myCounterTerroristLoadouts, myTerroristLoadouts);
        }
    }

    private Dictionary<RoundType, PlayerItems> loadLoadouts(MySqlConnection aConnection, ulong aSteamId, string aPistolTable, string aFullbuyTable, string aFieldPrefix) {
        Dictionary<RoundType, PlayerItems> myLoadouts = new Dictionary<RoundType, PlayerItems>();

        // Load Pistol round type
        string myPistolSql = $@"
        SELECT SteamId, {aFieldPrefix}_secondary, {aFieldPrefix}_armor, {aFieldPrefix}_grenades, {aFieldPrefix}_kit FROM {aPistolTable} WHERE SteamId = @SteamId
    ";
        using (MySqlCommand myPistolCommand = new MySqlCommand(myPistolSql, aConnection)) {
            myPistolCommand.Parameters.AddWithValue("@SteamId", aSteamId);
            using (MySqlDataReader pistolReader = myPistolCommand.ExecuteReader()) {
                while (pistolReader.Read()) {
                    PlayerItems myPlayerItems = createPlayerItems(pistolReader, aFieldPrefix);
                    myLoadouts[RoundType.Pistol] = myPlayerItems;
                }
            }
        }

        // Load FullBuy round type
        string myFullBuySql = $@"
        SELECT SteamId, {aFieldPrefix}_primary, {aFieldPrefix}_secondary, {aFieldPrefix}_armor, {aFieldPrefix}_grenades, {aFieldPrefix}_kit FROM {aFullbuyTable} WHERE SteamId = @SteamId
    ";
        using (MySqlCommand myFullBuyCommand = new MySqlCommand(myFullBuySql, aConnection)) {
            myFullBuyCommand.Parameters.AddWithValue("@SteamId", aSteamId);
            using (MySqlDataReader fullBuyReader = myFullBuyCommand.ExecuteReader()) {
                while (fullBuyReader.Read()) {
                    PlayerItems myPlayerItems = createPlayerItems(fullBuyReader, aFieldPrefix);
                    myLoadouts[RoundType.FullBuy] = myPlayerItems;
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
        String myConnectionString = $"Server={theDatabaseSettings.theServer}; Database={theDatabaseSettings.theDatabase}; Uid={theDatabaseSettings.theUserId}; Pwd={theDatabaseSettings.thePassword}; Port={theDatabaseSettings.thePort}";
        return new MySqlConnection(myConnectionString);
    }

    private void initTables() {
        using var myConnection = createConnection();

        var myPistolSql = @"
                CREATE TABLE IF NOT EXISTS maki_pistol_weapons (
                    `SteamId` VARCHAR(64) COLLATE utf8mb4_unicode_ci UNIQUE NOT NULL,
                    `ct_secondary` VARCHAR(32) NOT NULL,
                    `ct_armor` VARCHAR(32) NOT NULL,
                    `ct_grenades` VARCHAR(32) NOT NULL,
                    `ct_kit` BOOLEAN NOT NULL,
                    `t_secondary` VARCHAR(32) NOT NULL,
                    `t_armor` VARCHAR(32) NOT NULL,
                    `t_grenades` VARCHAR(32) NOT NULL,
                    `t_kit` BOOLEAN NULL,
                    UNIQUE (`SteamId`)
                );
                ";

        var myFullBuySql = @"
                CREATE TABLE IF NOT EXISTS maki_fullbuy_weapons (
                    `SteamId` VARCHAR(64) COLLATE utf8mb4_unicode_ci UNIQUE NOT NULL,
                    `ct_primary` VARCHAR(32),
                    `ct_secondary` VARCHAR(32) NOT NULL,
                    `ct_armor` VARCHAR(32) NOT NULL,
                    `ct_grenades` VARCHAR(32) NOT NULL,
                    `ct_kit` BOOLEAN NOT NULL,
                    `t_primary` VARCHAR(32),
                    `t_secondary` VARCHAR(32) NOT NULL,
                    `t_armor` VARCHAR(32) NOT NULL,
                    `t_grenades` VARCHAR(32) NOT NULL,
                    `t_kit` BOOLEAN NULL,
                    UNIQUE (`SteamId`)
                );
                ";

        myConnection.Execute(myPistolSql);
        myConnection.Execute(myFullBuySql);
    }

    public class DbSettings {
        public string? theServer { get; set; }
        public string? theDatabase { get; set; }
        public string? theUserId { get; set; }
        public string? thePassword { get; set; }
        public string? thePort { get; set; }

        public DbSettings(string? aServer, string? aDatabase, string? aUserId, string? aPassword, string? aPort) {
            theServer = aServer;
            theDatabase = aDatabase;
            theUserId = aUserId;
            thePassword = aPassword;
            thePort = aPort;
        }
    }
}