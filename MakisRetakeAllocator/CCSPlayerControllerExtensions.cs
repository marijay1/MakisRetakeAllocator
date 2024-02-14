using CounterStrikeSharp.API.Core;
using MakisRetakeAllocator.Loadouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSPlus.Base.Entities;

public static class CCSPlayerControllerExtensions {
    private static PlayerLoadout? thePlayerLoadout;

    public static void setPlayerLoadout(this CCSPlayerController aPlayer, PlayerLoadout aPlayerLoadout) {
        thePlayerLoadout = aPlayerLoadout;
    }

    public static PlayerLoadout getPlayerLoadout(this CCSPlayerController aPlayer) {
        return thePlayerLoadout;
    }
}