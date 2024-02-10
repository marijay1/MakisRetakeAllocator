using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;

namespace MakisRetakeAllocator.Loadouts {

    public abstract class LoadoutItem {
        public string theName { get; protected set; }
        public int theCost { get; protected set; }
        public CsItem theItem { get; protected set; }
        public ItemType theItemType { get; protected set; }
        public CsTeam theCsTeam { get; protected set; }

        public LoadoutItem(string aName, int aCost, CsItem anItem, ItemType anItemType, CsTeam aCsTeam) {
            theName = aName;
            theCost = aCost;
            theItem = anItem;
            theItemType = anItemType;
            theCsTeam = aCsTeam;
        }
    }

    public class LoadoutItems {

        public class PrimaryWeapon : LoadoutItem {

            public PrimaryWeapon(string aName, int aCost, CsItem anItem, ItemType anItemType, CsTeam aCsTeam) : base(aName, aCost, anItem, anItemType, aCsTeam) {
            }
        }

        public class SecondaryWeapon : LoadoutItem {

            public SecondaryWeapon(string aName, int aCost, CsItem anItem, ItemType anItemType, CsTeam aCsTeam) : base(aName, aCost, anItem, anItemType, aCsTeam) {
            }
        }

        public class Armor : LoadoutItem {

            public Armor(string aName, int aCost, CsItem anItem, ItemType anItemType, CsTeam aCsTeam) : base(aName, aCost, anItem, anItemType, aCsTeam) {
            }
        }

        public class Grenade : LoadoutItem {

            public Grenade(string aName, int aCost, CsItem anItem, ItemType anItemType, CsTeam aCsTeam) : base(aName, aCost, anItem, anItemType, aCsTeam) {
            }
        }
    }

    public enum ItemType {
        None,
        Sniper,
        Rifle,
        SMG,
        Heavy,
        Pistol,
        Armor,
        Grenade
    }
}