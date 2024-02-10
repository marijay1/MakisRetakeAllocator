using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using static MakisRetakeAllocator.Loadouts.LoadoutFactory;

namespace MakisRetakeAllocator.Loadouts;

public record PlayerLoadout {
    private readonly int theUserId;
    private Dictionary<RoundType, Dictionary<CsTeam, List<CsItem>>> theLoadouts;

    public PlayerLoadout(int anUserId, Dictionary<RoundType, Dictionary<CsTeam, List<CsItem>>>? aLoadouts) {
        theUserId = anUserId;
        theLoadouts = aLoadouts ?? CreateDefaultLoadouts();
    }

    public List<CsItem> getLoadout(RoundType aRoundType, CsTeam aTeam) {
        if (theLoadouts.TryGetValue(aRoundType, out var myTeamLoadouts)) {
            if (myTeamLoadouts.TryGetValue(aTeam, out var myLoadout)) {
                return myLoadout;
            }
        }

        return new List<CsItem>();
    }
}