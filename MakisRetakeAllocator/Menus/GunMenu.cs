using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using CSPlus.Base.Entities;
using MakisRetakeAllocator.Configs;
using MakisRetakeAllocator.Database;
using MakisRetakeAllocator.Enums;
using MakisRetakeAllocator.Loadouts;
using System.Numerics;
using static MakisRetakeAllocator.Loadouts.PlayerLoadout;
using static MakisRetakeAllocator.MakisRetakeAllocator;

namespace MakisRetakeAllocator.Menus;

public class GunMenu {
    private CsTeam theTeam;
    private RoundType theRoundType;

    private LoadoutFactory theLoadoutFactory;
    private RetakesConfig theRetakesConfig;
    private DataContext theDataContext;

    public GunMenu(CCSPlayerController aPlayer, CsTeam aTeam, LoadoutFactory aLoadoutFactory, MakisConfig aConfig, DataContext aDataContext) {
        theTeam = aTeam;
        theLoadoutFactory = aLoadoutFactory;
        theRetakesConfig = aConfig.theRetakesConfig;
        theDataContext = aDataContext;

        openMenu(aPlayer);
    }

    private void openMenu(CCSPlayerController aPlayer) {
        CenterHtmlMenu myChatMenu = new CenterHtmlMenu("Select your Round type:");

        myChatMenu.AddMenuOption("Full Buy", onRoundTypeSelect);
        myChatMenu.AddMenuOption("Pistol", onRoundTypeSelect);

        MenuManager.OpenCenterHtmlMenu(Plugin, aPlayer, myChatMenu);
    }

    private void onRoundTypeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myItemTypeString = anOption.Text;

        switch (myItemTypeString) {
            case "Full Buy":
                theRoundType = RoundType.FullBuy;
                openItemTypeMenu(aPlayer);
                updateMoney(aPlayer);
                break;

            case "Pistol":
                theRoundType = RoundType.Pistol;
                openItemTypeMenu(aPlayer);
                updateMoney(aPlayer);
                break;

            default:
                onMenuExit(aPlayer, anOption);
                break;
        }
    }

    public void openItemTypeMenu(CCSPlayerController aPlayer) {
        string myKitIsEnabledString = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType].theIsKitEnabled ? "Enabled" : "Disabled";

        CenterHtmlMenu myChatMenu = new CenterHtmlMenu("Select your Item type:");

        myChatMenu.AddMenuOption("Primary", onItemTypeSelect, theRoundType.Equals(RoundType.FullBuy) ? false : true);

        myChatMenu.AddMenuOption("Secondary", onItemTypeSelect);
        myChatMenu.AddMenuOption("Armor", onItemTypeSelect);
        myChatMenu.AddMenuOption("Grenades", onItemTypeSelect);

        myChatMenu.AddMenuOption($"Kit: {myKitIsEnabledString}", onItemTypeSelect, theTeam.Equals(CsTeam.CounterTerrorist) ? false : true);

        myChatMenu.AddMenuOption("Randomize Loadout", onRandomizeSelect, true);

        MenuManager.OpenCenterHtmlMenu(Plugin, aPlayer, myChatMenu);
    }

    private void onItemTypeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myItemTypeString = anOption.Text;

        switch (myItemTypeString) {
            case "Primary":
                openPrimaryMenu(aPlayer);
                break;

            case "Secondary":
                openSecondaryMenu(aPlayer);
                break;

            case "Armor":
                openArmorMenu(aPlayer);
                break;

            case "Grenades":
                openGrenadeMenu(aPlayer);
                break;

            case string myString when myString.StartsWith("Kit"):
                onKitSelect(aPlayer, anOption);
                break;

            default:
                onMenuExit(aPlayer, anOption);
                break;
        }
    }

    private void onKitSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        PlayerItems myPlayerLoadout = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType];
        myPlayerLoadout.theIsKitEnabled = !myPlayerLoadout.theIsKitEnabled;
        openItemTypeMenu(aPlayer);
    }

    private void onPrimarySelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myWeaponNameString = anOption.Text;
        LoadoutItem myLoadoutItem = theLoadoutFactory.getLoadoutItem(myWeaponNameString);
        PlayerItems myPlayerItems = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType];
        int myOldWeaponCost;

        if (aPlayer.getPlayerLoadout().canAddWeapon(myLoadoutItem, getStartingMoney(), theTeam, theRoundType, out myOldWeaponCost)) {
            myPlayerItems.thePrimaryWeapon = myLoadoutItem;
            aPlayer.PrintToChat($"{MakisRetakeAllocator.MessagePrefix} {MakisRetakeAllocator.Plugin.Localizer["mr.allocator.menu.guns.WeaponSelected", myWeaponNameString]}");
            openItemTypeMenu(aPlayer);
            updateMoney(aPlayer);
            return;
        }

        aPlayer.PrintToChat($"{MakisRetakeAllocator.MessagePrefix} {MakisRetakeAllocator.Plugin.Localizer["mr.allocator.menu.guns.TooExpensive", getAmountNeeded(aPlayer, myLoadoutItem, myOldWeaponCost)]}");
        openItemTypeMenu(aPlayer);
    }

    private void openPrimaryMenu(CCSPlayerController aPlayer) {
        CenterHtmlMenu myChatMenu = new CenterHtmlMenu("Select your Primary:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(aWeapon => (aWeapon.theCsTeam == theTeam || aWeapon.theCsTeam == CsTeam.None) && aWeapon.theItemType == ItemType.Primary && aWeapon.theIsInUse)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onPrimarySelect);
        }
        MenuManager.OpenCenterHtmlMenu(Plugin, aPlayer, myChatMenu);
    }

    private void onSecondarySelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myWeaponNameString = anOption.Text;
        LoadoutItem myLoadoutItem = theLoadoutFactory.getLoadoutItem(myWeaponNameString);
        PlayerItems myPlayerItems = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType];
        int myOldWeaponCost;

        if (aPlayer.getPlayerLoadout().canAddWeapon(myLoadoutItem, getStartingMoney(), theTeam, theRoundType, out myOldWeaponCost)) {
            myPlayerItems.theSecondaryWeapon = myLoadoutItem;
            aPlayer.PrintToChat($"{MakisRetakeAllocator.MessagePrefix} {MakisRetakeAllocator.Plugin.Localizer["mr.allocator.menu.guns.WeaponSelected", myWeaponNameString]}");
            openItemTypeMenu(aPlayer);
            updateMoney(aPlayer);
            return;
        }

        aPlayer.PrintToChat($"{MakisRetakeAllocator.MessagePrefix} {MakisRetakeAllocator.Plugin.Localizer["mr.allocator.menu.guns.TooExpensive", getAmountNeeded(aPlayer, myLoadoutItem, myOldWeaponCost)]}");
        openItemTypeMenu(aPlayer);
    }

    private void openSecondaryMenu(CCSPlayerController aPlayer) {
        CenterHtmlMenu myChatMenu = new CenterHtmlMenu("Select your Secondary:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(aWeapon => (aWeapon.theCsTeam == theTeam || aWeapon.theCsTeam == CsTeam.None) && aWeapon.theItemType == ItemType.Secondary && aWeapon.theIsInUse)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onSecondarySelect);
        }
        MenuManager.OpenCenterHtmlMenu(Plugin, aPlayer, myChatMenu);
    }

    private void onArmorSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myWeaponNameString = anOption.Text;
        LoadoutItem myLoadoutItem = theLoadoutFactory.getLoadoutItem(myWeaponNameString);
        PlayerItems myPlayerItems = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType];
        int myOldWeaponCost;

        if (aPlayer.getPlayerLoadout().canAddWeapon(myLoadoutItem, getStartingMoney(), theTeam, theRoundType, out myOldWeaponCost)) {
            myPlayerItems.theArmor = myLoadoutItem;
            aPlayer.PrintToChat($"{MakisRetakeAllocator.MessagePrefix} {MakisRetakeAllocator.Plugin.Localizer["mr.allocator.menu.guns.WeaponSelected", myWeaponNameString]}");
            openItemTypeMenu(aPlayer);
            updateMoney(aPlayer);
            return;
        }

        aPlayer.PrintToChat($"{MakisRetakeAllocator.MessagePrefix} {MakisRetakeAllocator.Plugin.Localizer["mr.allocator.menu.guns.TooExpensive", getAmountNeeded(aPlayer, myLoadoutItem, myOldWeaponCost)]}");
        openItemTypeMenu(aPlayer);
    }

    private void openArmorMenu(CCSPlayerController aPlayer) {
        CenterHtmlMenu myChatMenu = new CenterHtmlMenu("Select your Armor:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(anArmor => anArmor.theItemType == ItemType.Armor && anArmor.theIsInUse)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onArmorSelect);
        }
        MenuManager.OpenCenterHtmlMenu(Plugin, aPlayer, myChatMenu);
    }

    private void onGrenadeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myGrenadeNameString = anOption.Text;
        LoadoutItem myLoadoutItem = theLoadoutFactory.getLoadoutItem(myGrenadeNameString);
        PlayerItems myPlayerItems = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType];
        int myOldWeaponCost;

        if (!aPlayer.getPlayerLoadout().CanAddGrenade(aPlayer, theLoadoutFactory, myLoadoutItem, theTeam, theRoundType)) {
            openItemTypeMenu(aPlayer);
            return;
        }

        if (aPlayer.getPlayerLoadout().canAddWeapon(myLoadoutItem, getStartingMoney(), theTeam, theRoundType, out myOldWeaponCost)) {
            myPlayerItems.theGrenades.Add(myLoadoutItem);
            aPlayer.PrintToChat($"{MakisRetakeAllocator.MessagePrefix} {MakisRetakeAllocator.Plugin.Localizer["mr.allocator.menu.guns.WeaponSelected", myGrenadeNameString]}");
            openItemTypeMenu(aPlayer);
            updateMoney(aPlayer);
            return;
        }

        aPlayer.PrintToChat($"{MakisRetakeAllocator.MessagePrefix} {MakisRetakeAllocator.Plugin.Localizer["mr.allocator.menu.guns.TooExpensive", getAmountNeeded(aPlayer, myLoadoutItem, myOldWeaponCost)]}");
        openItemTypeMenu(aPlayer);
    }

    private void onGrenadeWipeSelect(CCSPlayerController aPlayer, ChatMenuOption aOption) {
        PlayerItems myPlayerItems = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType];
        myPlayerItems.theGrenades.Clear();
        openGrenadeMenu(aPlayer);
        updateMoney(aPlayer);
    }

    private void openGrenadeMenu(CCSPlayerController aPlayer) {
        CenterHtmlMenu myChatMenu = new CenterHtmlMenu("Select your Grenades:");
        myChatMenu.AddMenuOption("Clear Grenades", onGrenadeWipeSelect);
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(aGrenade => (aGrenade.theCsTeam == theTeam || aGrenade.theCsTeam == CsTeam.None) && aGrenade.theItemType == ItemType.Grenade && aGrenade.theIsInUse)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onGrenadeSelect);
        }
        MenuManager.OpenCenterHtmlMenu(Plugin, aPlayer, myChatMenu);
    }

    private void onRandomizeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        // TODO
        // Randomize Loadout
    }

    private void onMenuExit(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        theDataContext.insertPlayerLoadout(aPlayer.getPlayerLoadout());
    }

    private void menuTimeout() {
        // TODO
        // Close menu after certain amount of time
    }

    private int getStartingMoney() {
        if (theRoundType == RoundType.Pistol) {
            //Cfg?
            return 800;
        }
        return theTeam == CsTeam.Terrorist ? theRetakesConfig.theTerroristStartingMoney : theRetakesConfig.theCounterTerroristStartingMoney;
    }

    private int getAmountNeeded(CCSPlayerController aPlayer, LoadoutItem aNewLoadoutItem, int anOldWeaponCost) {
        int myCurrentMoney = aPlayer.InGameMoneyServices!.Account;
        int myNewWeaponCost = aNewLoadoutItem.theCost;

        return Math.Abs(myCurrentMoney + anOldWeaponCost - myNewWeaponCost);
    }

    private void updateMoney(CCSPlayerController aPlayer) {
        aPlayer.InGameMoneyServices!.Account = getStartingMoney() - aPlayer.getPlayerLoadout().getLoadoutCost(theTeam, theRoundType);
        Utilities.SetStateChanged(aPlayer, "CCSPlayerController", "m_pInGameMoneyServices");
    }
}