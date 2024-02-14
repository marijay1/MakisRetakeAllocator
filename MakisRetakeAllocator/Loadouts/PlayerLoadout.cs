using CounterStrikeSharp.API.Modules.Utils;
using MakisRetakeAllocator.Enums;

namespace MakisRetakeAllocator.Loadouts;

public class PlayerLoadout {
    private readonly int theSteamId;
    private Dictionary<RoundType, PlayerItems> theCounterTerroristLoadouts;
    private Dictionary<RoundType, PlayerItems> theTerroristLoadouts;

    public PlayerLoadout(int aSteamId, Dictionary<RoundType, PlayerItems>? aCounterTerroristLoadouts, Dictionary<RoundType, PlayerItems>? aTerroristLoadouts) {
        theSteamId = aSteamId;
        theCounterTerroristLoadouts = aCounterTerroristLoadouts ?? new Dictionary<RoundType, PlayerItems>();
        theTerroristLoadouts = aTerroristLoadouts ?? new Dictionary<RoundType, PlayerItems>();
    }

    public int getSteamId() {
        return theSteamId;
    }

    public Dictionary<RoundType, PlayerItems> getLoadouts(CsTeam aTeam) {
        if (aTeam == CsTeam.CounterTerrorist) {
            return theCounterTerroristLoadouts;
        }
        return theTerroristLoadouts;
    }

    public record PlayerItems {
        public LoadoutItem? thePrimaryWeapon { get; set; }
        public LoadoutItem theSecondaryWeapon { get; set; }
        public LoadoutItem theArmor { get; set; }
        public List<LoadoutItem> theGrenadePreference { get; set; }
        public bool? theIsKitEnabled { get; set; }

        public PlayerItems(LoadoutItem? aPrimaryWeapon, LoadoutItem aSecondaryWeapon, LoadoutItem aArmor, List<LoadoutItem> aGrenadePreference, bool? anIsKitEnabled) {
            thePrimaryWeapon = aPrimaryWeapon;
            theSecondaryWeapon = aSecondaryWeapon;
            theArmor = aArmor;
            theGrenadePreference = aGrenadePreference;
            theIsKitEnabled = anIsKitEnabled ?? false;
        }
    }
}