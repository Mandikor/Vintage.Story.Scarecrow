# Pegasus compatibility

Baseline: supplied **Pegasus: Winged horses 1.1.0-rc.1** (`pegasus`). Its manifest
requires Jaunt 3.0.0-rc.3 or later; this integration adds no separate dependency.

## Wild versus mount forms

Only `wildpegasus-*` receives additional scarecrow reactions. The `feralpegasus`
and `pegasus` definitions inherit the wild base, but represent the intermediate
and tamed mount forms. They are explicitly excluded from the added behavior.
The upstream `tamed` tag appears even on the wild template, so it is not used
to decide which forms should react.

Wild adults get copies of their original injury-triggered melee attack and
charge/pursuit tasks. Wild adults and young get a copy of their injury-triggered
flee task. Original emotion conditions, priorities, damage, movement, sounds,
animations and ranges remain intact. This does **not** introduce unconditional
flight from scarecrows: the native player-target flee task requires
`fleeondamage`, and adult defense requires `aggressiveondamage`.

Four existing player-sensitive resting/sleeping lists also recognize the three
scarecrow types for wild forms only, retaining their native 20- or 1-block range.
Feral/tamed forms keep the original player/predator lists.

## Patch structure

The JSON-only patch is at `compatibility/pegasus/patches/wildpegasus.json`.
Pegasus stores its AI in `behaviorConfigs/taskai/aitasks`, not directly in the
server behavior entry. Three appended tasks target only `scarecrow`,
`little-scarecrow` and `strawdummy`; `enabledByType` prevents inherited tasks
from becoming active on feral/tamed forms or inappropriate young variants.
The four wake lists use `stopOnNearbyEntityCodesByType` with a wild-only branch
and an unchanged fallback, preventing inheritance from changing mount behavior.

Copies disable retaliation, and the flee copy disables instant flight on
unrelated damage. Existing player/predator tasks remain unchanged. Eggs,
following the mother, coming to the owner, following a lead holder, taming,
growth, breeding, equipment and Jaunt's riding/flight controls are unchanged.
No C# adapter or natural-spawn prefixes are added. The supplied Pegasus DLL
contains no entity-generation or AI-rewriting startup implementation.

## Validation

Automated checks apply all 11 shipped patch operations and compose both child
definitions with the base template. The game's actual variant resolver is used
for all **12 wild/feral/tamed, adult/young, male/female variants**. Checks cover
eight active additional task configurations and 28 disabled configurations,
wild-only wake extensions, and preservation of original resolved AI settings.
Build passes; the author completed the in-game test successfully.

Suggested checks: wild adults' injury defense and fear, wild young's fear,
sleep/rest interruption near both scarecrow sizes and the straw dummy, and
unchanged behavior while riding or leading feral/tamed Pegasi. Ordinary nearby
placement alone is not expected to trigger the injury-dependent tasks.
