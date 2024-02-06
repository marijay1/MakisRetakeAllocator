using CounterStrikeSharp.API.Modules.Entities.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakisRetakeAllocator.Enums;
public enum ArmorEnum {
    None,
    [Item(CsItem.Kevlar)]
    HalfArmor,
    [Item(CsItem.KevlarHelmet)]
    FullArmor
}

