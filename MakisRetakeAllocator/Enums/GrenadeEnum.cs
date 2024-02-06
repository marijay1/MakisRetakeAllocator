using CounterStrikeSharp.API.Modules.Entities.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakisRetakeAllocator.Enums;
public enum GrenadeEnum {
    [Item(CsItem.Flashbang)]
    Flashbang,
    [Item(CsItem.Smoke)]
    Smoke,
    [Item(CsItem.HEGrenade)]
    Grenade,
    [Item(CsItem.Molotov)]
    Molotov,
    [Item(CsItem.Incendiary)]
    Incendiary,
    [Item(CsItem.Decoy)]
    Decoy,
    [Item(CsItem.TAGrenade)]
    XRay,
    [Item(CsItem.Diversion)]
    Diversion
}

