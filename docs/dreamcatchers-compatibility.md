# Dreamcatchers compatibility

Attachment baseline: supplied **Dreamcatchers 1.0.2** (`dreamcatchers`) by AiyonaMurakami.
The original manifest targets Vintage Story 1.17.3; attachment checks use the
currently installed game API. The author completed the in-game test successfully.

## Known upstream loading blocker

In the supplied **1.0.31** package, `assets/dreamcatchers/itemtypes/bead.json` contains model data
(`editor`, `textureWidth`, `elements`) instead of an item definition; it has no
item `code`. The author reports that the game does not load with this package.
The malformed asset was confirmed by inspection. This is an upstream packaging
error, not caused by the Scarecrow attachment patches. The author confirmed that
the older **1.0.2** package loads successfully. The attachment integration now
targets that working version; 1.0.31 remains unsuitable for gameplay validation.
The supplied mod files have not been modified or repaired by this integration.

## Corrected invisible attachment in 1.0.2

The initial patches used 1.0.31's `block/dreamcatcherwall` model path. The 1.0.2
package instead provides `block/dreamcatcher`, with different feather/yarn
texture keys. The game log explicitly reported the missing attachment shape
and that it would be invisible. Attachment metadata and step-parent patches now
reference the actual 1.0.2 model, retaining its native textures. Automated checks
are run against that package rather than the incompatible 1.0.31 assets.

## Effects review

Both reviewed packages are content-only and contain no DLL. Their dreamcatcher block has
only `HorizontalAttachable` and `RightClickPickup` behaviors. There is no light
emission, temporal-stability effect, storm protection, entity deterrent or
spawn-blocking logic in the supplied assets. The evocative manifest description
does not define a gameplay effect. Compatibility therefore remains decorative:
no radius bonus or invented protection is added.

## Arm attachment

Finished `dreamcatchers:dreamcatcher-*` blocks can occupy either arm slot of the
full-size Scarecrow, including both arms simultaneously. Existing interaction,
ownership, inventory preservation and removal rules apply. This uses the two
existing equipment slots, so a dreamcatcher replaces the equipment in that arm.
The little scarecrow has no arm slots and is unchanged. Crafting components and
tools are not added to the whitelist.

All 13 woods, ten primary colors, ten secondary colors and four wall directions
are covered: **5,200 block variants**. Attachments use the original wall model
in a consistent orientation, independent of its stored wall direction. Original
textures provide wood and cloth colors.

Dedicated invisible mounts counteract the arms' rotations. They use scale 0.6
and align the model's top-center with the existing arm hanging position. Slot
prefixes keep two attached copies distinct. The upstream model receives only
step-parent metadata, leaving its geometry and ordinary wall placement intact.

All JSON changes are in `compatibility/dreamcatchers/patches/`: block attachment
metadata, arm permissions/mapping and shape mounts. No C# integration is needed.

## Validation

Automated checks use the actual game variant resolver and attachment interfaces
for all 5,200 variants, verify unchanged original block behavior/textures and
world-model geometry, and attach the model to both arm mounts with the game's
step-parent mechanism. Matrix checks verify upright mounts and preserved hanging
points in both shape-transform versions. Build passes. The author confirmed
1.0.2 loads, but its initial attachment was invisible due to the wrong model path.
The author completed the in-game retest of the corrected 1.0.2 attachment successfully. These checks
cover attachment assets, not successful loading of the broken 1.0.31 package.

Test a dreamcatcher on each arm and two simultaneously, with several color/wood
variants and alongside a lantern or wind chime in the other arm. Confirm scale,
vertical alignment, removal, pickup inventory preservation and normal wall
placement. These visible checks are not replaced by the automated geometry tests.
