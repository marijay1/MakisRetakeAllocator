using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using MakisRetakeAllocator.Enums;

namespace MakisRetakeAllocator.Loadouts;

public class PlayerLoadout {
    private readonly ulong theSteamId;

    private Dictionary<RoundType, PlayerItems> theCounterTerroristLoadouts;
    private Dictionary<RoundType, PlayerItems> theTerroristLoadouts;

    public PlayerLoadout(ulong aSteamId, Dictionary<RoundType, PlayerItems>? aCounterTerroristLoadouts, Dictionary<RoundType, PlayerItems>? aTerroristLoadouts) {
        theSteamId = aSteamId;
        theCounterTerroristLoadouts = aCounterTerroristLoadouts ?? new Dictionary<RoundType, PlayerItems>();
        theTerroristLoadouts = aTerroristLoadouts ?? new Dictionary<RoundType, PlayerItems>();
    }

    public ulong getSteamId() {
        return theSteamId;
    }

    public Dictionary<RoundType, PlayerItems> getLoadouts(CsTeam aTeam) {
        return aTeam == CsTeam.CounterTerrorist ? theCounterTerroristLoadouts : theTerroristLoadouts;
    }

    public int getLoadoutCost(CsTeam aTeam, RoundType aRoundType) {
        PlayerItems myPlayerItems = getLoadouts(aTeam)[aRoundType];

        int myPrimaryCost = myPlayerItems.thePrimaryWeapon?.theCost ?? 0;
        int mySecondaryCost = myPlayerItems.theSecondaryWeapon.theCost;
        int myArmorCost = myPlayerItems.theArmor.theCost;
        int myGrenadeCost = myPlayerItems.theGrenades.Sum(grenade => grenade.theCost);

        int myPlayerLoadoutCost = myPrimaryCost + mySecondaryCost + myArmorCost + myGrenadeCost;

        return myPlayerLoadoutCost;
    }

    public bool canAddWeapon(LoadoutItem aLoadoutItem, int aStartingMoney, CsTeam aTeam, RoundType aRoundType, out int anOldWeaponCost) {
        int myCurrentLoadoutCost = getLoadoutCost(aTeam, aRoundType);
        int myOldWeaponCost = 0;

        PlayerItems myPlayerItems = getLoadouts(aTeam)[aRoundType];
        switch (aLoadoutItem.theItemType) {
            case ItemType.Primary:
                myOldWeaponCost = myPlayerItems.thePrimaryWeapon?.theCost ?? 0;
                break;

            case ItemType.Secondary:
                myOldWeaponCost = myPlayerItems.theSecondaryWeapon?.theCost ?? 0;
                break;

            case ItemType.Armor:
                myOldWeaponCost = myPlayerItems.theArmor?.theCost ?? 0;
                break;

            case ItemType.Grenade:
                break;
        }

        int myNewLoadoutCost = (myCurrentLoadoutCost - myOldWeaponCost) + aLoadoutItem.theCost;
        anOldWeaponCost = myOldWeaponCost;

        return myNewLoadoutCost <= aStartingMoney;
    }

    public bool CanAddGrenade(CCSPlayerController aPlayer, LoadoutFactory aLoadoutFactory, LoadoutItem aLoadoutItem, CsTeam aTeam, RoundType aRoundType) {
        PlayerItems myPlayerItems = getLoadouts(aTeam)[aRoundType];

        if (myPlayerItems.theGrenades.Count == 4) {
            Console.WriteLine("Too many grenades!");
            aPlayer.PrintToChat($"{MakisRetakeAllocator.MessagePrefix} {MakisRetakeAllocator.Plugin.Localizer["mr.allocator.menu.guns.ExceededGrenades"]}");
            return false;
        }

        if (aLoadoutItem != aLoadoutFactory.getLoadoutItem(CsItem.Flashbang) && myPlayerItems.theGrenades.Contains(aLoadoutItem)) {
            Console.WriteLine("duplicate grenade!");
            aPlayer.PrintToChat($"{MakisRetakeAllocator.MessagePrefix} {MakisRetakeAllocator.Plugin.Localizer["mr.allocator.menu.guns.DuplicateGrenade", aLoadoutItem.theName]}");
            return false;
        }

        if (countFlashbangs(myPlayerItems.theGrenades) >= 2 && aLoadoutItem == aLoadoutFactory.getLoadoutItem(CsItem.Flashbang)) {
            Console.WriteLine("too many Flashbangs!");
            aPlayer.PrintToChat($"{MakisRetakeAllocator.MessagePrefix} {MakisRetakeAllocator.Plugin.Localizer["mr.allocator.menu.guns.ExceededFlashbangs"]}");
            return false;
        }

        return true;
    }

    private int countFlashbangs(List<LoadoutItem> aGrenades) {
        return aGrenades.Count(aGrenade => aGrenade.theItem == CsItem.Flashbang);
    }

    public class PlayerItems {
        public LoadoutItem thePrimaryWeapon { get; set; }
        public LoadoutItem theSecondaryWeapon { get; set; }
        public LoadoutItem theArmor { get; set; }
        public List<LoadoutItem> theGrenades { get; set; }
        public bool theIsKitEnabled { get; set; }

        public PlayerItems(LoadoutItem aPrimaryWeapon, LoadoutItem aSecondaryWeapon, LoadoutItem aArmor, List<LoadoutItem> aGrenades, bool? anIsKitEnabled) {
            thePrimaryWeapon = aPrimaryWeapon;
            theSecondaryWeapon = aSecondaryWeapon;
            theArmor = aArmor;
            theGrenades = aGrenades;
            theIsKitEnabled = anIsKitEnabled ?? false;
        }
    }
}