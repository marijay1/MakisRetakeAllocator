using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using CSPlus.Base.Entities;
using MakisRetakeAllocator.Configs;
using MakisRetakeAllocator.Database;
using MakisRetakeAllocator.Enums;
using MakisRetakeAllocator.Loadouts;
using static MakisRetakeAllocator.Loadouts.PlayerLoadout;
using static MakisRetakeAllocator.MakisRetakeAllocator;

namespace MakisRetakeAllocator.Menus;

public class GunMenu {
    private int theCurrentMoney;
    private CsTeam theTeam;
    private RoundType? theRoundType;

    private LoadoutFactory theLoadoutFactory;
    private RetakesConfig theRetakesConfig;
    private DataContext theDataContext;

    public GunMenu(CCSPlayerController aPlayer, CsTeam aTeam, LoadoutFactory aLoadoutFactory, MakisConfig aConfig, DataContext aDataContext) {
        theTeam = aTeam;
        theLoadoutFactory = aLoadoutFactory;
        theRetakesConfig = aConfig.theRetakesConfig;
        theDataContext = aDataContext;
        theCurrentMoney = 0;

        openMenu(aPlayer);
    }

    private void updateMoney(CCSPlayerController aPlayer) {
        if (!theRoundType.HasValue) {
            throw new InvalidOperationException("Round type has not been selected.");
        }

        int myStartingMoney = theTeam == CsTeam.Terrorist ? theRetakesConfig.theTerroristStartingMoney : theRetakesConfig.theCounterTerroristStartingMoney;

        PlayerItems myPlayerLoadout = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value];

        int myPrimaryCost = myPlayerLoadout.thePrimaryWeapon?.theCost ?? 0;
        int mySecondaryCost = myPlayerLoadout.theSecondaryWeapon.theCost;
        int myArmorCost = myPlayerLoadout.theArmor.theCost;
        int myGrenadeCost = myPlayerLoadout.theGrenades.Sum(grenade => grenade.theCost);

        int myPlayerLoadoutCost = myPrimaryCost + mySecondaryCost + myArmorCost + myGrenadeCost;
        int myFinalMoney = myStartingMoney - myPlayerLoadoutCost;

        aPlayer.InGameMoneyServices!.Account = myFinalMoney;
    }

    private void openMenu(CCSPlayerController aPlayer) {
        CenterHtmlMenu myChatMenu = new CenterHtmlMenu("Select your Round type:");

        myChatMenu.AddMenuOption("Full Buy", onRoundTypeSelect);
        myChatMenu.AddMenuOption("Pistol", onRoundTypeSelect);

        MenuManager.OpenCenterHtmlMenu(thePlugin, aPlayer, myChatMenu);
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
        if (!theRoundType.HasValue) {
            throw new InvalidOperationException("Round type has not been selected.");
        }

        string myKitIsEnabledString = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value].theIsKitEnabled ? "Enabled" : "Disabled";

        CenterHtmlMenu myChatMenu = new CenterHtmlMenu("Select your Item type:");

        myChatMenu.AddMenuOption("Primary", onItemTypeSelect, theRoundType.Equals(RoundType.FullBuy) ? false : true);

        myChatMenu.AddMenuOption("Secondary", onItemTypeSelect);
        myChatMenu.AddMenuOption("Armor", onItemTypeSelect);
        myChatMenu.AddMenuOption("Grenades", onItemTypeSelect);

        myChatMenu.AddMenuOption($"Kit: {myKitIsEnabledString}", onItemTypeSelect, theTeam.Equals(CsTeam.CounterTerrorist) ? false : true);

        myChatMenu.AddMenuOption("Randomize Loadout", onRandomizeSelect, true);

        MenuManager.OpenCenterHtmlMenu(thePlugin, aPlayer, myChatMenu);
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
        PlayerItems myPlayerLoadout = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value];
        myPlayerLoadout.theIsKitEnabled = !myPlayerLoadout.theIsKitEnabled;
        openItemTypeMenu(aPlayer);
    }

    private void onPrimarySelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myWeaponNameString = anOption.Text;
        PlayerItems myPlayerItems = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value];
        myPlayerItems.thePrimaryWeapon = theLoadoutFactory.getLoadoutItem(myWeaponNameString);
        openItemTypeMenu(aPlayer);
        updateMoney(aPlayer);
    }

    private void openPrimaryMenu(CCSPlayerController aPlayer) {
        CenterHtmlMenu myChatMenu = new CenterHtmlMenu("Select your Primary:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(aWeapon => (aWeapon.theCsTeam == theTeam || aWeapon.theCsTeam == CsTeam.None) && aWeapon.theItemType == ItemType.Primary && aWeapon.theIsInUse)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onPrimarySelect);
        }
        MenuManager.OpenCenterHtmlMenu(thePlugin, aPlayer, myChatMenu);
    }

    private void onSecondarySelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myWeaponNameString = anOption.Text;
        PlayerItems myPlayerItems = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value];
        myPlayerItems.theSecondaryWeapon = theLoadoutFactory.getLoadoutItem(myWeaponNameString);
        openItemTypeMenu(aPlayer);
        updateMoney(aPlayer);
    }

    private void openSecondaryMenu(CCSPlayerController aPlayer) {
        CenterHtmlMenu myChatMenu = new CenterHtmlMenu("Select your Secondary:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(aWeapon => (aWeapon.theCsTeam == theTeam || aWeapon.theCsTeam == CsTeam.None) && aWeapon.theItemType == ItemType.Secondary && aWeapon.theIsInUse)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onSecondarySelect);
        }
        MenuManager.OpenCenterHtmlMenu(thePlugin, aPlayer, myChatMenu);
    }

    private void onArmorSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myArmorNameString = anOption.Text;
        PlayerItems myPlayerItems = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value];
        myPlayerItems.theSecondaryWeapon = theLoadoutFactory.getLoadoutItem(myArmorNameString);
        openItemTypeMenu(aPlayer);
        updateMoney(aPlayer);
    }

    private void openArmorMenu(CCSPlayerController aPlayer) {
        CenterHtmlMenu myChatMenu = new CenterHtmlMenu("Select your Armor:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(anArmor => anArmor.theItemType == ItemType.Armor && anArmor.theIsInUse)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onArmorSelect);
        }
        MenuManager.OpenCenterHtmlMenu(thePlugin, aPlayer, myChatMenu);
    }

    private void onGrenadeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myGrenadeNameString = anOption.Text;
        PlayerItems myPlayerItems = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value];
        if (myPlayerItems.theGrenades.Count == 4) {
            aPlayer.PrintToChat("You already have 4 grenades. Please clear your grenades before picking another one.");
            openGrenadeMenu(aPlayer);
            return;
        }
        myPlayerItems.theGrenades.Add(theLoadoutFactory.getLoadoutItem(myGrenadeNameString));
        openGrenadeMenu(aPlayer);
        updateMoney(aPlayer);
    }

    private void onGrenadeWipeSelect(CCSPlayerController aPlayer, ChatMenuOption aOption) {
        PlayerItems myPlayerItems = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value];
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
        MenuManager.OpenCenterHtmlMenu(thePlugin, aPlayer, myChatMenu);
    }

    private void onRandomizeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        // TODO
        // Randomize Loadout
    }

    private void menuTimeout() {
        // TODO
        // Close menu after certain amount of time
    }

    private void onMenuExit(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        theDataContext.insertPlayerLoadout(aPlayer.getPlayerLoadout());
    }
}