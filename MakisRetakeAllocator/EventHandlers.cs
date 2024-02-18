using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using CSPlus.Base.Entities;
using MakisRetakeAllocator.Enums;
using MakisRetakeAllocator.Loadouts;

namespace MakisRetakeAllocator;

public partial class MakisRetakeAllocator {

    public HookResult OnItemPurchase(EventItemPurchase @event, GameEventInfo anInfo) {
        return HookResult.Continue;
    }

    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo anInfo) {
        return HookResult.Continue;
    }

    private HookResult OnPlayerConnect(EventPlayerConnectFull @event, GameEventInfo anInfo) {
        CCSPlayerController myPlayer = @event.Userid;
        ulong mySteamId = myPlayer.SteamID;
        PlayerLoadout myPlayerLoadout = theDataContext.loadPlayerLoadout(mySteamId);
        if (!myPlayerLoadout.getLoadouts(CsTeam.CounterTerrorist).Any() || !myPlayerLoadout.getLoadouts(CsTeam.Terrorist).Any()) {
            myPlayerLoadout = theLoadoutFactory.CreateDefaultLoadout(myPlayer);
        }
        myPlayer.setPlayerLoadout(myPlayerLoadout);
        return HookResult.Continue;
    }
}