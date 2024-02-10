using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;

namespace MakisRetakeAllocator.Loadouts;

public class LoadoutFactory {

    public static Dictionary<RoundType, Dictionary<CsTeam, List<CsItem>>> CreateDefaultLoadouts() {
        Dictionary<RoundType, Dictionary<CsTeam, List<CsItem>>> myDefaultLoadouts = new Dictionary<RoundType, Dictionary<CsTeam, List<CsItem>>>();

        myDefaultLoadouts.Add(RoundType.Pistol, new Dictionary<CsTeam, List<CsItem>>());
        myDefaultLoadouts.Add(RoundType.FullBuy, new Dictionary<CsTeam, List<CsItem>>());

        foreach (RoundType aRoundType in Enum.GetValues(typeof(RoundType))) {
            (List<CsItem> aTerroristLoadout, List<CsItem> aCounterTerroristLoadout) = PopulateDefaultloadouts(aRoundType);
            myDefaultLoadouts[aRoundType].Add(CsTeam.Terrorist, aTerroristLoadout);
            myDefaultLoadouts[aRoundType].Add(CsTeam.CounterTerrorist, aCounterTerroristLoadout);
        }

        return myDefaultLoadouts;
    }

    private static (List<CsItem> aTerroristLoadout, List<CsItem> aCounterTerroristLoadout) PopulateDefaultloadouts(RoundType aRoundType) {
        List<CsItem> myTLoadout = new List<CsItem>();
        List<CsItem> myCTLoadout = new List<CsItem>();

        switch (aRoundType) {
            case RoundType.Pistol:
                myTLoadout.AddRange(new List<CsItem> {
                                CsItem.Glock,
                                CsItem.Kevlar
                });

                myCTLoadout.AddRange(new List<CsItem> {
                                CsItem.USP,
                                CsItem.Kevlar
                });
                break;

            case RoundType.FullBuy:
                myTLoadout.AddRange(new List<CsItem> {
                                CsItem.AK47,
                                CsItem.Glock,
                                CsItem.KevlarHelmet,
                                CsItem.Flashbang
                                //TODO add a dynamic way of generating grenades based on player preference, but also max # per nade
                });

                myCTLoadout.AddRange(new List<CsItem> {
                                CsItem.M4A1S,
                                CsItem.Glock,
                                CsItem.KevlarHelmet,
                                CsItem.Flashbang
                                //TODO add a dynamic way of generating grenades based on player preference, but also max # per nade
                });
                break;

            default:
                //Why would this ever happen??
                break;
        }

        return (myTLoadout, myCTLoadout);
    }

    public static HashSet<LoadoutItem> LOADOUT_ITEMS { get; } = new HashSet<LoadoutItem>() {
        //Primarys
        //Snipers
        new LoadoutItems.PrimaryWeapon("Scout", 1700, CsItem.SSG08, ItemType.Sniper, CsTeam.None),
        new LoadoutItems.PrimaryWeapon("AWP", 4750, CsItem.AWP, ItemType.Sniper, CsTeam.None),
        new LoadoutItems.PrimaryWeapon("SCAR-20", 5000, CsItem.SCAR20, ItemType.Sniper, CsTeam.CounterTerrorist),
        new LoadoutItems.PrimaryWeapon("G3SG1", 5000, CsItem.G3SG1, ItemType.Sniper, CsTeam.Terrorist),
        //Rifles
        new LoadoutItems.PrimaryWeapon("Galil", 1800, CsItem.Galil, ItemType.Rifle, CsTeam.Terrorist),
        new LoadoutItems.PrimaryWeapon("Famas", 2250, CsItem.Famas, ItemType.Rifle, CsTeam.CounterTerrorist),
        new LoadoutItems.PrimaryWeapon("AK-47", 2700, CsItem.AK47, ItemType.Rifle, CsTeam.Terrorist),
        new LoadoutItems.PrimaryWeapon("M4A1-S", 2900, CsItem.M4A1S, ItemType.Rifle, CsTeam.CounterTerrorist),
        new LoadoutItems.PrimaryWeapon("SG-553", 3000, CsItem.SG553, ItemType.Rifle, CsTeam.Terrorist),
        new LoadoutItems.PrimaryWeapon("M4A4", 3100, CsItem.M4A4, ItemType.Rifle, CsTeam.CounterTerrorist),
        new LoadoutItems.PrimaryWeapon("AUG", 1800, CsItem.AUG, ItemType.Rifle, CsTeam.CounterTerrorist),
        //SMGs
        new LoadoutItems.PrimaryWeapon("MAC-10", 1050, CsItem.Mac10, ItemType.SMG, CsTeam.Terrorist),
        new LoadoutItems.PrimaryWeapon("UMP-45", 1200, CsItem.UMP, ItemType.SMG, CsTeam.None),
        new LoadoutItems.PrimaryWeapon("MP9", 1250, CsItem.MP9, ItemType.SMG, CsTeam.CounterTerrorist),
        new LoadoutItems.PrimaryWeapon("PP-BIZON", 1400, CsItem.PPBizon, ItemType.SMG, CsTeam.None),
        new LoadoutItems.PrimaryWeapon("MP5", 1500, CsItem.MP5, ItemType.SMG, CsTeam.None),
        new LoadoutItems.PrimaryWeapon("MP7", 1500, CsItem.MP7, ItemType.SMG, CsTeam.None),
        new LoadoutItems.PrimaryWeapon("P90", 2350, CsItem.P90, ItemType.SMG, CsTeam.None),
        //Heavies
        new LoadoutItems.PrimaryWeapon("Nova", 1050, CsItem.Nova, ItemType.Heavy, CsTeam.None),
        new LoadoutItems.PrimaryWeapon("SawedOff", 1100, CsItem.SawedOff, ItemType.Heavy, CsTeam.Terrorist),
        new LoadoutItems.PrimaryWeapon("MAG-7", 1300, CsItem.MAG7, ItemType.Heavy, CsTeam.CounterTerrorist),
        new LoadoutItems.PrimaryWeapon("Negev", 1700, CsItem.Negev, ItemType.Heavy, CsTeam.None),
        new LoadoutItems.PrimaryWeapon("XM-1014", 1050, CsItem.XM1014, ItemType.Heavy, CsTeam.None),
        new LoadoutItems.PrimaryWeapon("M249", 5200, CsItem.M249, ItemType.Heavy, CsTeam.None),
        //Secondarys
        //Pistols
        new LoadoutItems.SecondaryWeapon("P2000", 0, CsItem.P2000, ItemType.Pistol, CsTeam.CounterTerrorist),
        new LoadoutItems.SecondaryWeapon("USP-S", 0, CsItem.USP, ItemType.Pistol, CsTeam.CounterTerrorist),
        new LoadoutItems.SecondaryWeapon("Glock-18", 0, CsItem.Glock, ItemType.Pistol, CsTeam.Terrorist),
        new LoadoutItems.SecondaryWeapon("P250", 300, CsItem.P250, ItemType.Pistol, CsTeam.None),
        new LoadoutItems.SecondaryWeapon("Dualies", 300, CsItem.Dualies, ItemType.Pistol, CsTeam.None),
        new LoadoutItems.SecondaryWeapon("Five-Seven", 500, CsItem.FiveSeven, ItemType.Pistol, CsTeam.CounterTerrorist),
        new LoadoutItems.SecondaryWeapon("Tec-9", 500, CsItem.Tec9, ItemType.Pistol, CsTeam.Terrorist),
        new LoadoutItems.SecondaryWeapon("CZ-75", 500, CsItem.CZ, ItemType.Pistol, CsTeam.None),
        new LoadoutItems.SecondaryWeapon("R8", 600, CsItem.R8, ItemType.Pistol, CsTeam.None),
        new LoadoutItems.SecondaryWeapon("Deagle", 700, CsItem.Deagle, ItemType.Pistol, CsTeam.None),
        //
        //Armor
        new LoadoutItems.Armor("Half Armor", 650, CsItem.Kevlar, ItemType.Armor, CsTeam.None),
        new LoadoutItems.Armor("Full Armor", 1000, CsItem.KevlarHelmet, ItemType.Armor, CsTeam.None),
        //
        //Grenades
        new LoadoutItems.Grenade("Decoy", 50, CsItem.Decoy, ItemType.Grenade, CsTeam.None),
        new LoadoutItems.Grenade("Flashbang", 200, CsItem.Flashbang, ItemType.Grenade, CsTeam.None),
        new LoadoutItems.Grenade("Smoke", 300, CsItem.Smoke, ItemType.Grenade, CsTeam.None),
        new LoadoutItems.Grenade("HE Grenade", 300, CsItem.HEGrenade, ItemType.Grenade, CsTeam.None),
        new LoadoutItems.Grenade("Molotov", 400, CsItem.Molotov, ItemType.Grenade, CsTeam.Terrorist),
        new LoadoutItems.Grenade("Incendiary", 400, CsItem.Incendiary, ItemType.Grenade, CsTeam.CounterTerrorist),
        //Experimental grenades
        new LoadoutItems.Grenade("X-Ray Grenade", 750, CsItem.TAGrenade, ItemType.Grenade, CsTeam.None),
        new LoadoutItems.Grenade("Diversion Grenade", 400, CsItem.Diversion, ItemType.Grenade, CsTeam.None)
    };
}