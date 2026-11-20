# Adventurer's Walking Stick compatibility

Baseline: walkingstick 3.0.12 (the local net10 package), Vintage Story 1.22.7.

## Scope

Both arms of the standard and Plumpkins full-size scarecrows accept the following
complete sticks, one per arm in place of another item:

- Standard, Crude, Fine, Blackthorn and Cow Skull.
- Architect, Blackguard, Forlorn, Gas, Jonas and Pathfinder.
- Reinforced, Shepherd's, Sling and Witch.
- Sheathed and Gun Sheathed (complete stick models).
- Lantern in all 13 original material variants.

This covers 18 item definitions and 30 generated inventory variants. Separate
`hidden-blade`, `hidden-gun`, `walkingstick-sheath` and `walkingstick-gun-sheath`
items are excluded. Use Shift + right-click to attach or remove equipment as
usual. The little scarecrow and neck/foot slots do not accept these items.

## Models and transforms

All patches are under `compatibility/walkingstick/patches`, in separate item files
plus the arm whitelist patch. They merge only Scarecrow metadata into existing
attributes; the original `utilities` category and riding-animal slot shapes remain
intact. Where no attachment category exists, Scarecrow supplies its normal missing
category fallback during asset finalization.

Both slots reference the item's original model. Per-arm transforms start neutral,
with `scarecrowPendulum: false`. Positions are deliberately uncalibrated: equip
a stick, hold another copy of the same variant, open `.tfedit`, select the relevant
Scarecrow arm, then copy the final transform into that item's patch. Reload and
verify the saved result. No dedicated Scarecrow stick shapes are created.

Pendulum motion can be enabled through the general boolean attribute after
alignment. See [arm transforms](scarecrow-arm-transforms.md) for pivots and frames.

No radius bonus is assigned, including to light-bearing sticks. Native item/light
metadata is retained. Player speed/hunger bonuses, charged abilities, minigames,
weapon functions and custom held-item visual/particle effects are not adapted to
the scarecrow. This integration renders the original base item models rather than
recreating arbitrary dynamic held-item rendering.

## Validation

Build passed. Isolated checks apply all 54 operations with the game's actual JSON
patch library, resolve all 30 variants with the game's by-type resolver, verify
original model files and native properties, retain 40 existing riding-slot
configurations, and test both arm slots and small/neck/foot/part/domain exclusions.
Default pendulum state and zero radius bonus are checked.

The author completed the in-game test successfully. Individual positions can
still be adjusted in the editor and saved in the per-item patches.
