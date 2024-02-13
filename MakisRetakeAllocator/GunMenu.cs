using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using MakisRetakeAllocator.Loadouts;

namespace MakisRetakeAllocator;

public class GunMenu {
    private static readonly int SECONDS_TO_TIMEOUT = 30;

    private int theMoneyAvailable = -1;
    private CsTeam theTeam;

    private LoadoutFactory theLoadoutFactory;

    public GunMenu(LoadoutFactory aLoadoutFactory, CsTeam aTeam) {
        theTeam = aTeam;
        theLoadoutFactory = aLoadoutFactory;
    }

    public void openMenu() {
        ChatMenu myChatMenu = new ChatMenu("Select your item type:");

        myChatMenu.AddMenuOption("Primary", onPrimarySelect);
        myChatMenu.AddMenuOption("Secondary", onSecondarySelect);
        myChatMenu.AddMenuOption("Armor", onArmorSelect);
        myChatMenu.AddMenuOption("Grenade", onGrenadeSelect);

        if (theTeam == CsTeam.CounterTerrorist) {
            myChatMenu.AddMenuOption($"Kit: {false}", onKitSelect);
        }
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
                openGrenadePrefrenceMenu();
                break;

            case "Temp Kit":
                onKitSelect(aPlayer, anOption);
                break;

            default:
                onMenuExit(aPlayer, anOption);
                break;
        }
    }

    private void onMenuExit(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        //closeMenu
    }

    private void onKitSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        //Toggle kit boolean
        theMoneyAvailable -= 400;
    }

    private void onPrimarySelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
        openPrimaryMenu();
    }

    private void openPrimaryMenu() {
        ChatMenu myChatMenu = new ChatMenu("Select your Primary:");
        foreach (LoadoutItem myWeapon in theLoadoutFactory.LOADOUT_ITEMS.Where(aWeapon => aWeapon.theCsTeam == theTeam)) {
            //TODO
        }
    }

    private void onSecondarySelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
    }

    private void openSecondaryMenu() {
    }

    private void onArmorSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
    }

    private void openArmorMenu() {
    }

    private void onGrenadeSelect(CCSPlayerController aPlayer, ChatMenuOption anOption) {
    }

    private void openGrenadePrefrenceMenu() {
        //TODO
    }

    private void menuTimeout() {
    }
}