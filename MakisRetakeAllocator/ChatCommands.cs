using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using MakisRetakeAllocator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakisRetakeAllocator;
public partial class MakisRetakeAllocator {
    [ConsoleCommand("css_guns", "Opens menu to allow players to choose their weapons.")]
    [ConsoleCommand("css_gun", "Opens menu to allow players to choose their weapons.")]
    [ConsoleCommand("css_weapons", "Opens menu to allow players to choose their weapons.")]
    [ConsoleCommand("css_weapon", "Opens menu to allow players to choose their weapons.")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void OnGunsCommand(CCSPlayerController? aPlayer, CommandInfo aCommandInfo)
    {
        if (aPlayer == null)
        {
            return;
        }
        //Open weapons menu for player
    }

    [ConsoleCommand("css_awp", "A player trying to get an AWP. Naughty..")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void OnAwpCommand(CCSPlayerController? aPlayer, CommandInfo aCommandoInfo)
    {
        if (aPlayer == null)
        {
            return;
        }
        //Stupid idiot thinks they are getting an AWP HAHAHAHAHA
    }
}

