using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using CSPlus.Base.Entities;
using MakisRetakeAllocator.Configs;
using MakisRetakeAllocator.Enums;
using MakisRetakeAllocator.Loadouts;
using static MakisRetakeAllocator.Loadouts.PlayerLoadout;
using static MakisRetakeAllocator.MakisRetakeAllocator;

namespace MakisRetakeAllocator;

public class GunMenu {
    private static readonly int SECONDS_TO_TIMEOUT = 30;

    private int theCurrentMoney;
    private CsTeam theTeam;
    private RoundType? theRoundType;

    private LoadoutFactory theLoadoutFactory;
    private RetakesConfig theConfig;

    public GunMenu(CCSPlayerController aPlayer, CsTeam aTeam, LoadoutFactory aLoadoutFactory, MakisConfig aConfig) {
        theTeam = aTeam;
        theLoadoutFactory = aLoadoutFactory;
        theConfig = aConfig.theRetakesConfig;
        theCurrentMoney = 0;

        openMenu(aPlayer);
    }

    private void updateMoney(CCSPlayerController aPlayer) {
        if (!theRoundType.HasValue) {
            throw new InvalidOperationException("Round type has not been selected.");
        }

        int myStartingMoney = theTeam == CsTeam.Terrorist ? theConfig.theTerroristStartingMoney : theConfig.theCounterTerroristStartingMoney;

        PlayerItems myPlayerLoadout = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value];

        int myPrimaryCost = myPlayerLoadout.thePrimaryWeapon?.theCost ?? 0;
        int mySecondaryCost = myPlayerLoadout.theSecondaryWeapon.theCost;
        int myArmorCost = myPlayerLoadout.theArmor.theCost;
        int myGrenadeCost = myPlayerLoadout.theGrenadePreference.Sum(grenade => grenade.theCost);

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
        myChatMenu.AddMenuOption("Grenade", onItemTypeSelect);

        myChatMenu.AddMenuOption($"Kit: {myKitIsEnabledString}", onItemTypeSelect, theTeam.Equals(CsTeam.CounterTerrorist) ? false : true);

        myChatMenu.AddMenuOption("Randomize Loadout", onRandomizeSelect);

        MenuManager.OpenCenterHtmlMenu(thePlugin, aPlayer, myChatMenu);
    }

    private void onBackToBeginningSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        openMenu(aPlayer);
    }

    private void onRandomizeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        //Randomize Loadout
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

            case "Grenade":
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
    }

    private void onPrimarySelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myWeaponNameString = anOption.Text;
        PlayerItems myPlayerItems = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value];
        myPlayerItems.thePrimaryWeapon = theLoadoutFactory.getLoadoutItem(myWeaponNameString);
        updateMoney(aPlayer);
    }

    private void openPrimaryMenu(CCSPlayerController aPlayer) {
        CenterHtmlMenu myChatMenu = new CenterHtmlMenu("Select your Primary:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(aWeapon => (aWeapon.theCsTeam == theTeam || aWeapon.theCsTeam == CsTeam.None) && aWeapon.theItemType == ItemType.Primary)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onPrimarySelect);
        }
        MenuManager.OpenCenterHtmlMenu(thePlugin, aPlayer, myChatMenu);
    }

    private void onSecondarySelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myWeaponNameString = anOption.Text;
        PlayerItems myPlayerItems = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value];
        myPlayerItems.theSecondaryWeapon = theLoadoutFactory.getLoadoutItem(myWeaponNameString);
        updateMoney(aPlayer);
    }

    private void openSecondaryMenu(CCSPlayerController aPlayer) {
        CenterHtmlMenu myChatMenu = new CenterHtmlMenu("Select your Primary:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(aWeapon => (aWeapon.theCsTeam == theTeam || aWeapon.theCsTeam == CsTeam.None) && aWeapon.theItemType == ItemType.Secondary)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onSecondarySelect);
        }
        MenuManager.OpenCenterHtmlMenu(thePlugin, aPlayer, myChatMenu);
    }

    private void onArmorSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myWeaponNameString = anOption.Text;
        PlayerItems myPlayerItems = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value];
        myPlayerItems.theSecondaryWeapon = theLoadoutFactory.getLoadoutItem(myWeaponNameString);
        updateMoney(aPlayer);
    }

    private void openArmorMenu(CCSPlayerController aPlayer) {
        CenterHtmlMenu myChatMenu = new CenterHtmlMenu("Select your Primary:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(aWeapon => aWeapon.theItemType == ItemType.Armor)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onPrimarySelect);
        }
        MenuManager.OpenCenterHtmlMenu(thePlugin, aPlayer, myChatMenu);
    }

    private void onGrenadeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
    }

    private void openGrenadeMenu(CCSPlayerController aPlayer) {
    }

    private void menuTimeout() {
    }

    private void onMenuExit(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        //closeMenu
    }
}