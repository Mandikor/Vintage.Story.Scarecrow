# Lantern compatibility

Both scarecrow sizes accept lanterns at their neck attachment point using
**Shift + right-click**. Use an empty hand with the same control to remove one.
The full-size scarecrow also accepts More Lanterns paper and metal lanterns in
either arm slot. Each arm holds a tool or a lantern. The little scarecrow retains
its single lantern slot at the neck.

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
`assets/scarecrow/compatibility/apelanterns/patches`. They add the neck and arm-slot
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

[More Lanterns source repository](https://github.com/Monkeybrain73/morelanterns)
