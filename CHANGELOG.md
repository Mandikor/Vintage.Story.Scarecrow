# Changelog

I keep track of notable Scarecrow changes here, newest first. The version numbers
match `scarecrow/modinfo.json`; dates refer to the release commits.

## Unreleased

### Merge-compatible JSON patch operations

- Replace every `add` operation in the shipped JSON patches with `addmerge`.
  Keep target paths, values, conditions and load sides unchanged so object
  additions merge with existing fields through the game's patch implementation.
- Update the patch examples in [FORMODDERS.md](FORMODDERS.md) to use `addmerge`.

### Shared arm mounts in the entity shapes

- Bake the common lantern/hanging mounts and storage-origin pose points into both
  full-size entity shapes, including Plumpkins, instead of adding them by patches.
- Reuse the shared hanging frame for More Lanterns, Wind Chimes and Dreamcatchers.
  Move Wind Chimes' mount scale and Dreamcatchers' scale/offset into their per-arm
  transforms; remove duplicate mod-specific arm mount elements and patches.
- Preserve existing More Lanterns calibration, the little scarecrow's neck setup,
  original body geometry, inventory rules, sound, particles and radius bonuses.
  Foreign-model and neck compatibility patches remain where needed.
- See [FORMODDERS.md](FORMODDERS.md#pendulum-motion) for shared points and transforms.
- Build, 84 native matrix comparisons and 20 owned-asset operations through the
  game's JSON patch library pass. Transform-editor, pendulum and Primitive Survival
  regression checks pass; in-game confirmation of this consolidation is pending.

### Modder documentation

- Consolidated attachment setup, models/textures, per-arm transforms, the native
  editor, all pendulum controls, radius bonuses and full patch examples in
  [FORMODDERS.md](FORMODDERS.md). README now links to this guide instead of
  duplicating implementation instructions. Existing change records below retain
  their historical scope; the guide describes the current development interface.

### Domain-aware natural-spawn filters

- Retain plain `ScaredAnimalPrefixes` entries as case-insensitive path prefixes
  across all mods. Add exact `modid:entity` and `modid:group-*` patterns, including
  `*:group-*` across domains. Qualified paths use explicit `*` wildcards; malformed
  qualified entries and regex patterns are ignored. Cache parsed rules and use
  the same matcher for both natural-spawn hooks.
- Display qualified filters as a translated group/representative animal name
  followed by the mod ID in parentheses. Use a compact group fallback instead of
  the full code; retain plain-entry display. Add an all-animal label in all 33
  languages for rules such as `modid:*`.
- Preserve config schema/defaults, radius checks and cage/player/birth exclusions.
  Build and 30 matching cases pass, including both exact/domain wildcard behavior
  and localized tooltip checks in all 33 languages. In-game confirmation is pending.

### Protection tooltip layout and occupied inventory

- Keep owner formatting and add a blank line afterward. Retain the effective
  spawn radius and affected animals; show the base radius with a generic equipment
  bonus number, without a plus sign or the strongest item's name.
- Add a separated inventory section to placed scarecrows, in left-arm, right-arm,
  neck and foot order. Omit empty slots and omit the heading when no slots are
  occupied. Color only the inventory heading gold; item rows use normal text.
  Apply the same empty-row rule to packed-item inventory tooltips. Omit the
  base/equipment breakdown on the vanilla straw dummy, which has no attachments.
- Update bonus formatting and neck/foot labels in all 33 languages. Preserve
  owner/protection styling, translated item names and escaped dynamic text.

### Consistent neck attachment names

- Update all four selection-box labels in all 33 languages to describe equipment
  locations instead of outdated tool-only or lantern-placement instructions.
  Keep the foot label specific to watering cans; existing item interaction hints
  continue to identify supported equipment.
- Rename `LanternAP` to `NeckAP` and `LanternStorage` to `NeckStorage` in both
  scarecrow entity definitions, all three entity shapes, vanilla lantern attachment
  shapes and all 33 selection-box translations. Preserve geometry, slot order and
  the saved `sc-neck` inventory code.

### Adventurer's Walking Stick arm attachments

Current integration instructions and examples: [FORMODDERS.md](FORMODDERS.md).

- Support 18 complete stick definitions/30 variants from walkingstick 3.0.12 on
  either arm of both full-size models, including 13 lantern materials and complete
  sheathed sticks. Exclude separate hidden blades/guns and empty sheaths, as well
  as small-scarecrow, neck and foot slots.
- Reference original models with neutral per-arm transforms for manual editor
  calibration and pendulum motion initially disabled. Preserve original categories,
  40 existing riding-animal attachment configurations and native item properties.
  No radius bonus or active player-ability adapter is added. Keep all patches in
  `compatibility/walkingstick/patches`; no C# integration is required.
- Build and checks pass for 54 operations using the game's actual JSON patch
  library, all 30 variants, original shapes and categories, slot/domain exclusions
  and zero bonus. My in-game test passed; individual positions remain
  editable through the per-arm transforms.

### General wind-driven pendulum switch

Current integration instructions and examples: [FORMODDERS.md](FORMODDERS.md).

- Add `scarecrowPendulum` as a visual-only opt-in for whole item/block models on
  either full-size arm. Explicit `false` freezes existing hangers at rest; absent
  metadata preserves previous behavior. Keep Wind Chimes' separate moving-part
  animation when enabled and freeze it when disabled, retaining sound and bonuses.
- Generalize the private attachment mesh path to items and blocks, retain native
  texture sources and per-arm transforms, and add storage-origin pose points on
  both standard and Plumpkins models. Generic tools keep their existing rest frame
  and full scale. Existing hanging lights retain their scale, pivot and particles.
- Allow `scarecrowPendulumPivot` and optional per-slot attachment-point overrides;
  add matching pose points for Dreamcatchers' existing specialized mounts. Keep
  tool/Dreamcatcher defaults off and existing animated-decoration defaults on in
  the current patches. No changes to attachment permission, radius or ownership.
- Build and isolated item/block switch, motion, pivot and constructor checks pass;
  Primitive Survival regression checks and 76 operations across 12 attachment
  patch files also pass. My in-game test passed.

### General arm transforms and native editor integration

Current integration instructions and examples: [FORMODDERS.md](FORMODDERS.md).

- Switch both arms' scythes, metal/stone hoes, bug nets, snow shovels, Art of
  Growing hayforks and Leaf Litter rakes to their original game/mod models.
  Remove 16 obsolete tool attachment shapes and preserve all authored transform
  values, including my tested right-arm bug-net placement. Other tool
  positions are intentionally left for manual calibration in the native editor.
- Add optional `onScarecrowLeftArmTransform` and `onScarecrowRightArmTransform`
  collectible attributes using native ModelTransform translation, rotation,
  origin and scaling. Apply adjustments to newly attached static geometry or the
  animated attachment frame before wind motion, including shadows/particles.
  Preserve body geometry, other equipment, native categories and slot permissions.
- Register separate arm targets in the native transform editor, with local live
  preview refresh and labels in all 33 languages. Exported JSON must be saved to
  source assets; preview changes are not synchronized to the server. Remove owned
  editor entries on disposal. No Walking Stick integration is added yet.
- Build and checks pass for native matrix equivalence in both animation versions,
  independent arms, default/invalid metadata, cache invalidation, geometry isolation
  and editor registration/events/cleanup. My transform game test passed.
- Add explicit neutral left/right defaults to all 16 existing arm-attachment
  definitions across 12 patch files, covering vanilla tools/lights, Art of Growing,
  Leaf Litter, More Lanterns, Wind Chimes, Dreamcatchers, censership and Primitive
  Survival. Translation/rotation remain zero, origin is 0.5 per axis and scale is
  one, preserving authored placement. Neck/foot-only items are unchanged.
- Fix six startup patch errors reported in game: More Lanterns and Wind Chimes
  define attributes by type, so place their neutral transforms in the existing
  `attributesByType` branches rather than beneath a missing root `attributes`.
  Verify all 62 operations in the 12 affected patch files with the game's actual
  JSON patch library against the supplied assets.
- Use More Lanterns ceiling variants as texture sources for animated arm models,
  preserving state/color/brightness and supplying rope, paper and coal mappings
  missing from floor-model texture sources. My retest passed; fresh
  client/server main/debug logs contain no errors and no previous texture-mapping
  failures. Server warnings remain about missing `GlassSouthL` step parents for
  Modern 1/2 and Gothic 1 on the Plumpkins shape; these need separate review.

### Animal Cages release compatibility

- Install a server-side Harmony hook only when Animal Cages is active. Mark the
  restored animal with `origin = animalcages-release` before either cage size
  spawns it. This prevents serialized `entityspawner` origins from causing
  released hares, raccoons or other configured animals to be removed by Scarecrow.
- Preserve saved animal state, normal AI and natural-spawn blocking. Keep the
  integration under `src/combat/animalcages`, with no hard dependency on Animal
  Cages and no JSON patches. Remove only this hook when the mod system is disposed.
- Build and checks using the actual 5.0.1 DLL pass: reproduce the original removal,
  verify both cage sizes, saved state, absent-mod gating, continued natural-spawn
  blocking and patch disposal. My in-game test passed: naturally spawned
  hares and raccoons were captured and released beside a scarecrow; both appeared
  successfully and fled through their normal AI tasks.

### Primitive Survival arm lighting

Current integration instructions and examples: [FORMODDERS.md](FORMODDERS.md).

- Support all 13 wooden lanterns and seven filled Firefly Jar types from Primitive
  Survival 5.1.4 on either arm of the standard and Plumpkins full-size models.
  Reuse native ceiling-lantern/jar geometry, textures and light colors with the
  existing wind-driven pendulum renderer. Align the jar cork to the hanging pivot
  through a private mesh offset; original world shapes remain unchanged.
- Wooden lanterns add +2 blocks and filled jars +1 to natural-spawn protection.
  Retain strongest-only bonuses, automatic outline updates and Ctrl + H. Exclude
  empty/baited jars, placed orientations and small/neck/foot slots. I left creature
  AI outside this integration; no AI tasks are patched.
- Keep all integration patches under `compatibility/primitivesurvival/patches`.
  Build and resolver checks pass for 96 variants, slot/bonus/removal rules, native
  textures and duplicate-mesh prevention. Mount checks pass on both full-size
  models. My in-game test passed.

### Plumpkins attachment rendering

Current integration instructions and examples: [FORMODDERS.md](FORMODDERS.md).

- Apply lantern, Wind Chimes, Dreamcatchers and More Lanterns mount patches to
  both full-size scarecrow shapes, including the alternative Plumpkins model.
  Add the missing arm child containers without changing its pumpkin geometry.
- Verify matching attachment positions, upright hanging mounts and preserved
  tool/foot storage points on both models. My in-game test passed.

### Vanilla oil lamps on arms

Current integration instructions and examples: [FORMODDERS.md](FORMODDERS.md).

- Allow all ten filled vanilla oil-lamp colors on either full-size scarecrow arm,
  using the game's existing ceiling shape with suspension and flame geometry.
  Reuse wind-driven pendulum motion and native light/ceramic textures; use the
  hanging variant's texture source for the rope.
- Add a +1-block natural-spawn radius bonus; retain strongest-only equipment
  bonuses so two oil lamps do not stack. Exclude raw/empty lamps, wall/down states
  and other slots. Add original flame/smoke particle profiles as private per-arm
  copies following the swinging wick, with pause/range/throttle guards and no
  shadow-pass emission. World blocks and shared particle data remain unchanged.
- Correct a low oil-lamp particle source found in testing: derive the emitter
  from the rendered ceiling model's flame pivot and parent transforms instead
  of reusing the placed-block emission height.
- Build and actual variant/slot checks pass for 60 color/orientation combinations,
  including duplicate-mesh prevention. My in-game retest passed after
  correcting the particle source.

### censership 1.0.6 arm decorations

Current integration instructions and examples: [FORMODDERS.md](FORMODDERS.md).

- Support 39 upright inventory censers on either full-size scarecrow arm:
  Metal Open, Metal Gills, Metal Dome and Carryable, using original hanging
  geometry/materials and wind-driven pendulum motion.
- Preserve original light and add client smoke from private copies of the
  original ceiling-block particle profiles. No invented radius bonus or buff.
  Stand-only objects, incense/candles, chains and other slots remain excluded.
- Keep all JSON patches under `compatibility/censership` and optional particle
  handling under `src/combat/censership`. Placed blocks remain unchanged.
- Build and checks pass for 234 orientations/material variants, including both
  arms, exclusions, zero bonus and duplicate-mesh prevention. My
  in-game test also passed.

### Wind-driven arm lanterns

Current integration instructions and examples: [FORMODDERS.md](FORMODDERS.md).

- Allow small/large vanilla lanterns on either full-size scarecrow arm alongside
  supported More Lanterns models, with independent wind-driven pendulum motion.
  Keep native metal, lining, glass, variant textures, light and strongest-only
  radius rules. The renderer also works without Wind Chimes installed.
- Remove new neck insertion on the full-size scarecrow. The little scarecrow
  retains its vanilla/More Lanterns neck lantern and previous rendering.
- Preserve old inventory indices and migrate saved/packed full-size neck lanterns
  to the first empty arm. If both arms are occupied, keep the old lantern safely
  removable at its legacy position without losing its bonus or overwriting gear.
- Geometry, variant, slot and migration checks pass, including 768 More Lanterns
  variants and both vanilla sizes/orientations. My gameplay test passed.
- Fix duplicate static/animated arm lanterns found in the initial game test:
  identify arm slots from the inventory instead of the unresolved `default`
  slot-name argument. Keep little/legacy neck rendering; my gameplay
  retest passed.

### Wind chime hanging-piece animation

Current integration instructions and examples: [FORMODDERS.md](FORMODDERS.md).

- Animate individual hanging pieces on each arm, with or without Immersive Lanterns,
  using local wind, damped pendulum response and independent phases. Retain
  original pivots, textures and fixed supports, including copper/bone models.
- Keep my attachment animation active alongside `immersivelanterns`; suppress
  shader wind on private copies to avoid double motion. Placed block animations
  remain independent. Remove the initial switch that left most attached models
  stationary with Immersive Lanterns installed.
- Cache split meshes and release them on equipment changes, shape reloads,
  death and disposal; shadows reuse current transforms. Sound, +6 radius bonus
  and shared/placed-block assets remain unchanged.
- Build and geometry/motion checks pass for seven models and 39 moving subtrees,
  both with original assets and Immersive Lanterns 0.4.1's shape patches applied.
  My attachment game tests passed both without Immersive Lanterns and
  with it enabled. The combined retest was limited to equipping scarecrows;
  placed-block issues were reproduced separately without Scarecrow installed.

### Dreamcatchers 1.0.2 arm decorations

Current integration instructions and examples: [FORMODDERS.md](FORMODDERS.md).

- Allow finished dreamcatchers on either arm of the full-size scarecrow, including
  both arms together, using the existing equipment/ownership/pickup rules.
- Add dedicated upright arm mounts at scale 0.6 and retain original wood/cloth
  textures and wall-placement geometry. All JSON patches live under
  `compatibility/dreamcatchers`; no C# adapter is needed.
- Review confirms the supplied content-only mod has no gameplay protection
  effect. No spawn-radius bonus, storm protection or other effect is invented.
- Attachment/variant checks pass for all 5,200 variants and both arm transforms.
  Correct the invisible attachment in the working 1.0.2 package by using its
  `block/dreamcatcher` model instead of 1.0.31's `block/dreamcatcherwall` path.
  The build passed, and my gameplay retest of the corrected attachment passed.
- Record the separate upstream 1.0.31 packaging error: `itemtypes/bead.json`
  contains shape data instead of an item definition and prevents game loading.
  I confirmed 1.0.2 loads. See local guide.

### Teapot's Undead Legion 2.2.2

- Extend hostile revenants, brutes, hunters, spectres, Lich-raised spectres and
  Liches with scarecrow targets: 21 combat lists and nine wake lists across six
  JSON files. Retain 12 already-covering wildcard lists without duplicate tasks.
- Preserve shared attack cooldowns, ranged spacing, weapon/tier gates, native
  spells and original player/prey behavior. Bound companions, player summons,
  transport creatures and projectiles remain unchanged. No new flee behavior,
  C# adapter or spawn prefixes.
- Actual game resolver checks pass for 180 hostile variants; 73 companion and
  nonliving variants remain unpatched. The build passed, and my in-game test passed.
  See local behavior guide.

### Teapot's Giant Scorpion 1.0.2

- Add five native player-target task copies for surface/cave scorpions: sting,
  claws, charge and two climbing pursuit variants. Preserve native damage,
  poison, reach, movement, animation and emotion conditions/triggers.
- Keep original player/predator tasks intact and disable unrelated-attacker
  retaliation only in copies. No flee task, C# adapter or spawn prefixes added.
- Actual game resolver checks pass for both variants and ten additional task
  configurations; native task inheritance verified against the DLL. Build passes;
  my in-game test also passed. See local behavior guide.

### Talonborne 1.2.0

- Extend the adult and young hippogryphs' native player-sensitive grazing
  interruption lists with all three scarecrow types, retaining the configured
  3-block range and generation-dependent idle sensitivity.
- Keep raised mounts' behavior intact: no invented flee/attack tasks, changes
  to bed-down or commanded rest, ownership, Jaunt flight, eggs or nests.
- Six JSON operations pass checks across all 16 adult/young color variants
  using the game's variant resolver. The build passed, and my in-game test passed.
  See local scope and behavior.

### Pegasus: Winged horses 1.1.0-rc.1

- Add JSON-only support for wild Pegasi: native injury-dependent adult melee
  and pursuit, injury flight for adults/young, and four extended rest/wake lists.
- Gate inherited additions by entity type so feral and tamed mounts retain
  their original behavior. Preserve native emotion conditions, movement,
  damage and ranges; leave eggs, owner following and Jaunt flight controls intact.
- Variant-resolver checks pass for all 12 wild/feral/tamed variants, including
  eight active and 28 disabled additional task configurations. Build passes;
  my in-game test also passed. See local scope.

### Fantasy Creatures Mod 0.8.3

- Add 14 JSON compatibility files for goblins, orks, skeletons, slimes, tree
  creatures and ground/flying dragons: 51 isolated native player-target task
  copies and five extended wake/idle-interruption lists.
- Preserve native attacks, pursuit, injury/daylight flight, variant settings and
  original player/prey tasks. Keep blue-dragon player following unchanged; only
  its player-target wing attack receives a scarecrow copy. No C# adapter or new
  spawn prefixes.
- Actual game variant-resolver checks pass for 26 variants and 74 additional
  task configurations. My in-game test also passed; see the
  local behavior guide.

### Flocks 0.22.1 compatibility

- Verify built-in recognition of both scarecrow sizes and the straw dummy for
  corvid avoidance and resident-crow spawn exclusion. Retain Flocks' independent
  `ScarecrowRadius` (15 blocks by default).
- Add a native mouse flee-task copy and extend its sleeping wake list. Preserve
  original player/predator tasks; disable damage-triggered flight and retaliation
  only in the copy. No new spawn prefixes or C# adapter.
- Other custom bird behaviors retain native player-only reactions; see
  local scope and settings. Build and automated checks
  pass; I completed the in-game test successfully.

### Additional relevant FotSA families

- Add JSON-only support for Felinae 0.2.16, Manidae 1.0.24 and Spheniscidae 1.0.20:
  six adult/young files and 19 native player-tag task copies. Preserve native
  fear conditions, movement/animation maps, priorities and land/swimming gates.
- Retain Manidae's zero-speed defensive animation instead of forcing flight.
  Penguin eggs and fully aquatic Iniidae/Sirenia are left unchanged. Original
  player/prey/breeding tasks are preserved; no new spawn-blocking prefixes.
- Actual game variant-resolver checks pass for 132 variants and 414 enabled
  configurations, including swimming and stationary defense. I also
  completed the in-game test successfully.

### Crows 0.2.7 native compatibility

- Use Crows' existing `otherScarecrows` mechanism instead of adding competing
  generic flee tasks. Retain the full-size 16/18 exclusion/avoidance radii,
  align the little scarecrow to 12/16 and register the straw dummy at 8/12.
- Extend the original player wake list with the three scarecrow types. Preserve
  flight, feeding, landing/perching, player/social behavior and other registrations;
  no C# adapter or new natural-spawn blocking entries are required.
- Verify the actual Crows DLL's recognition, flight-threat checks, boundaries,
  avoidance memory/expiry and native scarecrow values. I also completed
  the in-game test successfully.

### Native player AI for the remaining supported creature mods

- Reworked the remaining 26 static creature integrations and the shared vanilla
  chick template: 134 current definitions, 334 copied player-code/tag tasks and
  143 wake/proximity lists. Preserve native priorities, ranges, species animation
  maps, action settings and emotion conditions rather than generic size/diet rules.
- Restore original player/prey behavior previously changed by older Critter Pack
  and Teapot patches. Retarget only scarecrow reactions, disable attacker leakage
  in added tasks and retain tame/age eligibility. Remove artificial snail fleeing
  where the original mod has no corresponding player reaction.
- Add a gated, one-time server adapter under `combat/equusponies` for final wild
  Equus: Expanded guard/breed AI generated after JSON patching. Verify the actual
  builder's nine wild definitions and 64 variants, mod absence, tame/foreign/client
  exclusions, preserved source data and idempotence.
- Verify Detailed Animals' actual clone routine for both generated chick sexes.
  Bats retains its authored feed-deterrent integration as a custom-AI exception.
  Other vanilla animal patches are unchanged. Build and automated checks pass;
  I also completed this batch's in-game test successfully.

### Feverstone Wilds 3.2.1 native player AI

- Replaced the 3.1.1 baseline and corrected six shifted task-list paths for 3.2.1.
  Added juvenile and adolescent griffon patches alongside the existing cub/adult.
- Reworked 21 wild land-animal definitions with 47 native player-target tasks and
  32 sleep/proximity lists, including adult griffon flight seek/melee attacks.
  Preserve authored priorities, ranges, damage, timing, animations and conditions.
- Exclude tame variants from added reactions and wake-up changes; preserve native
  player/prey/owner tasks and disable attacker substitution in added tasks.
- Correct the cub's wild-variant selection only for its added scarecrow task.
  Integration remains JSON-only; build and actual game variant-resolver checks
  pass. I also found no issues during my in-game test.

### Local mod-library audit and compatibility backlog

- Audited 81 manifest mod IDs from 165 supplied ZIP/directory copies, comparing
  seven available version transitions and checking existing patch targets.
- Documented six invalid Feverstone Wilds 3.2.1 AI paths, native player-code/tag
  adaptation gaps, relevant update changes and malformed upstream source files.
- Added a per-entity CSV and a complete 43-ID candidate backlog, distinguishing
  creature support, pets/capture, equipment, other scarecrows and dependencies.
  Gameplay fixes and further integrations remain explicitly pending in TODO.md.

### Vara's Arctic Wolves: native AI reactions

- Revised the existing 1.5.0 integration to use every player-target AI task as
  its basis: six adult tasks and one pup flee task, preserving native priorities,
  damage, timing, movement, animations, temperature ranges and emotion conditions.
- Added scarecrow targets to both existing sleep wake-up lists while retaining
  player detection. Original player/prey tasks remain intact; added scarecrow
  tasks cannot inherit arbitrary attackers or trigger instant flight on damage.
- Kept all patches under `compatibility/varasarcticwolves/patches`, with no C#
  adapter or changes to the natural-spawn blocking configuration.

### Source organization and contributor documentation

- Grouped optional-mod C# adapters under `src/combat`, including Wind Chimes
  rendering, spatial sound and attachment hooks. Shared inventory rules remain
  in `Wearables`; JSON patches retain their per-mod compatibility directories.
- Moved interaction feedback to `Interactions`, documented important code
  sections and added a source structure and extension guide. Registered names,
  namespaces, save formats and gameplay behavior are preserved.

### Versioned configuration migration

- Added `ConfigVersion` schema 2. Existing 1.9.3 configs without a version are
  treated as schema 1 and upgraded automatically, adding only missing settings
  with defaults while preserving customized values and unknown properties.
- Back up the original configuration to `scarecrow.json.bak` before upgrading.
  Current complete files are left untouched; files with newer schema versions
  are never downgraded or rewritten. Invalid JSON, invalid known settings or
  invalid version values are not overwritten.
- Retain the loaded runtime settings if a backup or save fails. Missing settings
  are also repaired in partially edited configs already marked with the current
  version; case-insensitive property names do not create duplicate entries.

### Cleanup after scarecrow destruction

- Synchronize the empty equipment inventory after death and stop rendering dropped
  attachments and wind chimes on dead scarecrows. Dead scarecrows no longer offer
  equipment or pickup hints, and natural-spawn interception stops immediately.

### Interaction failure feedback

- Added short in-game error messages for placement blocked by solid blocks or
  living entities, unsuitable equipment, occupied attachment points, owner-only
  access and denied claim access. Messages use the requesting player's language;
  all 33 existing language files include the six new messages.
- Server-authoritative feedback avoids duplicate client-prediction messages and
  never writes to chat. Repeated attempts at the same failure remain suppressed
  until a 2.5-second gap; a different failure is shown immediately. Successful
  actions remain silent, and failed placement or attachment preserves the items.

### Environment-dependent wind chime audio

- Attached wind chimes now remain silent in calm weather and play their original
  recordings once with randomized pauses instead of continuously looping. Light
  wind creates longer pauses; strong wind shortens them, including existing pauses.
- Added cover-based sound attenuation for the scarecrow and listener, respecting
  Wind Chimes' `WindChimeIndoorVolume` and `DisableIndoorMuffle` settings. Active
  sounds update their volume as cover changes. Increased the outdoor volume boost
  from 10% to 50% of the configured held-item volume, capped at full volume.
  Hearing range, ambient channel and mute behavior are preserved.
- Kept environmental audio client-only and optional, with no weather queries when
  Wind Chimes is absent or no chime is attached. Weather queries and spatial sound
  positions preserve dimensions. Rendering, wind animation and spawn protection
  are unchanged.
- In-game testing confirmed environmental playback and the adjusted volume.

### Ownership and server interaction protection

- Applied the same owner-UID and claim-access checks to targeted pickup, attaching
  equipment and removing equipment, including attachment client packets. Denied
  interactions no longer fall through to vanilla attachment handling. Existing
  ownerless scarecrows retain claim-based access; damage handling is unchanged.
- Added an owner tooltip line with a bold light-blue label and a normally formatted
  player name. Names are synchronized by the server and resolved for offline owners
  from saved player data. Added owner, ownerless and unknown-name text to all 33
  language files. Existing spawn-protection labels retain the radius ring's color.

### Equipment preservation when relocating

- Added a stored-equipment section to the inventory tooltip of packed scarecrow
  items: left arm, right arm, lantern slot and foot slot, in that order. Little
  scarecrows list only their existing lantern and foot slots. Uses native item/mod
  names, marks empty slots and safely handles unavailable items without changing
  saved equipment. Added all labels to the 33 existing language files.

- Targeted pickup with Ctrl + an empty hand now packs attached equipment into the
  scarecrow item. Placement restores attachments to their original slots with
  durability and custom item attributes intact, including supported mod equipment.
- Only empty scarecrows can stack. Equipped scarecrows occupy individual slots,
  even with identical equipment. Pickup fills the hotbar before the backpack;
  if both are full, the packed item drops at the original position.
- Separated targeted pickup from death through damage: damage still drops equipped
  items separately. Shift + empty-hand removal of individual attachments is unchanged.

### More Lanterns compatibility

Current integration instructions and examples: [FORMODDERS.md](FORMODDERS.md).

- Added optional support for More Lanterns by Ape (`apelanterns` 1.4.4): all eleven
  wall/ceiling-capable metal and paper models, covering 128 upright inventory
  variants with their available colors, on/off states and brightness levels.
  Floating lanterns, stone lanterns and lantern parts are excluded.
- Both scarecrow sizes accept these lanterns at the neck. The full-size scarecrow
  also accepts one in each arm slot instead of a tool, allowing up to three lanterns.
  Existing tool attachment points and equipment categories are preserved.
- Paper lanterns add two blocks to natural-spawn protection and metal lanterns add
  four. Only the strongest equipped lantern's bonus applies, including arm lanterns.
  Removing a lantern immediately uses the strongest remaining bonus or restores
  the base radius. The radius overlay follows automatically; animal flee and
  attack distances are unchanged. On/off and brightness variants use the same bonus.
- Reused More Lanterns' hanging models and variant textures directly. Separate
  texture prefixes allow differently colored lanterns in all three slots without
  texture conflicts. World-block geometry remains unchanged.
- Increased the attached Chinese and decorated Chinese paper lanterns and all metal
  lanterns by 50% over the initial attachment scale. Basic and Medieval 1 Small paper
  lantern sizes are preserved. Counter-rotated both arm mounts so lanterns hang
  vertically in the resting pose without moving their hanging points.
- Kept all four compatibility JSON patch files under
  `assets/scarecrow/compatibility/apelanterns/patches`. Added a lantern compatibility
  guide and automated checks; in-game tests confirmed attachment, scaling, radius
  bonuses and corrected arm rotation.

### Added

- Added a localized protection tooltip for placed scarecrows, little scarecrows
  and straw dummies. It shows the effective natural-spawn radius, base radius,
  strongest equipped bonus and configured animal groups, or an explicit disabled
  state. Server settings are synchronized through entity attributes so multiplayer
  tooltips do not use a client's local spawn list.
- Added tooltip translations to all 33 existing language files. The new section
  starts with a blank line; fixed labels are bold in the radius ring's gold color,
  while numbers and translated equipment/animal names retain normal formatting.
  Equipment changes update the display without chat messages.
- Added optional Wind Chimes by Ape (`windchimes` 1.5.3 and 1.5.4) support for both arm slots
  of the full-size scarecrow: bamboo, wood, bone, brass, copper and crystal models,
  covering all eleven variants in 1.5.4 and the ten variants in 1.5.3.
  Original shapes and textures are reused with upright
  arm mounts, independent of More Lanterns; both compatibility packs can coexist.
- Attached wind chimes play their original material recordings as spatial ambient
  sound, using Wind Chimes' held volume and hearing-distance settings. Sounds follow
  the scarecrow and stop on removal, death, despawn or leaving hearing range.
  Wind chimes add six blocks to natural-spawn protection, taking precedence over
  lanterns without stacking. Removing one uses the strongest remaining equipment
  bonus; the radius outline updates automatically. All compatibility
  JSON patches are kept under `compatibility/windchimes`.
- Check Wind Chimes availability once during attachment initialization. Clients
  without the mod skip periodic audio inventory scans and sound processing entirely;
  servers never enable the sound path.
- Included the bone wind chime from the supplied 1.5.4 package, which is absent
  from the upstream 1.5.3 assets. It retains the original bone textures and the
  wood recording selected by that version's held-sound definition. Older versions
  without the bone model remain supported.
- Attached small vanilla lanterns now increase natural-spawn protection by two
  blocks; large lanterns add four. Both scarecrow sizes and the radius overlay use
  the current attachment bonus, which disappears when the lantern is removed.
  Animal flee and attack distances are unchanged.
- Added animal compatibility for Vara's Arctic Wolves, Bats, Equus: Shared Ancestry,
  Equus: Expanded, Feverstone's Horses, Feverstone Wilds, Sonya's Quality Fowl,
  More Animals, Teapot's Giant Boar, Giant Redriver and Great Bighorn, The Critters
  Pack coral fork, and Truth and Beauty: Detailed Animals.
- Added optional rules for 84 animal definitions and native feeding-deterrent
  settings for two bat definitions. Small and young animals flee; large herbivores
  defend during injury aggression; large predators approach and attack. Separate
  tame, aquatic and non-animal definitions retain their original behavior.
- Added regular JSON patches in separate mod-ID folders under `compatibility`,
  following the existing optional-mod asset structure. Existing mixed-target AI
  tasks retain their other targets. Generated ponies and chicks inherit existing
  rules; no additional C# loader or hard mod dependencies are required.
- Preserved native griffon flight attacks and bat feeding AI, with bat deterrent
  ranges aligned to 16/12/8 blocks. Documented supplied versions and exclusions.

### Fixed

- Corrected Wind Chimes renderer selection to patch server-loaded entity types,
  rather than only the client asset copy. Only remove wind chimes from the normal
  entity mesh when the separate renderer is actually active, preventing invisible
  attachments if that renderer is unavailable.
- Render attached wind chimes separately with the normal block shader so their
  authored wind modes and moving shape regions match the ceiling variants. The
  entity shader otherwise reinterprets their wind flags. Original block models
  remain unchanged; arm alignment, scale and shadows are retained.
- Removed blanket scarecrow fleeing from adult vanilla black, brown, polar and sun
  bears, wolves and hyenas. They now approach and attack scarecrow targets while
  respecting their native injury-fear state; pandas use injury-triggered defense.
- Corrected vanilla age and species selection so elder boars use mature-animal
  rules, smaller deer and goats flee, and young animals cannot inherit adult attacks.
- Corrected animal-mod scarecrow reactions that made small predators attack or left
  large herbivores without injury-triggered defensive attacks. Young animals remain
  on flee-only rules.
- Corrected the extinct duck `chenonettafinschi` movement pattern and selected an
  available charge animation for young elephants in the compatibility patches.
- Scarecrow and little scarecrow placement now checks the full collision box against
  solid blocks and overlapping living entities before consuming the item. Empty
  entity-intersection results correctly allow placement.
- Restored compatibility with vanilla stands by removing the global attachment-category
  overrides. Tools retain their original game or mod categories.
- Forbidden tools are rejected on both the client and server, and attachment hints
  only show items allowed at the selected attachment point.

### Changed

- Increased attached wind chime sound volume by 10% relative to the configured
  held-item volume, capped at full volume. A zero volume remains silent.
- The enabled radius overlay now refreshes automatically every half second after
  placement, movement, removal or changes to nearby loaded scarecrows. It stays
  enabled when no scarecrow is nearby, updates highlights only when the outline
  changes, and stops refreshing when switched off.
- Audited all 36 installed vanilla animal entity files and revised the 13 existing
  vanilla animal patch files, covering 25 definitions and 181 variants. Applied the
  same size, diet and injury-state rules as the animal-mod compatibility patches.
- Preserved vanilla attack damage, animations, timing and variant-specific cooldowns.
  Vanilla task priorities are scaled to each animal's original combat priorities;
  added tasks remain restricted to scarecrow targets. Fish, invertebrates, separate
  tamed elk definitions and debug bots retain their original specialized behavior.
- Audited and revised 46 animal compatibility patch files against the supplied assets
  of the 18 already supported animal mods. Reactions are selected by species size,
  diet and age: herbivores flee, large herbivores can defend themselves while in
  `aggressiveondamage`, small predators flee, and large predators approach and attack.
- Preserved native movement, attack damage and timing where applicable, aligned flee
  ranges to 16/12/8 blocks, and separated species masks so smaller animals and juveniles
  cannot inherit adult attack tasks. Documented the classification and audited mod
  versions in `docs/animal-compatibility.md`.
- Scarecrow attachments now extend the vanilla attachment system with an item-code
  whitelist per slot. The `allowedCodes` lists are defined in the entity JSON files
  and support domain-qualified wildcard patterns.
- Slot categories are derived from the permitted collectibles without replacing their
  original categories or attachment interfaces. A fallback category is supplied only
  for previously uncategorized items with scarecrow-specific attachment shapes.
- Existing attachments are preserved when loading a world and remain removable after
  a whitelist changes.
- Leaf Litter rake attachment shapes and textures now come from a compatibility JSON
  patch instead of the previous dedicated C# registration. The patch no longer
  overwrites the rake's category.
- Explicitly enabled the Art of Growing hayfork attachment patch.
- Added a reference to the game's survival assembly for the vanilla attachment classes.
- Added attachment regression checks covering vanilla stand compatibility, whitelist
  enforcement and category preservation. The `tests` directory is kept local and
  excluded from version control and distribution.

### Documentation

- Updated the MIT copyright notice to cover 2023-2026; the license terms are unchanged.
- Expanded the README with installation instructions, protection radii, animal behavior,
  crafting, controls, equipment, configuration, attachment rules, optional mod
  compatibility, contribution guidance and license information.
- Corrected the little scarecrow's documented protection radius to 12 blocks and
  clarified the difference between natural-spawn protection and animal flee behavior.
- Removed the build-from-source instructions and the local regression-test command
  from the public README.

## 1.9.4 (2026-11-19)

- Fixed animals fleeing from players after taking damage, including bears.
  Scarecrow-specific flee tasks no longer trigger instant flight when an animal
  is hurt and do not react to attackers outside their configured scarecrow targets.
- Applied the correction to vanilla animals and all existing animal-mod patches.
  Existing scarecrow detection ranges, movement settings and natural-spawn
  protection remain unchanged.

## 1.9.3 (2026-11-13)

- Scarecrows no longer remove animals that are born nearby or released from a cage. The spawn
  protection now only affects natural spawns, while newborn and player-placed animals simply
  flee from the scarecrow.
- Vanilla chickens, bears, foxes, hyenas and wolves now flee from scarecrows, little
  scarecrows and straw dummies. Flight distances and priorities were aligned with the
  existing animal patches, and the gazelle, sheep, pig and raccoon patches were corrected.
- Documented the pattern rules for the attachable item list in the source code.
- Ctrl+H (rebindable) toggles an outline on the ground that shows the area a scarecrow keeps
  animals away from. All scarecrows within 64 blocks are shown at once, and the outline follows
  the block grid instead of drawing a circle.
- French, Japanese, Russian and Ukrainian translations completed and corrected against the
  English file, and a typo in the English item description fixed.
- Added 27 more languages: Arabic, Belarusian, Bulgarian, Chinese (Simplified and Traditional),
  Czech, Danish, Dutch, Esperanto, Finnish, Hungarian, Icelandic, Italian, Korean, Lithuanian,
  Norwegian, Polish, Portuguese (Brazil and Portugal), Serbian, Slovak, Spanish (Spain and Latin
  America), Swedish, Thai, Turkish and Vietnamese. These translations are a best effort, so
  native review is welcome.

## 1.9.2 (2026-11-12)

- Attachment categories for the scarecrow slots are now part of the mod instead of the
  shared `attachableToEntity` item data. Another mod that patches that field can no longer
  stop scythes, hoes, bug nets, snow shovels, watering cans, hay forks or rakes from being
  attached.
- The vanilla `categoryCode` of those items is left untouched again.
- The Leaf Litter rake configuration no longer writes a category code either; shapes and
  textures still come from the json patches.

## 1.9.1 (2026-11-11)

- The Leaf Litter rake can be attached to the scarecrow arm slots.
- Compatibility with the FotSA mod family: Bovinae, Capreolinae, Cervinae, Moschidae,
  Elephantidae, Rhinocerotidae, Meiolaniidae, Vombatidae, Caninae, Casuariidae,
  Dinornithidae, Machairodontinae, Pantherinae, Thylacinidae and Viverridae.
- Flight distances and the Bovinae behavior adjusted.
- Scarecrow entities refactored, a spawn hook leak fixed and a mod config added.

## 1.9.0 (2026-11-08)

- Lanterns, watering cans, scythes and hoes can be attached to the scarecrow, with
  attachment points, selection boxes, shift-attach and drops on death.
- Bug nets and snow shovels fit both arm slots; new models, textures and mirror variants
  added; the scythe and hoe attachments revised.
- Hayforks and rakes can be attached to the arm slots.
- Equipment is set aside when the scarecrow is removed.
- Placement hints added.
- Scarecrow models and animations extensively revised; scarecrows and the straw dummy
  expanded.
- Recipe requirements adjusted, grass bundle requirements reduced.
- Mannequins accept scythes and hoes.
- Support for Plumpkins, Art of Growing and the Art of Growing Breeding Addon.
- AI: per-target flight and attacks for the Ferus Wild, per-entity behavior for the wild
  monoceros, per-target behavior for the wild horse.
- Escape behavior for Nandiniidae and Viverridae.
- Preparation for the whole FotSA mod family.

## 1.8.3 (2026-10-01)

- Mod description updated.

## 1.8.2 (2026-05-24)

- Flee entity tasks for animals expanded.

## 1.8.1 (2026-05-14)

- Support for Art of Growing, the Art of Growing Breeding Addon and Grass to Ropes
  restored.
- New recipes, escape behavior for baby monoceros and updated flee entity logic for the
  wild monoceros.
- Refactoring and style improvements, unused files removed.

## 1.8.0 (2026-04-25)

- Updated to .NET 10.0.
- File structure adapted to the vanilla layout, code cleaned up.
- Straw dummy recipe patch rewritten for better compatibility.
- Support for the patched versions of the Art of Growing mods.
- Flee conditions of supported mods adjusted.
- ConfigLib text removed.

## 1.7.1 (2025-10-12)

- Updated to Vintage Story 1.21.1, config removed.
- `modinfo.json` completed; the mod now ships as a DLL.

## 1.7.0 (2025-08-31)

- Updated to Vintage Story 1.21.0 and .NET 8.

## 1.6.10 (2025-06-09)

- Support for the Grass to Ropes mod.
- Support for the Monoceros and Equus mods.
- Update for FotSA: Viverridae.
- All vanilla creatures that eat seeds, fruit or vegetables added to the list of animals
  the scarecrow scares away.

## 1.6.9 (2025-06-01)

- Gazelles added to the list.
- Basic support for the FotSA: Viverridae Plus mod.
- Patching corrected.

## 1.6.8 (2025-05-26)

- Support for the Art of Growing mod and the Plumpkins x Art of Growing: Breeding Addon.
- Recipes finalized for those mods.
- Basic game version set back to 1.20.0.

## 1.6.7 (2025-05-24)

- Support for the Plumpkins x Art of Growing: Breeding Addon.

## 1.6.6 (2025-05-24)

- Recompiled for Vintage Story 1.20.10.
- Adjustments after the game update.

## 1.6.5 (2025-02-09)

- Recipes with dry grass and thatch added.
- Ukrainian translation corrected.

## 1.6.4 (2025-02-07)

- The scarecrow can only be picked up and placed with an empty hand.
- Ukrainian translation by m1RAGE added.
- `LICENSE.txt` updated.

## 1.6.3 (2025-01-26)

- Russian translation by FNX added.
- French translation fixed.

## 1.6.2 (2025-01-25)

- French translation by Mokradin added.

## 1.6.1 (2024-12-07)

- Updated to Vintage Story 1.20.0-rc.1.

## 1.6.0 (2024-11-13)

- Recompiled for Vintage Story 1.20.
- Configuration system cleaned up and a configuration error fixed.
- Texts for the ConfigLib mod added to the language files.
- The unnecessary activation of the scarecrows removed.
- Namespaces changed and `.editorconfig` introduced.
- `LICENSE.txt` updated.

## 1.5.1 (2024-08-18)

- Some additions.
- English language file updated.
- Mod icon fixed.

## 1.5.0 (2024-08-11)

- First release tracked in this repository.
- Straw dummy integration fixed, shapes renamed and adjusted to the original size, json
  additions and cleanup work.
