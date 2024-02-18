using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using CSPlus.Base.Entities;
using MakisRetakeAllocator.Configs;
using MakisRetakeAllocator.Enums;
using MakisRetakeAllocator.Loadouts;
using static MakisRetakeAllocator.Loadouts.PlayerLoadout;

namespace MakisRetakeAllocator;

public class GunMenu {
    private static readonly int SECONDS_TO_TIMEOUT = 30;

    private int theCurrentMoney;
    private CCSPlayerController thePlayer;
    private CsTeam theTeam;
    private RoundType? theRoundType;

    private LoadoutFactory theLoadoutFactory;
    private RetakesConfig theConfig;

    public GunMenu(CCSPlayerController aPlayer, CsTeam aTeam, LoadoutFactory aLoadoutFactory, MakisConfig aConfig) {
        thePlayer = aPlayer;
        theTeam = aTeam;
        theLoadoutFactory = aLoadoutFactory;
        theConfig = aConfig.theRetakesConfig;
        theCurrentMoney = 0;
    }

    private void updateMoney() {
        if (!theRoundType.HasValue) {
            throw new InvalidOperationException("Round type has not been selected.");
        }

        int myStartingMoney = theTeam == CsTeam.Terrorist ? theConfig.theTerroristStartingMoney : theConfig.theCounterTerroristStartingMoney;

        PlayerItems myPlayerLoadout = thePlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value];

        int myPrimaryCost = myPlayerLoadout.thePrimaryWeapon?.theCost ?? 0;
        int mySecondaryCost = myPlayerLoadout.theSecondaryWeapon.theCost;
        int myArmorCost = myPlayerLoadout.theArmor.theCost;
        int myGrenadeCost = myPlayerLoadout.theGrenadePreference.Sum(grenade => grenade.theCost);

        int myPlayerLoadoutCost = myPrimaryCost + mySecondaryCost + myArmorCost + myGrenadeCost;
        int myFinalMoney = myStartingMoney - myPlayerLoadoutCost;

        thePlayer.InGameMoneyServices!.Account = myFinalMoney;
    }

    public void openMenu() {
        ChatMenu myChatMenu = new ChatMenu("Select your Round type:");

        myChatMenu.AddMenuOption("Full Buy", onRoundTypeSelect);
        myChatMenu.AddMenuOption("Pistol", onRoundTypeSelect);
        myChatMenu.AddMenuOption("", null, true);
        myChatMenu.AddMenuOption("", null, true);
        myChatMenu.AddMenuOption("", null, true);
        myChatMenu.AddMenuOption("", null, true);
        myChatMenu.AddMenuOption("", null, true);
        myChatMenu.AddMenuOption("", null, true);
        myChatMenu.AddMenuOption("", null, true);
        myChatMenu.AddMenuOption("Exit", onMenuExit);

        MenuManager.OpenChatMenu(thePlayer, myChatMenu);
    }

    private void onRoundTypeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myItemTypeString = anOption.Text;
        switch (myItemTypeString) {
            case "Full Buy":
                openItemTypeMenu();
                theRoundType = RoundType.FullBuy;
                updateMoney();
                break;

            case "Pistol":
                openItemTypeMenu();
                theRoundType = RoundType.Pistol;
                updateMoney();
                break;

            default:
                onMenuExit(aPlayer, anOption);
                break;
        }
    }

    public void openItemTypeMenu() {
        if (!theRoundType.HasValue) {
            throw new InvalidOperationException("Round type has not been selected.");
        }

        string myKitIsEnabledString = thePlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value].theIsKitEnabled ? "Enabled" : "Disabled";

        ChatMenu myChatMenu = new ChatMenu("Select your Item type:");

        myChatMenu.AddMenuOption("Primary", onItemTypeSelect, theRoundType.Equals(RoundType.FullBuy) ? false : true);

        myChatMenu.AddMenuOption("Secondary", onItemTypeSelect);
        myChatMenu.AddMenuOption("Armor", onItemTypeSelect);
        myChatMenu.AddMenuOption("Grenade", onItemTypeSelect);

        myChatMenu.AddMenuOption($"Kit: {myKitIsEnabledString}", onItemTypeSelect, theTeam.Equals(CsTeam.CounterTerrorist) ? false : true);

        myChatMenu.AddMenuOption("", null, true);
        myChatMenu.AddMenuOption("Randomize Loadout", onRandomizeSelect);
        myChatMenu.AddMenuOption("", null, true);
        myChatMenu.AddMenuOption("Back", onBackToBeginningSelect);

        myChatMenu.AddMenuOption("Exit", onMenuExit);

        MenuManager.OpenChatMenu(thePlayer, myChatMenu);
    }

    private void onBackToBeginningSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        openMenu();
    }

    private void onRandomizeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        //Randomize Loadout
    }

    private void onItemTypeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myItemTypeString = anOption.Text;

        switch (myItemTypeString) {
            case "Primary":
                openPrimaryMenu();
                break;

            case "Secondary":
                openSecondaryMenu();
                break;

            case "Armor":
                openArmorMenu();
                break;

            case "Grenade":
                openGrenadeMenu();
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
        updateMoney();
    }

    private void openPrimaryMenu() {
        ChatMenu myChatMenu = new ChatMenu("Select your Primary:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(aWeapon => (aWeapon.theCsTeam == theTeam || aWeapon.theCsTeam == CsTeam.None) && aWeapon.theItemType == ItemType.Primary)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onPrimarySelect);
        }
        MenuManager.OpenChatMenu(thePlayer, myChatMenu);
    }

    private void onSecondarySelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myWeaponNameString = anOption.Text;
        PlayerItems myPlayerItems = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value];
        myPlayerItems.theSecondaryWeapon = theLoadoutFactory.getLoadoutItem(myWeaponNameString);
        updateMoney();
    }

    private void openSecondaryMenu() {
        ChatMenu myChatMenu = new ChatMenu("Select your Primary:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(aWeapon => (aWeapon.theCsTeam == theTeam || aWeapon.theCsTeam == CsTeam.None) && aWeapon.theItemType == ItemType.Secondary)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onSecondarySelect);
        }
        MenuManager.OpenChatMenu(thePlayer, myChatMenu);
    }

    private void onArmorSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myWeaponNameString = anOption.Text;
        PlayerItems myPlayerItems = aPlayer.getPlayerLoadout().getLoadouts(theTeam)[theRoundType.Value];
        myPlayerItems.theSecondaryWeapon = theLoadoutFactory.getLoadoutItem(myWeaponNameString);
        updateMoney();
    }

    private void openArmorMenu() {
        ChatMenu myChatMenu = new ChatMenu("Select your Primary:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(aWeapon => aWeapon.theItemType == ItemType.Armor)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onPrimarySelect);
        }
        MenuManager.OpenChatMenu(thePlayer, myChatMenu);
    }

    private void onGrenadeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
    }

    private void openGrenadeMenu() {
    }

    private void menuTimeout() {
    }

    private void onMenuExit(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        //closeMenu
    }
}