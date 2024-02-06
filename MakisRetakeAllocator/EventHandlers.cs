using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakisRetakeAllocator;
public partial class EventHandlers {
    [GameEventHandler]
    public HookResult OnPortItemPurchase(EventItemPurchase @event, GameEventInfo anInfo) {
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo anInfo) {
        return HookResult.Continue;
    }
}