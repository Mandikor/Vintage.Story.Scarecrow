# Lantern compatibility

Use **Shift + right-click** to equip a lantern, or the same control with an empty
hand to remove one. The full-size scarecrow accepts small/large vanilla lanterns
and supported More Lanterns lanterns in either arm instead of a tool. Its neck
no longer accepts new equipment. The little scarecrow keeps its existing neck
slot for both vanilla and supported More Lanterns lanterns.

## Wind animation and existing inventories

Arm lanterns swing as rigid bodies around their ceiling-model suspension points,
using independent wind/damping state per arm. Upright mounts compensate for arm
angles. The animation works with and without Immersive Lanterns and does not
create block entities or alter placed lanterns. Private rendering suppresses
shader wind so motion is not applied twice. Small/basic paper models keep scale
0.4; other paper and metal models use 0.6, as do vanilla ceiling models.

Vanilla uses its own `BlockLantern.GenMesh` texture provider, preserving the
stack's metal, lining and glass. Changing an arm's stack refreshes its mesh even
when the block code is unchanged. Shadows reuse the current swinging transform;
meshes are released on equipment changes, retessellation, death and disposal.
The little scarecrow's neck lantern remains rendered by its previous method.

The full-size inventory retains its old neck index for save compatibility.
Existing neck lanterns move to the first empty arm on server loading or packed
item restoration. Occupied arms are never overwritten: if both are full, the
legacy lantern stays removable at the neck with its radius bonus preserved.
New neck insertion is denied. No equipment is dropped or discarded by migration.

| Lantern | Natural-spawn radius bonus |
| --- | ---: |
| Small vanilla lantern | +2 blocks |
| Large vanilla lantern | +4 blocks |
| More Lanterns paper lantern | +2 blocks |
| More Lanterns metal lantern | +4 blocks |

The bonus applies while the lantern is attached, both during the day and at night.
More Lanterns' on/off states and brightness variants receive the same bonus.
When several lanterns are attached, only the strongest bonus applies, up to four
blocks. Removing it uses the strongest remaining lantern, or restores the base
radius if none remain. The enabled radius outline follows
the current bonus automatically; **Ctrl + H** continues to switch the outline on
and off. Flee and attack distances are unchanged. The bonus affects the configured
natural-spawn protection list, which contains hares and raccoons by default.

With Wind Chimes installed, an attached wind chime's **+6-block** bonus takes
precedence over lanterns. Equipment bonuses still do not stack.

## More Lanterns by Ape

Compatibility was checked against the supplied **More Lanterns 1.4.4** package
(`apelanterns`, packaged for Vintage Story 1.22.7). The mod is optional.

Supported models are the metal and paper lanterns that can be mounted on walls
or ceilings:

| Family | Models |
| --- | --- |
| Metal | Modern 1, Modern 2, Gothic 1, Medieval 2, Medieval 3, Medieval 4, Medieval 6 |
| Paper | Basic, Chinese, Chinese 2, Medieval 1 Small |

Use their normal **upright inventory variants**, as with vanilla lanterns. All
available colors, on/off states and metal-lantern brightness variants are supported.
The attachment uses the model's hanging shape and original variant textures.
Chinese paper lanterns and metal lanterns use larger mounts than the Basic and
Medieval 1 Small paper models. Arm mounts preserve the existing tool attachment
points and keep each lantern's textures separate when several are equipped.
Their rotations compensate for the arms' built-in angles so the lanterns hang
vertically in the resting pose, with their hanging points and sizes preserved.
Floating lanterns, stone lanterns and unfinished lantern parts are not accepted.

The four JSON patch files live in
`assets/scarecrow/compatibility/apelanterns/patches`. They add the little neck and full-size arm-slot
whitelists, attachment metadata, radius bonuses and model mounts. The hanging
shapes and textures are referenced from More Lanterns directly; its geometry and
textures are not copied into Scarecrow. Shape patches add attachment-parent metadata
and remove three redundant internal parent references, preserving world geometry.
The existing attachment categories of other equipment remain unchanged.

Automated checks resolve 768 valid lantern variants, including the 128 upright
inventory variants, using the game's variant resolver. They check attachment
acceptance and exclusions for both scarecrow sizes and the full-size arm slots,
radius bonuses, removal and non-stacking,
preservation of handbook and texture settings, and attachment of all eleven native
models at their expected scales. Matrix checks verify vertical arm alignment and
unchanged hanging points at both scales. In-game testing also confirmed the neck
attachments, adjusted model sizes, arm mounts and corrected vertical rotation.

The new arm-only/wind-animation revision additionally checks both vanilla sizes
in all six orientations, native category preservation, blocked full-size neck
insertion, retained little-neck acceptance, fixed swing pivots and lossless
legacy migration (left arm, right arm and both occupied). Build passes; the author
completed the revised attachment's in-game test successfully. Earlier game tests
above refer to the previous attachment rules.

The first arm-animation game test confirmed equipping and wind motion but exposed
duplicate static/animated lanterns. Static-mesh exclusion now checks the actual
inventory slot: the engine initially passes `default`, and only the base
attachable behavior resolves it to the configured arm code. The corrected
render exclusion passed the author's gameplay retest; neck lantern rendering is retained.

[More Lanterns source repository](https://github.com/Monkeybrain73/morelanterns)
