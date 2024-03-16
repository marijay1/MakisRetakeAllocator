using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using MakisRetakeAllocator.Enums;
using MakisRetakeAllocator.Loadouts;

namespace CSPlus.Base.Entities;

public static class CCSPlayerControllerExtensions {
    private static PlayerLoadout? thePlayerLoadout;

    public static void setPlayerLoadout(this CCSPlayerController aPlayer, PlayerLoadout aPlayerLoadout) {
        thePlayerLoadout = aPlayerLoadout;
    }

    public static PlayerLoadout getPlayerLoadout(this CCSPlayerController aPlayer) {
        return thePlayerLoadout;
    }

    public static void removeArmor(this CCSPlayerController aPlayer) {
        if (aPlayer.PlayerPawn.Value == null || aPlayer.PlayerPawn.Value.ItemServices == null) {
            return;
        }

        CCSPlayer_ItemServices myItemServices = new CCSPlayer_ItemServices(aPlayer.PlayerPawn.Value.ItemServices.Handle);
        myItemServices.HasHelmet = false;
        myItemServices.HasHeavyArmor = false;
    }

    public static ulong getSteamId64(this CCSPlayerController aPlayer) {
        return aPlayer.AuthorizedSteamID?.SteamId64 ?? 0;
    }

    public static void allocateItems(this CCSPlayerController aPlayer, RoundType aRoundType) {
        if (!aPlayer.IsValid) {
            return;
        }

        CsItem? myPrimary = thePlayerLoadout!.getLoadouts(aPlayer.Team)[aRoundType].thePrimaryWeapon.theItem;
        CsItem mySecondary = (CsItem)thePlayerLoadout.getLoadouts(aPlayer.Team)[aRoundType].theSecondaryWeapon.theItem!;
        List<LoadoutItem> myGrenades = thePlayerLoadout.getLoadouts(aPlayer.Team)[aRoundType].theGrenades;
        CsItem? myArmor = thePlayerLoadout.getLoadouts(aPlayer.Team)[aRoundType].theArmor.theItem;
        bool myIsKitEnabled = thePlayerLoadout.getLoadouts(aPlayer.Team)[aRoundType].theIsKitEnabled;

        if (myPrimary != null) {
            aPlayer.GiveNamedItem((CsItem)myPrimary);
        }

        aPlayer.GiveNamedItem(mySecondary);

        if (myGrenades.Count != 0) {
            foreach (LoadoutItem myGrenade in myGrenades) {
                aPlayer.GiveNamedItem((CsItem)myGrenade.theItem!);
            }
        }

        if (myArmor != null) {
            aPlayer.GiveNamedItem((CsItem)myArmor);
        }

        if (myIsKitEnabled) {
            CCSPlayer_ItemServices myItemServices = new CCSPlayer_ItemServices(aPlayer.PlayerPawn.Value!.ItemServices!.Handle) {
                HasDefuser = true
            };
        }
    }
}