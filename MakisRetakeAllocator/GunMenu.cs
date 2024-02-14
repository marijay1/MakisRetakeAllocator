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

    private LoadoutFactory theLoadoutFactory;
    private MakisConfig theConfig;

    public GunMenu(CCSPlayerController aPlayer, CsTeam aTeam, LoadoutFactory aLoadoutFactory, MakisConfig aConfig) {
        thePlayer = aPlayer;
        theTeam = aTeam;
        theLoadoutFactory = aLoadoutFactory;
        theConfig = aConfig;
        theCurrentMoney = 0;
    }

    private int setMoney(RoundType aRoundType) {
        int myStartingMoney = theTeam == CsTeam.Terrorist ? theConfig.theTerroristStartingMoney : theConfig.theCounterTerroristStartingMoney;
        PlayerItems myPlayerLoadout = thePlayer.getPlayerLoadout().getLoadouts(theTeam)[aRoundType];
        int myPrimaryCost = myPlayerLoadout.thePrimaryWeapon == null ? 0 : myPlayerLoadout.thePrimaryWeapon.theCost;
        int mySecondaryCost = myPlayerLoadout.theSecondaryWeapon.theCost;
        int myArmorCost = myPlayerLoadout.theArmor.theCost;

        int myPlayerLoadoutCost = myPrimaryCost + mySecondaryCost + myArmorCost;

        return -1;
    }

    public void openMenu() {
        ChatMenu myChatMenu = new ChatMenu("Select your Round type:");

        myChatMenu.AddMenuOption("Full Buy", onRoundTypeSelect);
        myChatMenu.AddMenuOption("Pistol", onRoundTypeSelect);
        myChatMenu.AddMenuOption("Exit", onMenuExit);
    }

    private void onRoundTypeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myItemTypeString = anOption.Text;
        switch (myItemTypeString) {
            case "Full Buy":
                openItemTypeMenu(RoundType.FullBuy);
                break;

            case "Pistol":
                openItemTypeMenu(RoundType.Pistol);
                break;

            default:
                onMenuExit(aPlayer, anOption);
                break;
        }
    }

    public void openItemTypeMenu(RoundType aRoundType) {
        ChatMenu myChatMenu = new ChatMenu("Select your Item type:");

        if (aRoundType == RoundType.FullBuy) {
            myChatMenu.AddMenuOption("Primary", onItemTypeSelect);
        }

        myChatMenu.AddMenuOption("Secondary", onItemTypeSelect);
        myChatMenu.AddMenuOption("Armor", onItemTypeSelect);
        myChatMenu.AddMenuOption("Grenade", onItemTypeSelect);

        if (theTeam == CsTeam.CounterTerrorist) {
            myChatMenu.AddMenuOption($"Kit: {false}", onItemTypeSelect);
        }

        myChatMenu.AddMenuOption("Exit", onMenuExit);
    }

    private void onItemTypeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        string myItemTypeString = anOption.Text;
        //int account = aPlayer.InGameMoneyServices!.Account;

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

            case "Temp Kit":
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
    }

    private void openPrimaryMenu() {
        ChatMenu myChatMenu = new ChatMenu("Select your Primary:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(aWeapon => (aWeapon.theCsTeam == theTeam || aWeapon.theCsTeam == CsTeam.None) && aWeapon.theItemType == ItemType.Primary)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onPrimarySelect);
        }
    }

    private void onSecondarySelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
    }

    private void openSecondaryMenu() {
        ChatMenu myChatMenu = new ChatMenu("Select your Primary:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(aWeapon => (aWeapon.theCsTeam == theTeam || aWeapon.theCsTeam == CsTeam.None) && aWeapon.theItemType == ItemType.Secondary)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onSecondarySelect);
        }
    }

    private void onArmorSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
    }

    private void openArmorMenu() {
        ChatMenu myChatMenu = new ChatMenu("Select your Primary:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(aWeapon => aWeapon.theItemType == ItemType.Armor)) {
            myChatMenu.AddMenuOption(myWeapon.theName, onPrimarySelect);
        }
    }

    private void onGrenadeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
    }

    private void openGrenadeMenu() {
        //TODO
    }

    private void menuTimeout() {
    }

    private void onMenuExit(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        //closeMenu
    }
}