# Fantasy Creatures compatibility

Baseline: supplied **Fantasy Creatures Mod 0.8.3**, mod ID
`fantasycreaturesupdate`, asset domain `fantasycreatures`.

## Scope and behavior

Scarecrows represent players for the existing combat/fear AI. This includes
hostile humanoids and undead: they can approach and damage a scarecrow rather
than being forced to flee. The patches do not make these creatures harmless.

| Definition | Adapted native player reactions |
| --- | --- |
| Cave goblin | Daylight-sensitive flight, injury flight, melee and pursuit |
| Surface goblin | Injury flight, melee and pursuit in hunter, pelted and fallback task sets |
| Lone hunter goblin | Injury flight, melee and pursuit |
| Ork | Injury flight, melee and pursuit in its wildcard task set |
| Skeleton, surface skeleton and deserter | Injury flight, melee, pursuit and sleeping interruption |
| Skeleton blackguard | Melee, pursuit and sleeping interruption |
| Slime | Injury flight, melee and pursuit |
| Treant | Melee, pursuit and idle interruption |
| Ent shepherd | Melee only; its existing flight targets do not include players |
| Red/scorched ground dragons | Claw, bite, wing attack and normal/injury pursuit |
| Red/scorched flying dragons | Native player-target flight with each form's speed/range |
| Blue dragon | Player-target wing attack only |

The blue dragon's `stayclosetoentity` player-following task is intentionally
unchanged: it is social following, not combat/fear. Its claw and bite tasks do
not target players, so no scarecrow copies of those attacks are added. No
separate owned/tame entity variants were found in the supplied definitions.

## Patch structure

All 14 JSON patch files are under
`compatibility/fantasycreaturesupdate/patches/`. Their targets use the upstream
`fantasycreatures` asset domain. Both ordinary `aitasks` arrays and variant
`aitasksByType` arrays are extended, including wildcard fallback arrays.

The 51 additional tasks target only `scarecrow`, `little-scarecrow` and
`strawdummy`. Original player/prey tasks remain unchanged. Copies retain native
priorities, movement, sounds, animation, damage, reach, daylight and emotion
conditions. Retaliation is disabled in copies; flee copies also disable
instant flight on unrelated damage. Five existing player wake/idle-interruption
lists receive the three targets without replacing their original entries.

No C# adapter, additional spawn-blocking prefixes or upstream asset edits are
needed. Spawning, dragon transformations and loot remain unchanged. Without
Fantasy Creatures installed, these patches have no matching external assets.

## Validation

Automated checks apply every shipped operation to the supplied assets and use
the game's actual variant resolver: **14 definitions, 26 resolved variants,
51 task copies, five wake lists and 74 resolved additional task configurations**.
Checks verify native task preservation, target isolation and retained variant
settings. Build passes; the author completed the in-game test successfully.

For gameplay testing, include hunter/pelted surface goblins, both ground/flying
dragon colors, the blue dragon, skeleton sleep interruption and large tree
creatures' melee reach. Test both scarecrow sizes and the vanilla straw dummy.
Hostile attacks are expected; injury-only flight still requires its native
emotion state, and cave-goblin daylight flight retains its original chance.
