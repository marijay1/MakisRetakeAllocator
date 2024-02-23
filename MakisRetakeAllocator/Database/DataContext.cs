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

    public void insertPlayerLoadout(PlayerLoadout aPlayerLoadout) {
        using (MySqlConnection myConnection = createConnection()) {
            myConnection.Open();

            insertOrUpdatePistolWeapons(aPlayerLoadout, myConnection);
            insertOrUpdateFullbuyWeapons(aPlayerLoadout, myConnection);
        }
    }

    private void insertOrUpdatePistolWeapons(PlayerLoadout aPlayerLoadout, MySqlConnection aConnection) {
        string myPistolSql = @"
                INSERT INTO maki_pistol_weapons (SteamId, ct_secondary, ct_armor, ct_grenades, ct_kit, t_secondary, t_armor, t_grenades, t_kit)
                VALUES (@SteamId, @CTSecondary, @CTArmor, @CTGrenades, @CTKit, @TSecondary, @TArmor, @TGrenades, @TKit)
                ON DUPLICATE KEY UPDATE
            ct_secondary = VALUES(ct_secondary),
            ct_armor = VALUES(ct_armor),
            ct_grenades = VALUES(ct_grenades),
            ct_kit = VALUES(ct_kit),
            t_secondary = VALUES(t_secondary),
            t_armor = VALUES(t_armor),
            t_grenades = VALUES(t_grenades),
            t_kit = VALUES(t_kit)";

        using (MySqlCommand myCommand = new MySqlCommand(myPistolSql, aConnection)) {
            PlayerItems myCTPlayerItems = aPlayerLoadout.getLoadouts(CsTeam.CounterTerrorist)[RoundType.Pistol];
            PlayerItems myTPlayerItems = aPlayerLoadout.getLoadouts(CsTeam.Terrorist)[RoundType.Pistol];
            myCommand.Parameters.AddWithValue("@SteamId", aPlayerLoadout.getSteamId());
            myCommand.Parameters.AddWithValue("@CTSecondary", myCTPlayerItems.theSecondaryWeapon.theName);
            myCommand.Parameters.AddWithValue("@CTArmor", myCTPlayerItems.theArmor.theName);
            myCommand.Parameters.AddWithValue("@CTGrenades", string.Join(",", myCTPlayerItems.theGrenadePreference.Select(aGrenade => aGrenade.theName)));
            myCommand.Parameters.AddWithValue("@CTKit", myCTPlayerItems.theIsKitEnabled);
            myCommand.Parameters.AddWithValue("@TSecondary", myTPlayerItems.theSecondaryWeapon.theName);
            myCommand.Parameters.AddWithValue("@TArmor", myTPlayerItems.theArmor.theName);
            myCommand.Parameters.AddWithValue("@TGrenades", string.Join(",", myTPlayerItems.theGrenadePreference.Select(aGrenade => aGrenade.theName)));
            myCommand.Parameters.AddWithValue("@TKit", null);

            myCommand.ExecuteNonQuery();
        }
    }

    private void insertOrUpdateFullbuyWeapons(PlayerLoadout aPlayerLoadout, MySqlConnection aConnection) {
        string myFullbuySql = @"
                INSERT INTO maki_fullbuy_weapons (SteamId, ct_primary, ct_secondary, ct_armor, ct_grenades, ct_kit, t_primary, t_secondary, t_armor, t_grenades, t_kit)
                VALUES (@SteamId, @CTPrimary, @CTSecondary, @CTArmor, @CTGrenades, @CTKit, @TPrimary, @TSecondary, @TArmor, @TGrenades, @TKit)
                ON DUPLICATE KEY UPDATE
            ct_primary = VALUES(ct_primary),
            ct_secondary = VALUES(ct_secondary),
            ct_armor = VALUES(ct_armor),
            ct_grenades = VALUES(ct_grenades),
            ct_kit = VALUES(ct_kit),
            t_primary = VALUES(t_primary),
            t_secondary = VALUES(t_secondary),
            t_armor = VALUES(t_armor),
            t_grenades = VALUES(t_grenades),
            t_kit = VALUES(t_kit)";

        using (MySqlCommand myCommand = new MySqlCommand(myFullbuySql, aConnection)) {
            PlayerItems myCTPlayerItems = aPlayerLoadout.getLoadouts(CsTeam.CounterTerrorist)[RoundType.FullBuy];
            PlayerItems myTPlayerItems = aPlayerLoadout.getLoadouts(CsTeam.Terrorist)[RoundType.FullBuy];
            myCommand.Parameters.AddWithValue("@SteamId", aPlayerLoadout.getSteamId());
            myCommand.Parameters.AddWithValue("@CTPrimary", myCTPlayerItems.thePrimaryWeapon.theName);
            myCommand.Parameters.AddWithValue("@CTSecondary", myCTPlayerItems.theSecondaryWeapon.theName);
            myCommand.Parameters.AddWithValue("@CTArmor", myCTPlayerItems.theArmor.theName);
            myCommand.Parameters.AddWithValue("@CTGrenades", string.Join(",", myCTPlayerItems.theGrenadePreference.Select(aGrenade => aGrenade.theName)));
            myCommand.Parameters.AddWithValue("@CTKit", myCTPlayerItems.theIsKitEnabled);
            myCommand.Parameters.AddWithValue("@TPrimary", myTPlayerItems.thePrimaryWeapon.theName);
            myCommand.Parameters.AddWithValue("@TSecondary", myTPlayerItems.theSecondaryWeapon.theName);
            myCommand.Parameters.AddWithValue("@TArmor", myTPlayerItems.theArmor.theName);
            myCommand.Parameters.AddWithValue("@TGrenades", string.Join(",", myTPlayerItems.theGrenadePreference.Select(aGrenade => aGrenade.theName)));
            myCommand.Parameters.AddWithValue("@TKit", null);

            myCommand.ExecuteNonQuery();
        }
    }

    public PlayerLoadout loadPlayerLoadout(ulong aSteamId) {
        using (MySqlConnection myConnection = createConnection()) {
            myConnection.Open();
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
                    PlayerItems myPlayerItems = createPlayerItems(pistolReader, aFieldPrefix, RoundType.Pistol);
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
                    PlayerItems myPlayerItems = createPlayerItems(fullBuyReader, aFieldPrefix, RoundType.FullBuy);
                    myLoadouts[RoundType.FullBuy] = myPlayerItems;
                }
            }
        }

        return myLoadouts;
    }

    private PlayerItems createPlayerItems(MySqlDataReader aSqlReader, string aFieldPrefix, RoundType aRoundType) {
        LoadoutItem myPrimaryWeapon = aRoundType == RoundType.Pistol ? theLoadoutFactory.getLoadoutItem("No Weapon") : theLoadoutFactory.getLoadoutItem(aSqlReader[$"{aFieldPrefix}_primary"] as string);

        bool? myKitEnabled = false;
        try {
            myKitEnabled = aSqlReader.IsDBNull(aSqlReader.GetOrdinal("ct_kit")) ? null : (bool?)aSqlReader["ct_kit"];
        } catch (IndexOutOfRangeException) {
            // Handle the case where 'ct_kit' column doesn't exist
        }

        return new PlayerItems(
            myPrimaryWeapon,
            theLoadoutFactory.getLoadoutItem(aSqlReader[$"{aFieldPrefix}_secondary"] as string),
            theLoadoutFactory.getLoadoutItem(aSqlReader[$"{aFieldPrefix}_armor"] as string),
            getGrenades(aSqlReader, $"{aFieldPrefix}_grenades"),
            myKitEnabled
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