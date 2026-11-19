# Talonborne compatibility

Baseline: supplied **Talonborne 1.2.0** (`talonborne`), which requires Jaunt
3.3.1. This integration is limited to the mod's existing player-sensitive
grazing behavior; it does not add combat or fear to raised mounts.

## Scope

The supplied entities are a nest, eggs, three young stages and the adult
hippogryph. Four color variants exist: common, crystal, fallen and royal.
Animals are raised from nest eggs; no separate wild combat creature definition
exists. Neither adult nor young AI contains native player-target flee, melee
or pursuit tasks to copy.

Adults and young already interrupt their `graze` idle animation for nearby
players. Two small JSON patches append `scarecrow`, `little-scarecrow` and
`strawdummy` to these existing lists. The native **3-block** configured range,
generation-dependent idle sensitivity, priorities and animation timing remain
unchanged. Interruption does not force flight or movement away from a scarecrow.

## Custom behavior review

The supplied DLL registers `talonbeddown`, derived from the game's `AiTaskIdle`.
It requires repeated pathfinding failures and checks ground/water/movement
conditions. It has no explicit player proximity selector or nonzero stop range
in these assets; it is left unchanged.

The commanded-rest behavior gates task execution while the rest flag is set.
The patches do not override that gate or change player interactions, ownership,
owner recall, riding, saddles, Jaunt flight, growth, feeding or nest spawning.
The mod's `AssetsFinalize` adjusts nest spawn frequency, not the patched AI.

Eggs and nests receive no patches. The egg's `onNearbyEntityCodes` field is not
read by the supplied game's `AiTaskIdle`; it is not treated as an effective
target selector or expanded with inert entries. This integration does not
change the upstream hatching animation or attempt to repair it.

## Files and validation

The patches are in `compatibility/talonborne/patches/hippogryph.json` and
`hippogryphyoung.json`. Adults store tasks in `behaviorConfigs/taskai`; young
store them directly in the server task-AI behavior. No C# adapter, new AI tasks
or natural-spawn prefixes are added.

Automated checks apply all six operations to the supplied assets and use the
game's actual variant resolver for **16 variants**: four adults and twelve
young across baby, juvenile and adolescent stages. After removing only the
three appended targets, the complete resolved definitions match the originals.
Build passes; the author completed the in-game test successfully.

For gameplay testing, compare grazing interruption with and without a nearby
scarecrow for adults and young. Confirm commanded rest remains active and normal
owner recall, riding and flight still work. No fleeing or attacks are expected.
