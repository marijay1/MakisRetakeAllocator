[![GPLv3 License](https://img.shields.io/badge/License-GPL%20v3-yellow.svg)](https://opensource.org/licenses/) [![Github All Releases](https://img.shields.io/github/downloads/marijay1/MakisRetakeAllocator/total.svg)](https://github.com/marijay1/MakisRetake/releases)
# Maki's Retakes Allocator

A CS2 Retakes Allocator, created using CounterStrikeSharp.
This plugin was created for my own server, so I will not go out of my way to add requested features.


## Features

- MySQL to allow sync between multiple servers
- Center HTML menu to select loadouts
- Pistol and Full Buy rounds
- Seperate loadouts for Full buy and Pistol
- Retakes Allocator config


## Roadmap

- [ ] Per weapons configuration

- [ ] Randomize loadout

- [x] Translations

- [ ] Icons in the Gun menu

- [x] Economy in the menu to simulate a regular buy round

- [ ] Add better feedback to the player/admin

- [ ] More database options


## Installation

- Download the [latest release](https://github.com/marijay1/MakisRetakeAllocator/releases)
- Extract the folder into the **plugins** folder in the **counterstrikesharp** directory

## Configuration

A file in the **configs/MakisRetakeAllocator** folder in the **counterstrikesharp** directory named `MakisRetakeAllocator.json` will be generated on the first load of the plugin. It will contain the following configurations:

| Database Config               | Description                                                       | Default   |
|-------------------------------|-------------------------------------------------------------------|-----------|
| Host                          | The host IP or host domain for the MySQL server                   | localhost |
| Database                      | The Database name for the MySQL server                            | database  |
| Username                      | The Username of the user for the MySQL server                     | username  |
| Password                      | The Password of the user for the MySQL server                     | password  |
| Port                          | The Port that is used in your MySQL server                        | 3306      |

| Retakes Config                | Description                                                       | Default   |
|-------------------------------|-------------------------------------------------------------------|-----------|
| NumberOfPistolRounds          | The number of Pistol rounds                                       | 5         |
| StartingTerroristMoney        | The starting money for Terrorist on a Buy round                   | 4850      |
| StartingCounterTerroristMoney | The starting money for Counter-Terrorist on a Buy round           | 4450      |
| SecondsUntilMenuTimeout       | The amount of time without interacting required to close the menu | 30        |

## Commands

| Command  | Arguments | Description                                   | Permissions |
|----------|-----------|-----------------------------------------------|-------------|
| css_guns |           | Opens the menu to configure a players loadout | None        |
| css_awp  |           | Tells players that awps are not permitted     | None        |

## Acknowledgements

I used the below repos for inspiration:
 - [B3none/cs2-retakes](https://github.com/B3none/cs2-retakes)
 - [yonilerner/cs2-retakes-allocator](https://github.com/matiassingers/awesome-readme)

