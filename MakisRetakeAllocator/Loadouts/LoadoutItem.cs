using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;

namespace MakisRetakeAllocator.Loadouts;

public class LoadoutItem {
    public string theName { get; }
    public int theCost { get; }
    public CsItem theItem { get; }
    public ItemType theItemType { get; }
    public CsTeam theCsTeam { get; }

    public LoadoutItem(string aName, int aCost, CsItem anItem, ItemType anItemType, CsTeam aCsTeam) {
        theName = aName;
        theCost = aCost;
        theItem = anItem;
        theItemType = anItemType;
        theCsTeam = aCsTeam;
    }

    public enum ItemType {
        Sniper,
        Rifle,
        SMG,
        Heavy,
        Pistol,
        Armor,
        Grenade
    }
}