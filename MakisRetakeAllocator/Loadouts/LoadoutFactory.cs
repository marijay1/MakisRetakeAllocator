using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using MakisRetakeAllocator.Enums;
using static MakisRetakeAllocator.Loadouts.PlayerLoadout;

namespace MakisRetakeAllocator.Loadouts;

public class LoadoutFactory {

    public PlayerLoadout CreateDefaultLoadout(CCSPlayerController aPlayer) {
        PlayerLoadout myPlayerLoadout = new PlayerLoadout((int)aPlayer.UserId!, null, null);

        Dictionary<RoundType, PlayerItems> myCounterTerroristLoadout = myPlayerLoadout.getCounterTerroristLoadouts();
        Dictionary<RoundType, PlayerItems> myTerroristLoadout = myPlayerLoadout.getTerroristLoadouts();

        myCounterTerroristLoadout.Add(RoundType.Pistol, new PlayerItems(
            null,
            getLoadoutItem(CsItem.USP),
            getLoadoutItem(CsItem.Kevlar),
            getLoadoutItem(CsItem.Flashbang),
            false
            ));

        myCounterTerroristLoadout.Add(RoundType.FullBuy, new PlayerItems(
            getLoadoutItem(CsItem.M4A1S),
            getLoadoutItem(CsItem.USP),
            getLoadoutItem(CsItem.KevlarHelmet),
            getLoadoutItem(CsItem.Flashbang),
            true
            ));

        myTerroristLoadout.Add(RoundType.Pistol, new PlayerItems(
            null,
            getLoadoutItem(CsItem.Glock),
            getLoadoutItem(CsItem.Kevlar),
            getLoadoutItem(CsItem.Flashbang),
            false
            ));

        myTerroristLoadout.Add(RoundType.Pistol, new PlayerItems(
            getLoadoutItem(CsItem.AK47),
            getLoadoutItem(CsItem.Glock),
            getLoadoutItem(CsItem.KevlarHelmet),
            getLoadoutItem(CsItem.Flashbang),
            false
            ));

        return new PlayerLoadout((int)aPlayer.UserId!, myCounterTerroristLoadout, myTerroristLoadout);
    }

    public LoadoutItem getLoadoutItem(CsItem aCsItem) {
        return LOADOUT_ITEMS.FirstOrDefault(aWeapon => aWeapon.theItem.Equals(aCsItem))!;
    }

    public LoadoutItem getLoadoutItem(String aWeaponString) {
        return LOADOUT_ITEMS.FirstOrDefault(aWeapon => aWeapon.theName.Equals(aWeaponString))!;
    }

    public List<LoadoutItem> LOADOUT_ITEMS { get; } = new List<LoadoutItem>() {
        //Primarys
        //Snipers
        new LoadoutItem("Scout", 1700, CsItem.SSG08, ItemType.Sniper, CsTeam.None),
        new LoadoutItem("AWP", 4750, CsItem.AWP, ItemType.Sniper, CsTeam.None),
        new LoadoutItem("SCAR-20", 5000, CsItem.SCAR20, ItemType.Sniper, CsTeam.CounterTerrorist),
        new LoadoutItem("G3SG1", 5000, CsItem.G3SG1, ItemType.Sniper, CsTeam.Terrorist),
        //Rifles
        new LoadoutItem("Galil", 1800, CsItem.Galil, ItemType.Rifle, CsTeam.Terrorist),
        new LoadoutItem("Famas", 2250, CsItem.Famas, ItemType.Rifle, CsTeam.CounterTerrorist),
        new LoadoutItem("AK-47", 2700, CsItem.AK47, ItemType.Rifle, CsTeam.Terrorist),
        new LoadoutItem("M4A1-S", 2900, CsItem.M4A1S, ItemType.Rifle, CsTeam.CounterTerrorist),
        new LoadoutItem("SG-553", 3000, CsItem.SG553, ItemType.Rifle, CsTeam.Terrorist),
        new LoadoutItem("M4A4", 3100, CsItem.M4A4, ItemType.Rifle, CsTeam.CounterTerrorist),
        new LoadoutItem("AUG", 1800, CsItem.AUG, ItemType.Rifle, CsTeam.CounterTerrorist),
        //SMGs
        new LoadoutItem("MAC-10", 1050, CsItem.Mac10, ItemType.SMG, CsTeam.Terrorist),
        new LoadoutItem("UMP-45", 1200, CsItem.UMP, ItemType.SMG, CsTeam.None),
        new LoadoutItem("MP9", 1250, CsItem.MP9, ItemType.SMG, CsTeam.CounterTerrorist),
        new LoadoutItem("PP-BIZON", 1400, CsItem.PPBizon, ItemType.SMG, CsTeam.None),
        new LoadoutItem("MP5", 1500, CsItem.MP5, ItemType.SMG, CsTeam.None),
        new LoadoutItem("MP7", 1500, CsItem.MP7, ItemType.SMG, CsTeam.None),
        new LoadoutItem("P90", 2350, CsItem.P90, ItemType.SMG, CsTeam.None),
        //Heavies
        new LoadoutItem("Nova", 1050, CsItem.Nova, ItemType.Heavy, CsTeam.None),
        new LoadoutItem("SawedOff", 1100, CsItem.SawedOff, ItemType.Heavy, CsTeam.Terrorist),
        new LoadoutItem("MAG-7", 1300, CsItem.MAG7, ItemType.Heavy, CsTeam.CounterTerrorist),
        new LoadoutItem("Negev", 1700, CsItem.Negev, ItemType.Heavy, CsTeam.None),
        new LoadoutItem("XM-1014", 1050, CsItem.XM1014, ItemType.Heavy, CsTeam.None),
        new LoadoutItem("M249", 5200, CsItem.M249, ItemType.Heavy, CsTeam.None),
        //Secondarys
        //Pistols
        new LoadoutItem("P2000", 0, CsItem.P2000, ItemType.Pistol, CsTeam.CounterTerrorist),
        new LoadoutItem("USP-S", 0, CsItem.USP, ItemType.Pistol, CsTeam.CounterTerrorist),
        new LoadoutItem("Glock-18", 0, CsItem.Glock, ItemType.Pistol, CsTeam.Terrorist),
        new LoadoutItem("P250", 300, CsItem.P250, ItemType.Pistol, CsTeam.None),
        new LoadoutItem("Dualies", 300, CsItem.Dualies, ItemType.Pistol, CsTeam.None),
        new LoadoutItem("Five-Seven", 500, CsItem.FiveSeven, ItemType.Pistol, CsTeam.CounterTerrorist),
        new LoadoutItem("Tec-9", 500, CsItem.Tec9, ItemType.Pistol, CsTeam.Terrorist),
        new LoadoutItem("CZ-75", 500, CsItem.CZ, ItemType.Pistol, CsTeam.None),
        new LoadoutItem("R8", 600, CsItem.R8, ItemType.Pistol, CsTeam.None),
        new LoadoutItem("Deagle", 700, CsItem.Deagle, ItemType.Pistol, CsTeam.None),
        //
        //Armor
        new LoadoutItem("Half Armor", 650, CsItem.Kevlar, ItemType.Armor, CsTeam.None),
        new LoadoutItem("Full Armor", 1000, CsItem.KevlarHelmet, ItemType.Armor, CsTeam.None),
        //
        //Grenades
        new LoadoutItem("Decoy", 50, CsItem.Decoy, ItemType.Grenade, CsTeam.None),
        new LoadoutItem("Flashbang", 200, CsItem.Flashbang, ItemType.Grenade, CsTeam.None),
        new LoadoutItem("Smoke", 300, CsItem.Smoke, ItemType.Grenade, CsTeam.None),
        new LoadoutItem("HE Grenade", 300, CsItem.HEGrenade, ItemType.Grenade, CsTeam.None),
        new LoadoutItem("Molotov", 400, CsItem.Molotov, ItemType.Grenade, CsTeam.Terrorist),
        new LoadoutItem("Incendiary", 400, CsItem.Incendiary, ItemType.Grenade, CsTeam.CounterTerrorist),
        //Experimental grenades
        new LoadoutItem("X-Ray Grenade", 750, CsItem.TAGrenade, ItemType.Grenade, CsTeam.None),
        new LoadoutItem("Diversion Grenade", 400, CsItem.Diversion, ItemType.Grenade, CsTeam.None)
    };
}