using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using CSPlus.Base.Entities;
using MakisRetakeAllocator.Enums;
using MakisRetakeAllocator.Loadouts;

namespace MakisRetakeAllocator;

public partial class MakisRetakeAllocator {

    [GameEventHandler]
    public HookResult OnItemPurchase(EventItemPurchase @event, GameEventInfo anInfo) {
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo anInfo) {
        if (Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules!.WarmupPeriod) {
            return HookResult.Continue;
        }

        CCSPlayerController myPlayer = @event.Userid;
        if (!myPlayer.IsValid) {
            return HookResult.Continue;
        }
        myPlayer.removeArmor();
        myPlayer.RemoveWeapons();

        myPlayer.allocateItems(theRoundType);
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerConnect(EventPlayerConnectFull @event, GameEventInfo anInfo) {
        CCSPlayerController myPlayer = @event.Userid;
        ulong mySteamId = myPlayer.getSteamId64();
        PlayerLoadout myPlayerLoadout;
        Task.Run(async () => {
            myPlayerLoadout = await thePlayerLoadoutContext.getLoadoutAsync(mySteamId);
            if (!myPlayerLoadout.getLoadouts(CsTeam.CounterTerrorist).Any() || !myPlayerLoadout.getLoadouts(CsTeam.Terrorist).Any()) {
                myPlayerLoadout = theLoadoutFactory.CreateDefaultLoadout(myPlayer);
            }
            myPlayer.setPlayerLoadout(myPlayerLoadout);
        });

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo anInfo) {
        CCSPlayerController myPlayer = @event.Userid;
        ulong mySteamId = myPlayer.getSteamId64();
        Task.Run(async () => await thePlayerLoadoutContext.upsertLoadoutAsync(mySteamId, myPlayer.getPlayerLoadout()));

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo anInfo) {
        theCurrentRound++;

        theRoundType = theCurrentRound > Config.theRetakesConfig.theNumPistolRounds ? RoundType.FullBuy : RoundType.Pistol;

        return HookResult.Continue;
    }

    private void OnMapStart(string aMapName) {
        theCurrentRound = 0;
    }
}