using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using MakisRetakeAllocator.Enums;
using static MakisRetakeAllocator.Loadouts.PlayerLoadout;

namespace MakisRetakeAllocator.Loadouts;

public class LoadoutFactory {

    public PlayerLoadout CreateDefaultLoadout(CCSPlayerController aPlayer) {
        PlayerLoadout myPlayerLoadout = new PlayerLoadout(aPlayer.SteamID, null, null);

        Dictionary<RoundType, PlayerItems> myCounterTerroristLoadout = myPlayerLoadout.getLoadouts(CsTeam.CounterTerrorist);
        Dictionary<RoundType, PlayerItems> myTerroristLoadout = myPlayerLoadout.getLoadouts(CsTeam.Terrorist);

        myCounterTerroristLoadout.Add(RoundType.Pistol, new PlayerItems(
            getLoadoutItem("No Weapon"),
            getLoadoutItem(CsItem.USP),
            getLoadoutItem(CsItem.Kevlar),
            new List<LoadoutItem>(),
            false
            ));

        myCounterTerroristLoadout.Add(RoundType.FullBuy, new PlayerItems(
            getLoadoutItem(CsItem.M4A1S),
            getLoadoutItem(CsItem.USP),
            getLoadoutItem(CsItem.KevlarHelmet),
            new List<LoadoutItem>(),
            true
            ));

        myTerroristLoadout.Add(RoundType.Pistol, new PlayerItems(
            getLoadoutItem("No Weapon"),
            getLoadoutItem(CsItem.Glock),
            getLoadoutItem(CsItem.Kevlar),
            new List<LoadoutItem>(),
            false
            ));

        myTerroristLoadout.Add(RoundType.FullBuy, new PlayerItems(
            getLoadoutItem(CsItem.AK47),
            getLoadoutItem(CsItem.Glock),
            getLoadoutItem(CsItem.KevlarHelmet),
            new List<LoadoutItem>(),
            false
            ));

        return new PlayerLoadout(aPlayer.SteamID, myCounterTerroristLoadout, myTerroristLoadout);
    }

    public LoadoutItem getLoadoutItem(CsItem aCsItem) {
        return LOADOUT_ITEMS.FirstOrDefault(aWeapon => aWeapon.theItem.Equals(aCsItem))!;
    }

    public LoadoutItem getLoadoutItem(String aWeaponString) {
        return LOADOUT_ITEMS.FirstOrDefault(aWeapon => aWeapon.theName.Equals(aWeaponString))!;
    }

    public List<LoadoutItem> LOADOUT_ITEMS { get; } = new List<LoadoutItem>() {
        //Primarys
        new LoadoutItem("No Weapon", 0, null, ItemType.Primary, CsTeam.None, true),
        //Snipers
        new LoadoutItem("Scout", 1700, CsItem.SSG08, ItemType.Primary, CsTeam.None, true),
        new LoadoutItem("AWP", 4750, CsItem.AWP, ItemType.Primary, CsTeam.None, false),
        new LoadoutItem("SCAR-20", 5000, CsItem.SCAR20, ItemType.Primary, CsTeam.CounterTerrorist, false),
        new LoadoutItem("G3SG1", 5000, CsItem.G3SG1, ItemType.Primary, CsTeam.Terrorist, false),
        //Rifles
        new LoadoutItem("Galil", 1800, CsItem.Galil, ItemType.Primary, CsTeam.Terrorist, true),
        new LoadoutItem("Famas", 2250, CsItem.Famas, ItemType.Primary, CsTeam.CounterTerrorist, true),
        new LoadoutItem("AK-47", 2700, CsItem.AK47, ItemType.Primary, CsTeam.Terrorist, true),
        new LoadoutItem("M4A1-S", 2900, CsItem.M4A1S, ItemType.Primary, CsTeam.CounterTerrorist, true),
        new LoadoutItem("SG-553", 3000, CsItem.SG553, ItemType.Primary, CsTeam.Terrorist, true),
        new LoadoutItem("M4A4", 3100, CsItem.M4A4, ItemType.Primary, CsTeam.CounterTerrorist, true),
        new LoadoutItem("AUG", 1800, CsItem.AUG, ItemType.Primary, CsTeam.CounterTerrorist, true),
        //SMGs
        new LoadoutItem("MAC-10", 1050, CsItem.Mac10, ItemType.Primary, CsTeam.Terrorist, true),
        new LoadoutItem("UMP-45", 1200, CsItem.UMP, ItemType.Primary, CsTeam.None, true),
        new LoadoutItem("MP9", 1250, CsItem.MP9, ItemType.Primary, CsTeam.CounterTerrorist, true),
        new LoadoutItem("PP-BIZON", 1400, CsItem.PPBizon, ItemType.Primary, CsTeam.None, true),
        new LoadoutItem("MP5", 1500, CsItem.MP5, ItemType.Primary, CsTeam.None, true),
        new LoadoutItem("MP7", 1500, CsItem.MP7, ItemType.Primary, CsTeam.None, true),
        new LoadoutItem("P90", 2350, CsItem.P90, ItemType.Primary, CsTeam.None, true),
        //Heavies
        new LoadoutItem("Nova", 1050, CsItem.Nova, ItemType.Primary, CsTeam.None, false),
        new LoadoutItem("SawedOff", 1100, CsItem.SawedOff, ItemType.Primary, CsTeam.Terrorist, false),
        new LoadoutItem("MAG-7", 1300, CsItem.MAG7, ItemType.Primary, CsTeam.CounterTerrorist, false),
        new LoadoutItem("Negev", 1700, CsItem.Negev, ItemType.Primary, CsTeam.None, false),
        new LoadoutItem("XM-1014", 1050, CsItem.XM1014, ItemType.Primary, CsTeam.None, false),
        new LoadoutItem("M249", 5200, CsItem.M249, ItemType.Primary, CsTeam.None, false),
        //Secondarys
        //Pistols
        new LoadoutItem("P2000", 0, CsItem.P2000, ItemType.Secondary, CsTeam.CounterTerrorist, true),
        new LoadoutItem("USP-S", 0, CsItem.USP, ItemType.Secondary, CsTeam.CounterTerrorist, true),
        new LoadoutItem("Glock-18", 0, CsItem.Glock, ItemType.Secondary, CsTeam.Terrorist, true),
        new LoadoutItem("P250", 300, CsItem.P250, ItemType.Secondary, CsTeam.None, true),
        new LoadoutItem("Dualies", 300, CsItem.Dualies, ItemType.Secondary, CsTeam.None, true),
        new LoadoutItem("Five-Seven", 500, CsItem.FiveSeven, ItemType.Secondary, CsTeam.CounterTerrorist, true),
        new LoadoutItem("Tec-9", 500, CsItem.Tec9, ItemType.Secondary, CsTeam.Terrorist, true),
        new LoadoutItem("CZ-75", 500, CsItem.CZ, ItemType.Secondary, CsTeam.None, true),
        new LoadoutItem("R8", 600, CsItem.R8, ItemType.Secondary, CsTeam.None, true),
        new LoadoutItem("Deagle", 700, CsItem.Deagle, ItemType.Secondary, CsTeam.None, true),
        //
        //Armor
        new LoadoutItem("No Armor", 0 , null, ItemType.Armor, CsTeam.None, true),
        new LoadoutItem("Half Armor", 650, CsItem.Kevlar, ItemType.Armor, CsTeam.None, true),
        new LoadoutItem("Full Armor", 1000, CsItem.KevlarHelmet, ItemType.Armor, CsTeam.None, true),
        //
        //Grenades
        new LoadoutItem("Decoy", 50, CsItem.Decoy, ItemType.Grenade, CsTeam.None, true),
        new LoadoutItem("Flashbang", 200, CsItem.Flashbang, ItemType.Grenade, CsTeam.None, true),
        new LoadoutItem("Smoke", 300, CsItem.Smoke, ItemType.Grenade, CsTeam.None, true),
        new LoadoutItem("HE Grenade", 300, CsItem.HEGrenade, ItemType.Grenade, CsTeam.None, true),
        new LoadoutItem("Molotov", 400, CsItem.Molotov, ItemType.Grenade, CsTeam.Terrorist, true),
        new LoadoutItem("Incendiary", 400, CsItem.Incendiary, ItemType.Grenade, CsTeam.CounterTerrorist, true),
        //Experimental grenades
        new LoadoutItem("X-Ray Grenade", 750, CsItem.TAGrenade, ItemType.Grenade, CsTeam.None, false),
        new LoadoutItem("Diversion Grenade", 400, CsItem.Diversion, ItemType.Grenade, CsTeam.None, false)
    };
}