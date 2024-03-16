using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using MakisRetakeAllocator.Enums;
using MakisRetakeAllocator.Loadouts;
using System.Data;
using static MakisRetakeAllocator.Loadouts.PlayerLoadout;

namespace MakisRetakeAllocator.Database;

public class PlayerLoadoutContext {
    private SqlDataAccess theSqlDataAccess;
    private LoadoutFactory theLoadoutFactory;

    public PlayerLoadoutContext(SqlDataAccess aSqlDataAccess, LoadoutFactory aLoadoutFactory) {
        theSqlDataAccess = aSqlDataAccess;
        theLoadoutFactory = aLoadoutFactory;
        Task.Run(async () => await initTablesAsync());
    }

    public async Task<PlayerLoadout> getLoadoutAsync(ulong aSteamId) {
        Dictionary<RoundType, PlayerItems> myCounterTerroristLoadout = await getTeamLoadoutAsync(CsTeam.CounterTerrorist, aSteamId);
        Dictionary<RoundType, PlayerItems> myTerroristLoadout = await getTeamLoadoutAsync(CsTeam.Terrorist, aSteamId);

        return new PlayerLoadout(aSteamId, myCounterTerroristLoadout, myTerroristLoadout);
    }

    private async Task<Dictionary<RoundType, PlayerItems>> getTeamLoadoutAsync(CsTeam aTeam, ulong aSteamId) {
        Dictionary<RoundType, PlayerItems> myLoadout = new Dictionary<RoundType, PlayerItems>();

        foreach (RoundType myRoundType in Enum.GetValues(typeof(RoundType))) {
            PlayerItems myItems = await getPlayerItemsAsync(aTeam, myRoundType, aSteamId);
            myLoadout.Add(myRoundType, myItems);
        }

        return myLoadout;
    }

    private async Task<PlayerItems> getPlayerItemsAsync(CsTeam aTeam, RoundType aRoundType, ulong aSteamId) {
        string myResourceName = aRoundType.Equals(RoundType.Pistol) ? "p_GetUserItems" : "fb_GetUserItems";
        string mySql = theSqlDataAccess.readEmbeddedSqlProcedure(myResourceName);

        return await theSqlDataAccess.loadDataAsync<PlayerItems, dynamic, CsTeam, RoundType>(
            mySql,
            new { SteamId = aSteamId },
            MapToPlayerItems,
            aTeam,
            aRoundType);
    }

    private PlayerItems MapToPlayerItems(IDataReader myReader, CsTeam aTeam, RoundType aRoundType) {
        string myRowPrefix = aTeam.Equals(CsTeam.Terrorist) ? "t" : "ct";

        LoadoutItem myPrimaryWeapon = theLoadoutFactory.getLoadoutItem("No Weapon");
        if (aRoundType.Equals(RoundType.FullBuy)) {
            myPrimaryWeapon = theLoadoutFactory.getLoadoutItem(myReader[$"{myRowPrefix}_primary"].ToString());
        }

        LoadoutItem mySecondaryWeapon = theLoadoutFactory.getLoadoutItem(myReader[$"{myRowPrefix}_secondary"].ToString());
        LoadoutItem myArmor = theLoadoutFactory.getLoadoutItem(myReader[$"{myRowPrefix}_armor"].ToString());

        List<LoadoutItem> myGrenades = new List<LoadoutItem>();
        string[] myGrenadesString = myReader[$"{myRowPrefix}_grenades"].ToString().Split(',');
        foreach (string myGrenadeString in myGrenadesString) {
            myGrenades.Add(theLoadoutFactory.getLoadoutItem(myGrenadeString));
        }

        bool? myIsKitEnabled = Convert.ToBoolean(myReader[$"{myRowPrefix}_kit"]);

        return new PlayerItems(myPrimaryWeapon, mySecondaryWeapon, myArmor, myGrenades, myIsKitEnabled);
    }

    public async Task upsertLoadoutAsync(ulong aSteamId, PlayerLoadout aLoadout) {
        foreach (RoundType myRoundType in Enum.GetValues(typeof(RoundType))) {
            await upsertTeamLoadoutAsync(aSteamId, myRoundType, aLoadout);
        }
    }

    private async Task upsertTeamLoadoutAsync(ulong aSteamId, RoundType aRoundType, PlayerLoadout aLoadout) {
        string myResourceName = aRoundType.Equals(RoundType.Pistol) ? "p_UpsertUserItems" : "fb_UpsertUserItems";
        string mySql = theSqlDataAccess.readEmbeddedSqlProcedure(myResourceName);

        PlayerItems myCounterTerroristItems = aLoadout.getLoadouts(CsTeam.CounterTerrorist)[aRoundType];
        PlayerItems myTerroristItems = aLoadout.getLoadouts(CsTeam.Terrorist)[aRoundType];

        var myParameters = new {
            SteamId = aSteamId,
            CTPrimary = aRoundType.Equals(RoundType.FullBuy) ? myCounterTerroristItems.thePrimaryWeapon.theName : null,
            CTSecondary = myCounterTerroristItems.theSecondaryWeapon.theName,
            CTArmor = myCounterTerroristItems.theArmor.theName,
            CTGrenades = string.Join(",", myCounterTerroristItems.theGrenades.Select(aGrenade => aGrenade.theName)),
            CTKit = myCounterTerroristItems.theIsKitEnabled,
            TPrimary = aRoundType.Equals(RoundType.FullBuy) ? myTerroristItems.thePrimaryWeapon.theName : null,
            TSecondary = myTerroristItems.theSecondaryWeapon.theName,
            TArmor = myTerroristItems.theArmor.theName,
            TGrenades = string.Join(",", myTerroristItems.theGrenades.Select(aGrenade => aGrenade.theName)),
            TKit = myTerroristItems.theIsKitEnabled
        };

        await theSqlDataAccess.saveDataAsync(mySql, myParameters);
    }

    private async Task initTablesAsync() {
        string myPistolResourceName = "p_InitTable.sql";
        string myPistolSql = theSqlDataAccess.readEmbeddedSqlProcedure(myPistolResourceName);
        await theSqlDataAccess.executeSql(myPistolSql);

        string myFullbuyResourceName = "fb_InitTable.sql";
        string myFullbuySql = theSqlDataAccess.readEmbeddedSqlProcedure(myFullbuyResourceName);
        await theSqlDataAccess.executeSql(myFullbuySql);
    }
}