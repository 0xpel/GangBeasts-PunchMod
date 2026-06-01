# GangBeasts-PunchMod

A MelonLoader mod for Gang Beasts that buffs your local player's punch force, damage, speed and lift.

## Features
- 25x punch force
- 15x punch damage
- 10x movement speed
- 50x lift strength
- Only affects your local player — bots and other players are unaffected

## Requirements
- [MelonLoader v0.7.3+](https://github.com/LavaGang/MelonLoader/releases)
- Gang Beasts (Steam)

## Installation
1. Install MelonLoader using the installer
2. Download `GangBeastsPunchMod2.dll` from the [Releases](../../releases) page
3. Place it in `Gang Beasts/Mods/`
4. Launch the game

## Notes
- Works **only in local matches** — online servers sync physics server-side so the mod has no effect
- Does not affect bots or other players
- Activates automatically 3 seconds after entering a match

## How it works
Uses `Traverse` from HarmonyLib to access IL2CPP properties at runtime.
Identifies the local player by checking the `_controlledBy` property on the `Actor` class in the `Femur` namespace.

**Properties modified:**
| Property | Multiplier |
|----------|------------|
| `_punchForceModifer` | 25x |
| `_punchDamageModifer` | 15x |
| `groundSpeed` | 10x |
| `liftStrength` | 50x |

## Technical Details
- Built with MelonLoader + HarmonyLib
- Game uses IL2CPP — required reverse engineering with dnSpy to find correct class names and properties
- `Traverse` is used to access private IL2CPP properties at runtime without direct references

## Known Issues
- Does not work in online multiplayer
- May need adjustment after game updates

## Credits
- Inspired by [OnePunchGB by Zooks](https://thunderstore.io/c/gang-beasts/p/Zooks/OnePunchGB/)
- Built with [MelonLoader](https://melonwiki.xyz/)
