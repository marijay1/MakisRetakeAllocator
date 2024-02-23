using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;

namespace MakisRetakeAllocator;

public partial class MakisRetakeAllocator {

    [ConsoleCommand("css_guns", "Opens menu to allow players to choose their weapons.")]
    [ConsoleCommand("css_gun", "Opens menu to allow players to choose their weapons.")]
    [ConsoleCommand("css_weapons", "Opens menu to allow players to choose their weapons.")]
    [ConsoleCommand("css_weapon", "Opens menu to allow players to choose their weapons.")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void OnGunsCommand(CCSPlayerController? aPlayer, CommandInfo aCommandInfo) {
        if (aPlayer == null) {
            return;
        }
        new GunMenu(aPlayer, aPlayer.Team, theLoadoutFactory, Config, theDataContext);
    }

    [ConsoleCommand("css_awp", "A player trying to get an AWP. Naughty..")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void OnAwpCommand(CCSPlayerController? aPlayer, CommandInfo aCommandoInfo) {
        if (aPlayer == null) {
            return;
        }
        aPlayer.PrintToCenterHtml("No Awps!!");
        //Stupid idiot thinks they are getting an AWP HAHAHAHAHA
    }
}