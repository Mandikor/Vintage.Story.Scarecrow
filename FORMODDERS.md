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
- [The in-game transform editor](#the-in-game-transform-editor)
- [Pendulum motion](#pendulum-motion)
- [Radius bonuses](#radius-bonuses)
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
| Full-size scarecrow, both models | 1 | `sc-rightarm` | `RArmAP` | `RArmStorage` |
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

### 2. Supply geometry, both transforms and every pendulum setting

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
      "onScarecrowLeftArmTransform": {
        "translation": { "x": 0.0, "y": 0.1, "z": 0.0 },
        "rotation": { "x": 0.0, "y": 0.0, "z": 15.0 },
        "origin": { "x": 0.5, "y": 0.5, "z": 0.5 },
        "scale": 0.8
      },
      "onScarecrowRightArmTransform": {
        "translation": { "x": 0.0, "y": 0.1, "z": 0.0 },
        "rotation": { "x": 0.0, "y": 0.0, "z": -15.0 },
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
      "onScarecrowRightArmTransform": {
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

| Attribute | Scope |
| --- | --- |
| `onScarecrowLeftArmTransform` | `sc-leftarm` only |
| `onScarecrowRightArmTransform` | `sc-rightarm` only |

Both use native `ModelTransform` JSON:

| Field | Meaning | Omitted value |
| --- | --- | --- |
| `translation` | `{ "x", "y", "z" }` displacement in block units | Zero |
| `rotation` | `{ "x", "y", "z" }` angles in degrees | Zero |
| `origin` | `{ "x", "y", "z" }` transform origin in block units | `(0.5, 0.5, 0.5)` |
| `scale` | Uniform scale | One |
| `scaleXYZ` | Native vector for independent axis scaling | Native uniform-scale fallback |

One block equals 16 shape units. For nonuniform scaling, use `scaleXYZ` instead
of competing uniform and per-axis settings:

```json
{
  "onScarecrowRightArmTransform": {
    "translation": { "x": 0.0, "y": 0.125, "z": 0.0 },
    "rotation": { "x": 0.0, "y": 0.0, "z": -15.0 },
    "origin": { "x": 0.5, "y": 0.5, "z": 0.5 },
    "scaleXYZ": { "x": 0.8, "y": 1.0, "z": 0.8 }
  }
}
```

Place this object inside collectible `attributes`. The neutral transform is zero
translation/rotation and scale one; its origin can remain the native default.
There is no automatic left/right mirroring or fallback from one arm to the other.
Missing transforms preserve existing rendering; malformed/non-finite optional
transforms are ignored.

Coordinates are relative to the attachment's existing local frame, not world
axes. Static tools use their step-parent frame; animated lights use their mount
and model frame. Models are not normalized to a universal grip point. Transform
matrix order follows native `T * O * Rx * Ry * Rz * S * -O`. The adjustment is
applied before wind motion and affects the attachment's shadows and supported
following particle emitters. Body geometry, other equipment and shared source
shapes remain unchanged. Code that edits transforms should replace the complete
attribute object; the cache detects replacement, not in-place token edits.

## The in-game transform editor

1. Equip an allowed item on either full-size model. Hold another copy of the same
   collectible/material variant in your active hotbar slot.
2. Open `.tfedit` and select **Scarecrow – left arm** or **Scarecrow – right arm**.
   Labels follow the game's language.
3. Adjust translation, rotation, origin and scale. Loaded scarecrows wearing that
   collectible refresh on this client; the other arm's transform is independent.
4. Copy the exported JSON into the corresponding source attribute or patch.
   Reload assets/world as appropriate and verify the saved position again.

This is local development preview, not a per-stack or server-side configuration
editor. All loaded copies of that collectible on the editing client can change;
other players do not receive the preview. Values do not become permanent merely
by closing the editor. Native editor reset/close behavior still applies. Pendulum
switch, pivot and point names are edited in JSON, not by a `.tfedit` checkbox.
Editing a transform does not grant permission for an unsupported held item.

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

Generic models use their arm-storage origin at full scale before the optional arm
transform. Legacy hanging lights retain authored scale, offsets and ceiling frame.
For a legacy hanging light, omit the pivot or explicitly use `[0.5,1,0.5]` rather
than copying the generic example's zero pivot.

The pendulum pivot and `ModelTransform.origin` serve different operations: the
first sets the swing pivot, the second sets the resting transform's origin. Use
the arm transforms for rest alignment, not the swing pivot to compensate for an
incorrect model placement.

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
model-specific mount size and displacement now live in the per-arm transforms.

Legacy hangers marked with `scarecrowArmLantern` or `scarecrowHangingDecoration`
use dedicated `ScarecrowLanternLeftAP` / `ScarecrowLanternRightAP` points and ignore
point overrides. When disabled they retain their private renderer at rest, with
light/textures/particles. Generic tools return to static attachment rendering.

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
| `scarecrowArmLanternScale` | Number; legacy hanger `0.6`, generic model `1` | Additional whole-model renderer scale, separate from the per-arm transform |
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

- [Vanilla tools](scarecrow/assets/scarecrow/patches/wearables/itemtypes-tool-vanilla.json)
- [Walking Stick integration](scarecrow/assets/scarecrow/compatibility/walkingstick/patches)
- [Dreamcatchers integration](scarecrow/assets/scarecrow/compatibility/dreamcatchers/patches)
- [More Lanterns integration](scarecrow/assets/scarecrow/compatibility/apelanterns/patches)
- [Wind Chimes integration](scarecrow/assets/scarecrow/compatibility/windchimes/patches)
- [Shared attachment behavior](scarecrow/src/Wearables/ScarecrowAttachments.cs)
- [Arm transform implementation](scarecrow/src/Wearables/ScarecrowArmTransform.cs)
- [Pendulum metadata implementation](scarecrow/src/Wearables/ScarecrowPendulum.cs)

## Interaction and persistence

One item occupies each slot. Shift + right-click equips a permitted held item;
Shift + empty hand removes individual equipment. Owner-UID and claim checks apply
to attaching, removing and targeted pickup, including server packet handling.
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
