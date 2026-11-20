# Animal Cages compatibility

Baseline: Animal Cages 5.0.1 and Vintage Story 1.22.7, on `dev`.

## Release origin

Animal Cages serializes the complete captured entity with `ToBytes(false)` and
restores it with `FromBytes(false)`. A naturally spawned animal therefore retains
`Attributes.origin = entityspawner`. Its direct `SpawnEntity` call bypasses the
natural-spawn interceptor, but still raises the world entity-spawn event. Scarecrow's
fallback filter used to remove released animals whose code matched a configured
prefix and whose position was within the protection radius.

The optional server-only integration installs a Harmony postfix on
`Animalcages.BlockEntityAnimalCage.getCapturedEntity()`. In 5.0.1 this private
method is called from `OnBlockBroken`, immediately before positioning and spawning
the restored animal. Both small and medium cages share this release path.
The postfix replaces the restored copy's origin with `animalcages-release`.
Scarecrow only removes entities marked `entityspawner`, so cage releases survive.

The hook is installed only if `animalcages` is enabled. The target type and method
are resolved at runtime, with a warning if the expected signature is unavailable.
No Animal Cages assembly is referenced at compile time. Harmony is supplied by
the game. Mod-system disposal removes the owned postfix, leaving other mods' hooks
alone. Implementation: `src/combat/animalcages/AnimalCagesCompatibility.cs`.

## Scope

- No changes to cage capture rules, recipes, models or stored entity bytes.
- Owner/generation and other serialized data remain intact; only the restored
  entity's origin is changed before release.
- Existing flee/attack tasks still apply once the animal is free.
- Naturally spawned animals retain their original origin and remain blocked.
- No JSON patches, configuration options or global changes to spawning.

## Validation

Build passed. A local harness loads the actual Animal Cages 5.0.1 DLL and exercises
its serialization/restoration path and Scarecrow's spawn-event filter. It reproduces
the original removal without the patch, then verifies both cage classes with the
real Harmony postfix installed, saved state preservation, absent-mod gating,
continued natural-spawn removal and disposal. These are isolated checks, not a
full running-world simulation.

The author's in-game test passed: naturally spawned hares and raccoons were
captured in cages and released beside a scarecrow. Both appeared successfully
and their normal flee AI activated. Separate coverage of both cage sizes,
occupied-cage save/reload and natural-spawn blocking was not reported in this
game test. A Creative-placed animal alone does not reproduce the original
conflict because it lacks the natural-spawn origin.
