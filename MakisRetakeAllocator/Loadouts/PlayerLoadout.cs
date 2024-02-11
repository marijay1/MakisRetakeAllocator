namespace MakisRetakeAllocator.Loadouts;

public class PlayerLoadout {
    private readonly int theUserId;
    private Dictionary<RoundType, PlayerItems> theCounterTerroristLoadouts;
    private Dictionary<RoundType, PlayerItems> theTerroristLoadouts;

    public PlayerLoadout(int anUserId, Dictionary<RoundType, PlayerItems>? aCounterTerroristLoadouts, Dictionary<RoundType, PlayerItems>? aTerroristLoadouts) {
        theUserId = anUserId;
        theCounterTerroristLoadouts = aCounterTerroristLoadouts ?? new Dictionary<RoundType, PlayerItems>();
        theTerroristLoadouts = aTerroristLoadouts ?? new Dictionary<RoundType, PlayerItems>();
    }

    public int getUserId() {
        return theUserId;
    }

    public Dictionary<RoundType, PlayerItems> getCounterTerroristLoadouts() {
        return theCounterTerroristLoadouts;
    }

    public Dictionary<RoundType, PlayerItems> getTerroristLoadouts() {
        return theTerroristLoadouts;
    }

    public class PlayerItems {
        public LoadoutItems.PrimaryWeapon? thePrimaryWeapon;
        public LoadoutItems.SecondaryWeapon theSecondaryWeapon;
        public LoadoutItems.Armor theArmor;
        public List<LoadoutItems.Grenade>? theGrenades;
        public bool theIsKitEnabled;

        public PlayerItems(LoadoutItems.PrimaryWeapon? aPrimaryWeapon, LoadoutItems.SecondaryWeapon aSecondaryWeapon, LoadoutItems.Armor aArmor, List<LoadoutItems.Grenade> aGrenades, bool anIsKitEnabled) {
            thePrimaryWeapon = aPrimaryWeapon;
            theSecondaryWeapon = aSecondaryWeapon;
            theArmor = aArmor;
            theGrenades = aGrenades ?? new List<LoadoutItems.Grenade>();
            theIsKitEnabled = anIsKitEnabled;
        }
    }
}