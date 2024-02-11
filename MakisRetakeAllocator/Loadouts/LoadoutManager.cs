using CounterStrikeSharp.API.Core;

namespace MakisRetakeAllocator.Loadouts;

public class LoadoutManager {
    private Dictionary<CCSPlayerController, PlayerLoadout> theLoadouts;

    public LoadoutManager() {
    }

    private Dictionary<CCSPlayerController, PlayerLoadout> initializePlayerLoadouts() {
        //TODO
    }

    public PlayerLoadout getLoadout(CCSPlayerController aPlayer) {
        return theLoadouts[aPlayer];
    }
}