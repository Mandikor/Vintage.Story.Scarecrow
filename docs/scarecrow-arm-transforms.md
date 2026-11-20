# Arm transforms for modders

Scarecrow supports two optional collectible attributes using Vintage Story's
standard `ModelTransform` format:

- `onScarecrowLeftArmTransform` for `sc-leftarm`.
- `onScarecrowRightArmTransform` for `sc-rightarm`.

These are JSON attributes, not item callbacks or methods to override. They adjust
only the rendering of an already permitted attachment. They do not grant permission
to equip an item, change its category, or add a radius bonus. The existing item-code
whitelist and `attachableToEntity` shape/texture metadata are still required.

## Example

Place the attributes on the collectible definition or add them with an optional
compatibility patch. Retain any existing categories and slot shapes.

```json
"attributes": {
  "onScarecrowLeftArmTransform": {
    "translation": { "x": 0, "y": 0.1, "z": 0 },
    "rotation": { "x": 0, "y": 0, "z": 15 },
    "origin": { "x": 0, "y": 0, "z": 0 },
    "scale": 0.8
  },
  "onScarecrowRightArmTransform": {
    "translation": { "x": 0, "y": 0.1, "z": 0 },
    "rotation": { "x": 0, "y": 0, "z": -15 },
    "origin": { "x": 0, "y": 0, "z": 0 },
    "scale": 0.8
  }
}
```

Translation and origin use block units (1 = 16 shape units); rotation uses degrees.
`scale` is uniform, while `scaleXYZ` supports independent x/y/z scaling. Omitted
translation/rotation default to zero, scale to one and origin to `(0.5, 0.5, 0.5)`,
as in the native editor. Specify origin explicitly when a different pivot is needed.

Transforms are additional adjustments in the attachment's existing local frame,
not world coordinates or replacements for the authored slot shape. A static tool
uses the frame of the shape element into which it is step-parented. Animated
lanterns/chimes use their existing mount/model frame, before wind motion. Different
authored models may therefore need different values and origins; they are not
automatically normalized to a common grip point. Left and right settings are
independent, with no automatic mirroring or fallback to the other arm.

All existing arm-attachment integrations include explicit neutral defaults for
both arms in their patches: zero translation/rotation, origin `(0.5, 0.5, 0.5)`
and scale one. These preserve the existing authored positions while providing
editable starting points. This covers 16 collectible definitions across 12 patch
files; definitions with variants share the defaults across their variants.
Foot-only watering cans and neck-only slots do not need arm attributes.

Tool attachments now reference the original scythe, metal/stone hoe, bug-net,
snow-shovel, hayfork and rake models on both arms. Their 16 dedicated arm shapes
have been removed. The author verified the right-arm bug net through editor
adjustment, JSON export and world reload; those saved transform values are retained.
The remaining tool placements are awaiting manual calibration. Do not reset
existing transform values to identity when updating the model references.
For definitions using `attributesByType` instead of a root `attributes` object,
the defaults are included in each existing attribute branch. Patching a child of
a nonexistent root object fails before the game's by-type resolution stage.

Static geometry is wrapped only after the engine has added that slot's shape.
The body, other attachments, shared source shapes and native texture collection
are preserved. Animated attachments apply the same native transform matrix before
their existing motion, including shadow rendering and following particle emitters.
Absent attributes leave the previous appearance unchanged. Malformed/non-finite
optional transforms are ignored. Cache invalidation follows replacement of the
attribute token; code changing transforms should replace the complete JSON object,
as the editor does, instead of mutating a cached token in place.

## Optional whole-model pendulum

Add a boolean alongside the transform attributes:

```json
"scarecrowPendulum": true,
"scarecrowPendulumPivot": [0, 0, 0]
```

The switch is independent of attachment permission, item category and radius
bonus. It accepts items as well as blocks. Current tool and Dreamcatcher patches
explicitly default to `false`, while existing animated lights/decorations and
Wind Chimes default to `true`. With no switch, legacy hanging decorations/chimes
keep their previous animation and ordinary items stay static.

Generic opted-in models render separately rather than being added to the static
body mesh. They use the same arm storage coordinate frame as ordinary tools, with
scale one by default. Their transform is applied before pendulum motion. The
optional three-number pivot is in that model frame, in block units, before the
arm transform; it defaults to `(0,0,0)` for generic models. Existing lanterns and
other legacy hangers retain their ceiling-frame pivot `(0.5,1,0.5)` and original
scale. Their existing mesh offsets are still applied before motion.

`false` leaves legacy hanging models in the private renderer at their resting
pose, preserving textures, light and particle handling. Ordinary tools return to
their static attachment path. Wind chimes retain their specialized split meshes:
`true` advances individual parts, while `false` leaves them at rest. Their wind
sounds and radius bonuses are unaffected. Neither the little scarecrow's neck
nor ordinary placed world blocks gain this motion.

For an attachment that already uses a special step-parent frame rather than the
normal arm storage, an advanced optional mapping can name equivalent pose points:

```json
"scarecrowPendulumAttachmentPoints": {
  "sc-leftarm": "MyLeftMountAP",
  "sc-rightarm": "MyRightMountAP"
}
```

The modder must supply those attachment points on the entity shape. Without an
override, generic models use `ScarecrowArmLeftOriginAP`/`ScarecrowArmRightOriginAP`.
Dreamcatchers already include equivalent points on their existing mounts so their
position is retained if enabled. Legacy hanging lights keep their dedicated
lantern points. Models requiring multiple different step parents or a custom
shape/texture supplier need individual validation; a boolean does not reproduce
arbitrary custom rendering logic.

Edit these attributes in source patches and reload the game/assets. `.tfedit`
continues to edit the transform attributes; it does not provide a pendulum checkbox.

The author reported a successful in-game test of the pendulum extension.

## Native transform editor

1. Equip a currently supported item on a full-size scarecrow. Use either the
   normal or Plumpkins model. Hold another copy of that same collectible in your
   active hotbar slot, retaining the same variant/material code.
2. Open the native editor with `.tfedit` and select **Scarecrow – left arm** or
   **Scarecrow – right arm** from its transform list. Labels follow the game language.
3. Adjust translation, rotation, origin and scale. Loaded scarecrows carrying the
   selected collectible refresh locally; other items and the other arm are unaffected.
4. Use the native editor's JSON-copy controls and save the exported attribute in
   your source assets or compatibility patch. Reload assets/game to verify persistence.

This is a client-side development preview, not a server editing command. Changes
affect every loaded instance of that collectible on the editing client and are not
sent to other players or saved into individual equipment stacks. Native editor
reset/close behavior still applies. Merely holding an unsupported item does not
make it attachable. The little scarecrow has no arm slots and is unaffected.

## Validation

Build and isolated checks passed for native matrix equivalence in animation
versions 1 and 2, independent arms, default/invalid metadata, cached reads and
replacement/removal, isolation of body/previous equipment, native editor target
registration, event-driven preview refresh and disposal cleanup.

The author reported a successful in-game test of the transform function. Explicit
neutral defaults were added afterward and checked for identity matrices and complete
arm-patch coverage. No Walking Stick integration is included in this change.
After reported startup errors, all 62 operations in the 12 affected patch files
were also applied successfully using the game's actual JSON patch library against
the supplied mod assets. The author completed the subsequent game test successfully;
the fresh client/server main/debug logs contain no errors. The six patch failures
and previous arm-lantern texture-mapping errors are absent. Server warnings still
report missing `GlassSouthL` step parents for More Lanterns Modern 1/2 and Gothic 1
on the Plumpkins shape; these have not yet been resolved.
