# Scarecrow attachments: modder's guide

I use the game's attachment system together with per-slot code filters, optional
arm transforms and wind-driven rendering. This guide documents the complete
attachment workflow on the current `dev` branch. It describes development features;
an older release ZIP may not include them.

## Contents

- [Slots and responsibilities](#slots-and-responsibilities)
- [Complete integration example](#complete-integration-example)
- [Models, textures and variants](#models-textures-and-variants)
- [Arm transforms](#arm-transforms)
- [Neck attachments](#neck-attachments)
- [The in-game transform editor](#the-in-game-transform-editor)
- [Pendulum motion](#pendulum-motion)
- [Radius bonuses](#radius-bonuses)
- [Equipment information and handbook](#equipment-information-and-handbook)
- [Specialized integrations](#specialized-integrations)
- [Interaction and persistence](#interaction-and-persistence)
- [Validation and troubleshooting](#validation-and-troubleshooting)

## Slots and responsibilities

Four independent pieces make an attachment work:

1. The entity's `allowedCodes` permits its collectible code at that slot.
2. `attachableToEntity` supplies attachment geometry and native category handling.
3. Optional arm transforms adjust the resting position, rotation and size.
4. Optional pendulum metadata enables wind motion. A separate attribute grants a
   natural-spawn radius bonus when appropriate.

Neither a transform nor the pendulum switch makes an item attachable by itself.
These are collectible JSON attributes, not callbacks named `onScarecrowTransform`.
No C# subclass or hard dependency is necessary for a normal item/block attachment.

| Entity | Slot index | Stable slot code | Selection attachment point | Default shape element |
| --- | ---: | --- | --- | --- |
| Full-size scarecrow, standard and Plumpkins model | 0 | `sc-leftarm` | `LArmAP` | `LArmStorage` |
| Full-size scarecrow, both models | 1 | `sc-rightarm` | `RArmAP` | `ScarecrowToolRightMount` |
| Full-size scarecrow, legacy contents only | 2 | `sc-neck` | `NeckAP` | `NeckStorage` |
| Full-size scarecrow, both models | 3 | `sc-foot` | `FootAP` | `FootStorage` |
| Little scarecrow | 0 | `sc-neck` | `NeckAP` | `NeckStorage` |
| Little scarecrow | 1 | `sc-foot` | `FootAP` | `FootStorage` |

The little scarecrow has no arms. The vanilla straw dummy has no equipment slots.
Full-size neck insertion is disabled; old neck lanterns are migrated to the first
free arm. If both arms are occupied, the old lantern remains safely removable.
Do not repurpose inventory indices: saved contents use stable semantic slot codes.
Selection-box order must match `wearableSlots` order. An attachment point, a shape
element and a slot code are different identifiers.

Entity assets are `scarecrow:entities/nonliving/scarecrow.json` and
`scarecrow:entities/nonliving/little-scarecrow.json`. Rules live under
`behaviorConfigs/scarecrowattachments/wearableSlots`, not in `ModConfig/scarecrow.json`.

## Complete integration example

This example assumes an existing item `mymod:gardencharm` defined in
`mymod:itemtypes/gardencharm.json`, with an existing original shape at
`assets/mymod/shapes/item/gardencharm.json`. Replace every `mymod` name and model
path with your own. This is an attachment patch, not a complete item definition.
The example enables both full-size arms without changing the little scarecrow.

### 1. Permit the collectible on both arms

```json
[
  {
    "file": "scarecrow:entities/nonliving/scarecrow.json",
    "op": "addmerge",
    "path": "/behaviorConfigs/scarecrowattachments/wearableSlots/0/allowedCodes/-",
    "value": "mymod:gardencharm"
  },
  {
    "file": "scarecrow:entities/nonliving/scarecrow.json",
    "op": "addmerge",
    "path": "/behaviorConfigs/scarecrowattachments/wearableSlots/1/allowedCodes/-",
    "value": "mymod:gardencharm"
  }
]
```

Use `mymod:gardencharm-*` if the actual collectible codes have variant suffixes.
An exact code does not include those variants. Always qualify the domain so a
similarly named item in another mod is not accidentally accepted. These lists use
the game's `WildcardUtil`, independently of the natural-spawn configuration rules.
Append entries; replacing the entire list removes other integrations' permissions.

### 2. Supply geometry, one shared transform and every pendulum setting

```json
[
  {
    "file": "mymod:itemtypes/gardencharm.json",
    "op": "addmerge",
    "path": "/attributes",
    "value": {
      "attachableToEntity": {
        "attachedShapeBySlotCode": {
          "sc-leftarm": { "base": "mymod:item/gardencharm" },
          "sc-rightarm": { "base": "mymod:item/gardencharm" }
        },
        "texturePrefixCode": "scarecrow-mymod-gardencharm"
      },
      "onScarecrowTransform": {
        "translation": { "x": 0.0, "y": 0.1, "z": 0.0 },
        "rotation": { "x": 0.0, "y": 0.0, "z": 15.0 },
        "origin": { "x": 0.5, "y": 0.5, "z": 0.5 },
        "scale": 0.8
      },
      "scarecrowPendulum": true,
      "scarecrowPendulumPivot": [0.0, 0.0, 0.0],
      "scarecrowPendulumAttachmentPoints": {
        "sc-leftarm": "ScarecrowArmLeftOriginAP",
        "sc-rightarm": "ScarecrowArmRightOriginAP"
      }
    }
  }
]
```

The numbers are illustrative starting values, not calibrated positions for an
arbitrary model. Set `scarecrowPendulum` to `false` for a stationary tool. For an
ordinary arm model, the pivot and point mappings shown explicitly repeat defaults;
the boolean alone is sufficient to enable swinging.

Keep any existing `categoryCode`, other animals' shapes and native properties.
Scarecrow derives accepted slot categories from permitted collectibles. It only
supplies fallback category `scarecrowattachment` when an otherwise uncategorized
collectible has `sc-` attachment shapes and no custom attachment interface. Do not
overwrite a category such as `tool` or `utilities` merely to equip this entity.
If a collectible implements its own `IAttachableToEntity`, inspect that interface's
behavior: JSON alone may not override its custom model/category decisions.

### 3. Package optional patches

Within Scarecrow, integrations follow
`scarecrow/assets/scarecrow/compatibility/<manifest-modId>/patches/`. The manifest
ID selects the optional integration; asset domains inside `file` and `base` may
differ from that ID. For example, Arctic Wolves uses manifest `varasarcticwolves`
and assets `arcticwolves`. In your own mod, use its normal patch folder and an
appropriate Scarecrow dependency/conditional-loading arrangement. Avoid applying
patches to absent target assets. Install the integration on server and clients.

### Group operations by target

Keep additions to the same asset and object together in one `addmerge` operation,
as in the complete `/attributes` example above. Batch additions to an existing
array by targeting the array itself with an array value:

```json
[
  {
    "file": "scarecrow:entities/nonliving/scarecrow.json",
    "op": "addmerge",
    "path": "/behaviorConfigs/scarecrowattachments/wearableSlots/0/allowedCodes",
    "value": ["mymod:gardencharm", "mymod:othertool-*"]
  }
]
```

Use the parent array path when appending several entries this way. Keeping `/-`
while supplying an array would append that entire array as one nested element.
Task lists use the same parent-array pattern, with task objects as array entries.
Existing entries and the order of the new entries must remain intact.

Do not force every change into a single operation: client/server sides and other
conditions must agree. Updates to distinct existing array indices cannot be
represented as object keys in a merged value. Keep dependent removals/replacements
in order and retain the optional compatibility folder of each manifest mod ID.

## Models, textures and variants

`attachedShapeBySlotCode` maps stable slot codes to shape references. `base` is a
shape asset location without `shapes/` or `.json`; `file` in a patch is the full
definition asset path including `.json`. Original tool models can be reused: a
separate attachment shape is not required just to change position or scale.
Special geometry or custom suspension may still justify a dedicated shape.

The engine attaches geometry to the slot's configured `stepParentTo` frame.
Preserve required step parents and provide corresponding elements on both the
standard and Plumpkins entity shapes when creating special mounts. Changes to a
world model affect all its uses; prefer a private attachment model when needed.

Textures still come through the collectible's native attachment pipeline. Use
distinct `texturePrefixCode` values to avoid collisions between different models
or materials, and retain existing variant substitutions. All texture codes in the
selected shape must resolve. A ceiling model may require rope or other textures
that the inventory/floor variant does not supply. A transform cannot repair a
missing texture, a wrong model path or an absent step parent.

For variant definitions, the native system also supports
`attachableToEntityByType` and `attachedShapeBySlotCodeByType`. Existing hoes and
lanterns demonstrate these patterns. Patch the source branch actually present:
some mods use `attributesByType` instead of root `attributes`. A child path below
a nonexistent `/attributes` fails before variant resolution. For example, for an
existing `*-up` branch, merge attributes into `/attributesByType/*-up`:

```json
[
  {
    "file": "mymod:blocktypes/gardencharm.json",
    "op": "addmerge",
    "path": "/attributesByType/*-up",
    "value": {
      "scarecrowPendulum": true,
      "onScarecrowTransform": {
        "translation": { "x": 0, "y": 0, "z": 0 },
        "rotation": { "x": 0, "y": 0, "z": 0 },
        "origin": { "x": 0.5, "y": 0.5, "z": 0.5 },
        "scale": 1
      }
    }
  }
]
```

This variant example assumes its geometry and permissions are already configured;
the branch name must match the actual source definition. Preserve other branches.
Patches follow the target asset's load side. Existing vanilla tool patches use
`"side": "server"`; do not limit entity metadata to an isolated client copy and
assume it replaces the server-loaded entity type.

## Arm transforms

Use **one** collectible attribute: `onScarecrowTransform`. Author it for the left
arm; the right arm receives the mirrored adjustment automatically. This is JSON
metadata, not a method or callback. There are no separate arm overrides.

Place this object inside the collectible's `attributes`, or merge it through a
compatibility patch into the appropriate root/by-type attributes branch:

```json
{
  "onScarecrowTransform": {
    "translation": { "x": 0.06, "y": 0.05, "z": 0.01 },
    "rotation": { "x": 0, "y": -4, "z": 0 },
    "origin": { "x": 0.5, "y": 0.5, "z": 0.5 },
    "scale": 0.9
  }
}
```

The format is native `ModelTransform` JSON:

| Field | Meaning | Omitted value |
| --- | --- | --- |
| `translation` | `{ "x", "y", "z" }` displacement in block units | Zero |
| `rotation` | `{ "x", "y", "z" }` angles in degrees | Zero |
| `origin` | `{ "x", "y", "z" }` transform origin in block units | `(0.5, 0.5, 0.5)` |
| `scale` | Uniform scale | One |
| `scaleXYZ` | Independent axis scaling | Native uniform-scale fallback |

One block equals 16 shape units. For nonuniform scaling use, for example,
`"scaleXYZ": { "x": 0.8, "y": 1, "z": 0.8 }` instead of `scale`. Keep authored
values to at most two decimal places for readable patches; runtime/editor
calculations retain full numeric precision.

### Mirroring

The shared adjustment is reflected at **local `x = 0.5`** in the model's block-unit
frame. Default tool-frame attachments additionally reflect their private right-arm
mesh at that plane, so asymmetric tool heads point in opposite directions rather
than merely sharing a mirrored offset. Triangle winding, shader normals and face
directions are corrected; original world/inventory models and UV values stay
unchanged. Hanging lights, Wind Chimes and Dreamcatchers retain their existing
model orientation. The resulting right-arm transform settings are:

| Field | Right arm from shared left-arm settings |
| --- | --- |
| `translation` | `(-x, y, z)` |
| `rotation` | `(x, -y, -z)` degrees |
| `origin` | `(1 - x, y, z)` |
| `scale` / `scaleXYZ` | Unchanged |

Shared translation `(0.06, 0.05, 0.01)` and rotation `(0, -4, 0)` therefore become
`(-0.06, 0.05, 0.01)` and `(0, 4, 0)` on the right. Z displacement does not change
sign. Default origin `(0.5, 0.5, 0.5)` remains unchanged. Missing fields receive
native defaults before reflection; a neutral transform stays neutral on both arms.

The former `onScarecrowLeftArmTransform` and `onScarecrowRightArmTransform`
attributes are no longer read and cannot block the shared setting. Remove them
from integrations and retain only `onScarecrowTransform`. To migrate a right-arm
calibration, apply the same mirroring formulas once to express it for the left
arm, then store that result as the shared transform. The right-arm bug-net
calibration has been transferred this way; the left arm now uses its mirrored pose.
If the shared attribute is absent, no additional transform is applied, even when
old individual-arm keys remain. Malformed or non-finite shared values are ignored.

Coordinates are relative to the attachment frame, not world axes. Static tools
use their step-parent frame; animated lights use their mount/model frame. Both
standard and Plumpkins hanging mounts have matching arm axis orientations.
The right tool slot uses `ScarecrowToolRightMount`, a corrected child of the
historically rotated `RArmStorage`. Its pose point `ScarecrowArmRightOriginAP`
is on that child, so tools at rest and tools with pendulum motion use the
same corrected frame. The left tool frame stays on `LArmStorage`. Default-frame
tools use private cached attachment meshes even with pendulum motion disabled,
allowing the right mesh to be reflected independently of the body mesh.

Do not rotate `RArmStorage` itself to correct tools: the independent hanging
mounts below it already compensate for its authored orientation. The tool child
normalizes the right frame without moving working lights, chimes or Dreamcatchers.
Test both arms for asymmetric original meshes, which are not normalized to a
universal grip point. Automatic tool-mesh reflection applies when both pose-point
names resolve to the standard tool origin points; custom pose-point mappings
retain their own authored model orientation.

Matrix order is native `T * O * Rx * Ry * Rz * S * -O`. It is applied before wind
motion, including shadows and supported following particle emitters. Body geometry,
other equipment and shared source shapes stay unchanged. Code editing the
attribute should replace its complete JSON object so the cache detects the change.

## Neck attachments

Neck equipment needs a `sc-neck` model in `attachedShapeBySlotCode` and permission
in the little scarecrow's slot 0 `allowedCodes`. Use the normal native attachment
pipeline and existing `NeckStorage`; no new shape mount is needed. Primitive
Survival demonstrates this with its wooden lanterns and filled Firefly Jars.

Optional `attributes.onScarecrowNeckTransform` uses the same native ModelTransform
format, defaults and finite-value validation as the arm setting. It affects only
newly attached neck geometry, without mirroring or applying the arm transform.
Existing neck items without this attribute keep their appearance. For example:

```json
{
  "onScarecrowNeckTransform": {
    "translation": { "x": -0.08, "y": -0.4, "z": -0.08 },
    "rotation": { "x": 0, "y": 0, "z": 0 },
    "origin": { "x": 0, "y": 0, "z": 0 },
    "scale": 0.4
  }
}
```

These illustrative wood-lantern values align a ceiling-model suspension point in
the neck's local frame; other models need their own values. Arm renderer offsets
and scales are not applied to neck models. Neck equipment uses the static entity
mesh and does not gain pendulum motion. Permissions, radius bonuses, ownership,
pickup and persistence remain separate from this visual metadata.

## The in-game transform editor

1. Equip an allowed item on either full-size model. Hold another copy of the same
   collectible/material variant in the active hotbar slot.
2. Open `.tfedit` and select **Scarecrow – both arms**. This is the only Scarecrow
   arm-transform target; its label is translated in all 33 languages.
3. Set the left-arm translation, rotation, origin and scale. Loaded scarecrows
   wearing the same collectible refresh locally on both arms; right is mirrored.
4. Copy the JSON into `onScarecrowTransform` in the source asset/patch, reload and
   verify persistence. Remove obsolete individual-arm entries from your patch.

For neck equipment, equip it on the little scarecrow, hold the same collectible
variant and select **Scarecrow – Neck**. Export the result into
`onScarecrowNeckTransform`. This is an independent neck target, not an additional
individual-arm override; changing it does not change either arm's calibration.

Preview is local development tooling, not a per-stack/server configuration editor.
All loaded copies of that collectible on this client can change. Other players
receive no preview, and closing the editor does not save source files. Native
reset/close behavior still applies. Pendulum flags, pivots and point names are
edited directly in JSON. Editing a transform does not grant attachment permission.

## Pendulum motion

The generic whole-model pendulum supports both items and blocks on the two
full-size arms, including the Plumpkins model. Each arm moves independently with
local wind and damped follow-through. The little scarecrow's neck and placed world
blocks are outside this feature.

| Attribute | Meaning | Default |
| --- | --- | --- |
| `scarecrowPendulum` | Boolean controlling visual wind motion on both arms | Legacy hanging lights/decorations and Wind Chimes enabled; ordinary models disabled |
| `scarecrowPendulumPivot` | Three finite numbers `[x,y,z]`, model-frame block units before arm transform | Generic models `[0,0,0]`; legacy hangers `[0.5,1,0.5]` |
| `scarecrowPendulumAttachmentPoints` | Object mapping `sc-leftarm`/`sc-rightarm` to pose-point names in entity shape | Generic `ScarecrowArmLeftOriginAP` / `ScarecrowArmRightOriginAP` |

Explicit `true` opts a generic model into separate animated rendering; explicit
`false` keeps it still. Missing boolean preserves prior behavior. Current tool,
Walking Stick and Dreamcatcher patches default to false; existing lanterns, oil
lamps, censers, Firefly Jars and wind chimes default to true.

Generic models use their corrected tool-frame origin at full scale before the shared arm
transform. Legacy hanging lights retain authored scale, offsets and ceiling frame.
For a legacy hanging light, omit the pivot or explicitly use `[0.5,1,0.5]` rather
than copying the generic example's zero pivot.

The pendulum pivot and `ModelTransform.origin` serve different operations: the
first sets the swing pivot, the second sets the resting transform's origin. Use
the arm transforms for rest alignment, not the swing pivot to compensate for an
incorrect model placement.
For default tool-frame attachments, author the pivot for the left model; the
right tool mesh and its pivot are both reflected (`x` becomes `1 - x`). Hanging
lights and custom pose-point models retain their existing pivot handling.

For a custom mount, provide real pose points in both entity shapes and map them:

```json
{
  "scarecrowPendulum": true,
  "scarecrowPendulumPivot": [0.5, 1.0, 0.5],
  "scarecrowPendulumAttachmentPoints": {
    "sc-leftarm": "MyLeftMountAP",
    "sc-rightarm": "MyRightMountAP"
  }
}
```

This is an `attributes` fragment; it does not create the named points. Missing
mapping entries use generic defaults, but a supplied nonexistent point is not a
replacement for authoring the mount. Dreamcatchers' matching points are
the shared `ScarecrowLanternLeftAP` and `ScarecrowLanternRightAP`; its previous
model-specific mount size and displacement now live in the shared transform.

Legacy hangers marked with `scarecrowArmLantern` or `scarecrowHangingDecoration`
use dedicated `ScarecrowLanternLeftAP` / `ScarecrowLanternRightAP` points and ignore
point overrides. When disabled they retain their private renderer at rest, with
light/textures/particles. Default-frame tools also retain their private renderer
at rest, including the reflected right model. The switch controls motion only.

Wind Chimes uses model-specific split meshes and individual part pivots. The
boolean freezes/enables those parts; whole-model pivot and point overrides do
not apply. Sound and radius bonuses remain active when motion is disabled.
Its animation also works with Immersive Lanterns; private attachment copies
suppress shader wind to avoid duplicate motion. This does not fix unrelated
placed-block issues in that mod combination.

Both full-size entity shape files now contain the shared hanging mounts and arm
origin points directly. No runtime shape patch is needed to create them. All
supported hanging mod attachments use `ScarecrowLanternLeftAP` /
`ScarecrowLanternRightAP`, retaining these established names for compatibility.
Wind Chimes now uses these points too, with its former mount scale folded into
the arm transforms. Generic tools retain the storage-origin points because their
grip frame differs from a ceiling-hung model. Item-specific placement belongs in
the transform attributes instead of additional per-mod arm mount elements.
The little scarecrow's neck mounts and their optional compatibility patches are
unchanged. Patches to foreign models may still be required for step-parent and
texture compatibility; only duplicate owned arm infrastructure was removed.

There are no public JSON controls for wind strength multiplier, damping,
frequency, maximum angle or independent left/right motion booleans. The three
pendulum attributes above are the exposed controls. Multiple unrelated step
parents, custom mesh suppliers and arbitrary animated models require individual
review; enabling a flag cannot reproduce their specialized rendering logic.

## Radius bonuses

Optional integer collectible attribute:

```json
{
  "scarecrowRadiusBonus": 2
}
```

Place it in resolved collectible `attributes`. Values are clamped to **0–6**.
Only the strongest allowed equipped bonus counts, never a sum. Eligible slots
are arms/neck, not foot. Vanilla upright small/large lanterns have built-in
bonuses of two/four blocks; other supported items use this attribute. Legacy full-size
neck hangers retain their bonus while awaiting removal/migration.

This extends only configured **natural-spawn blocking**, not flee/attack task
distances, damage, player abilities or an automatic general protection aura.
The radius tooltip/outline follows equipment changes; Ctrl+H retains its toggle.
Motion is independent: a stationary attachment may still grant a bonus. Existing
integrations use paper/wood lanterns +2, metal/large lanterns +4, oil lamps/filled
Firefly Jars +1 and Wind Chimes +6. Decorative Dreamcatchers, censers and Walking
Sticks have no bonus. Add a bonus deliberately, not because a model is equippable.

## Equipment information and handbook

I derive the equipment description from the resolved `scarecrowattachments`
slot `allowedCodes` and the collectible's native attachment category. It uses
the same radius-bonus calculation as placed scarecrows and the existing
`scarecrowPendulum` rules. An arm transform alone does not advertise an item as
attachable. The large alternate pumpkin shape shares the large entity's slots.
No separate tooltip patch or second item list is needed for a supported item.

Only the client installs the description behavior and appends a section to the
native handbook metadata, preserving existing sections and collectible behavior
instances. The guide uses native `handbook://` item-page links. Itemstack
attributes, source models, vanilla categories and server permissions are not
modified. The optional inventory section is controlled by the personal
`ShowEquipmentTooltips` setting in `ModConfig/scarecrow-client.json` (default
`false`; restart after editing). Handbook equipment details remain available
with either preference. All new labels and the guide are translated in the 33
existing language files.

The equipment behavior stores the resolved information; it does not insert
text during `GetHeldItemInfo`, because item subclasses may append their own
description after that callback. When inventory tooltips are enabled, a
client-scoped Harmony postfix appends the section to the completed
`ItemStack.GetDescription` result. It leaves server descriptions unchanged and
is removed when the client mod system is disposed.

## Specialized integrations

These existing attributes identify specialized paths; they are not universal
shortcuts for making any collectible act like another mod:

- `scarecrowArmLantern`: block-based legacy hanging-light rendering.
- `scarecrowHangingDecoration`: block-based legacy hanging-decoration rendering.
- `scarecrowWindChime`: specialized Wind Chimes handling, including split geometry
  and optional sound. Sound uses the mod's original `heldIdleSound`, volume/range
  and cover settings; the Wind Chimes dependency is checked before audio work.

Inspect existing integrations before using these markers. Ceiling texture
selection, shape offsets, part naming, flames/smoke and material sounds may need
dedicated support. Generic pendulum `true` does not automatically execute a
block entity, emit arbitrary block particles, play item abilities or add audio.
Native light/texture handling is preserved by supported rendering paths; custom
effects require review. Existing oil lamps/censers have private particle copies
that follow the animated model without modifying world-block particle data.

The whole-model renderer also reads these advanced attributes. They are not
additional wind-strength controls and do not run on the ordinary static path:

| Attribute | Format / default | Effect and limits |
| --- | --- | --- |
| `scarecrowArmLanternScale` | Number; legacy hanger `0.6`, generic model `1` | Additional whole-model renderer scale, separate from the shared arm transform |
| `scarecrowArmLanternOffset` | Three-number array; no offset | Translates the private mesh in block units before motion; used to align Firefly Jar corks |
| `scarecrowUseCeilingTextures` | Boolean; false | Blocks only: looks up the corresponding `-down` texture source by replacing the final `up` of an inventory code. Only use with that actual orientation naming scheme; falls back to the original collectible if no block resolves |
| `scarecrowCenserSmoke` | Boolean; false | Blocks only and only with `censership` active: enables its specialized smoke adapter using original particle profiles |
| `scarecrowOilLampParticles` | Boolean; false | Blocks only: original particle profiles follow the model's `Flame` element pivot, with an oil-lamp fallback position when missing |

For example, an existing legacy hanging block with suitable geometry can retain
its ceiling frame explicitly with this resolved `attributes` fragment:

```json
{
  "scarecrowHangingDecoration": true,
  "scarecrowArmLanternScale": 0.6,
  "scarecrowArmLanternOffset": [0.0, 0.0, 0.0],
  "scarecrowUseCeilingTextures": false,
  "scarecrowCenserSmoke": false,
  "scarecrowOilLampParticles": false,
  "scarecrowPendulum": true,
  "scarecrowPendulumPivot": [0.5, 1.0, 0.5]
}
```

This fragment assumes existing permission, attachment shapes, textures and the
legacy lantern mounts. Do not mark a generic tool as a legacy hanger merely to
enable swinging. Supply sensible finite values for advanced scale/offset metadata;
the transform validator is not a general validator for every renderer attribute.

Useful working references in this repository:

My game test confirmed mirrored and aligned bug nets, snow shovels, scythes,
hoes, hayforks, rakes and the retained Walking Sticks. Walking Stick support now
excludes the spear-style Blackguard and Forlorn, which could not be attached with
Shift + right-click; 16 definitions/28 variants remain. Existing calibrated values
in the patches are starting points to retain when extending these integrations.

- [Vanilla tools](scarecrow/assets/scarecrow/patches/wearables/itemtypes-tool-vanilla.json)
- [Walking Stick integration](scarecrow/assets/scarecrow/compatibility/walkingstick/patches)
- [Dreamcatchers integration](scarecrow/assets/scarecrow/compatibility/dreamcatchers/patches)
- [More Lanterns integration](scarecrow/assets/scarecrow/compatibility/apelanterns/patches)
- [Wind Chimes integration](scarecrow/assets/scarecrow/compatibility/windchimes/patches)
- [Shared attachment behavior](scarecrow/src/Wearables/ScarecrowAttachments.cs)
- [Arm transform implementation](scarecrow/src/Wearables/ScarecrowArmTransform.cs)
- [Pendulum metadata implementation](scarecrow/src/Wearables/ScarecrowPendulum.cs)

## Interaction and persistence

The server configuration can override the three built-in base protection radii
through `ScarecrowRadius`, `LittleScarecrowRadius` and `StrawDummyRadius`.
`/scarecrow reload` applies them to existing entities and synchronizes client
tooltips and outlines. Equipment bonuses are added afterward. These settings
do not change JSON-patched AI distances. Other entity codes reusing
`EntityScarecrow` retain their asset-defined `scareRadius`.

One item occupies each slot. Shift + right-click equips a permitted held item;
Shift + empty hand removes individual equipment. Owner-UID and claim checks apply
to attaching, removing and targeted pickup, including server packet handling.
The owner can grant per-entity `equipment` access for gear changes or `full` access
for gear changes and pickup through `/scarecrow access`. Delegates cannot manage
grants. The server checks claim rights in addition to grants. Grants use player
UIDs and persist with the entity; they are not copied into the pickup item.
Existing ownerless entities retain claim-based access. Unsupported items or failed
placement do not consume the held item.

Ctrl + empty hand picks up the scarecrow with equipment stored by semantic slot
code in `scarecrowEquipment`. Placement restores item attributes and durability.
Only empty scarecrows stack; equipped ones remain individual items. Pickup prefers
the hotbar, then backpack, then drops the packed item if inventory is full.
Damage death drops equipment separately, clears its display and stops protection.
Existing equipment remains removable after a whitelist change. Do not rewrite
saved slot codes or reorder slots to implement visual changes.

## Validation and troubleshooting

1. Verify patch target paths, existing parent objects and variant branches against
   the actual installed source version. Check startup logs for patch failures.
2. Test left and right separately and together on standard and Plumpkins models.
   Check every intended material/orientation; exclude raw, empty or placed variants
   unless specifically supported.
3. Confirm original categories and other animals'/stands' attachments still work.
4. Inspect resting pose, light, textures, shadows and supported particles. Test
   calm/strong wind, motion true/false and absence of optional mods.
5. Save exported transforms, reload and verify persistence. Test removal, packed
   pickup/replacement, inventory tooltips and damage drops.
6. On a server, test owner/nonowner and claim denial. Confirm radius bonus uses
   strongest-only rules and does not change animal combat ranges.

| Symptom | Check |
| --- | --- |
| Cannot attach | Full domain-qualified allowed code, correct slot, native attachment category/interface, ownership and claim rights |
| Equips but invisible | Resolved shape path, slot mapping, step parent, texture codes and active renderer |
| Doubled static and moving model | Special renderer must omit that slot from static entity mesh; do not add a second independent renderer |
| Wrong angle or size | Local arm frame and independent transform; hanging objects may need counter-rotation |
| Wrong swing center | Generic versus legacy pivot, block units and actual pose-point mapping |
| Preview lost after reload | Export JSON and save it to the source asset/patch, including correct by-type branch |
| No bonus | Resolved integer attribute, allowed arm/neck slot, 0–6 clamp and strongest-only rule |
| No particles or sound on a new item | Generic pendulum is visual; specialized effects are not automatically inherited |

See [README](README.md) for player-facing controls/configuration and
[CHANGELOG](CHANGELOG.md) for when features were introduced.
