# censership compatibility

Baseline: supplied **censership 1.0.6** (`censership`) by EliteJam.

## Supported hanging decorations

The full-size scarecrow accepts the normal upright inventory variants in either
arm, replacing a tool, lantern or other decoration:

| Family | Models | Materials |
| --- | --- | --- |
| `censerattachable` | Metal Open, Metal Gills, Metal Dome | Antique, gold, silver, tin bronze, bismuth bronze, black bronze, bismuth, brass, electrum, meteoric iron |
| `censercarryable` | Carryable censer | The same metals except antique |

There are **39 eligible inventory variants**. Attachment uses the original
ceiling model for each attachable style and the original `carryable2` model for
the portable family, with native material textures. Either arm can hold a censer,
and both can be equipped together. Use the existing Shift + right-click rules;
ownership, pickup inventory preservation and equipment drops remain unchanged.

Only `*-up` inventory variants are accepted. Wall/down block states, standalone
chains/caps, floor-only censers, incense bowls/sticks, candles, perfumed water,
unfinished clay objects and the disabled Tiangong definition are excluded.
Neither the little scarecrow nor the full-size legacy neck/foot slot accepts
these decorations.

## Effects and rendering

The reviewed hanging families use ordinary blocks with smoke particle profiles
and warm `lightHsv` emission. Their original definitions provide no animal
deterrence, temporal protection or other buff, so **no spawn-radius bonus** is
added. Unlike some other censership objects, these families have no fuel/burnout
state to simulate.

Arm censers reuse the independent rigid-body pendulum renderer at scale 0.6,
including with Immersive Lanterns installed. The renderer draws only one copy;
the normal entity mesh excludes these arm attachments. Light continues through
the existing equipment-light handling.

Smoke needs a small client-side adapter because placed-block particle callbacks
do not run for inventory attachments. It clones the matching `*-down` block's
original profiles for each equipped arm, preserving the authored smoke colors,
velocity, lifetime and other settings. Source offsets follow the moving model;
spread and particle size are scaled to the attachment. Private profiles avoid
changing particles on placed blocks or other scarecrows.

Emission is throttled, skipped while paused, limited to nearby visible models
and not advanced by shadow passes. Removing equipment or disposing its renderer
also removes its smoke adapter. No block-change events or placed block entities
are created by the integration.

## Files and checks

All three JSON patch files live in `compatibility/censership/patches/`; they add
arm permissions and optional attachment metadata while retaining original
handbook, textures, light and particle definitions. The particle adapter lives
in `src/combat/censership/`. The mod remains optional and no configuration
entries or migration are needed.

Automated checks use the actual game variant resolver for **234 model/material/
orientation combinations**, including all 39 accepted items and four original
models. Checks cover both arm slots, exclusions, zero bonus, unchanged source
properties, static-mesh exclusion and independent clones of ceiling smoke
profiles. Build passes; the author completed the in-game test successfully.

Test all four models on both arms, smoke position while swinging, warm light,
different metals together, mixed equipment, removal/pickup and the radius tooltip.
Visual scale, smoke appearance and light output require the gameplay test.
