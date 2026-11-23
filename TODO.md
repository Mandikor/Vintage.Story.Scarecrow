# Scarecrow ideas

This is my working list for the `dev` branch. I will revisit deferred items before
implementing them. I have removed tool bonuses from the plan.

## Current status — updated 2026-11-23

I have completed the planned compatibility reviews and the recorded game tests.
No active implementation task remains. Visual customization (item 4) and aging/
repair (item 5) remain deliberately deferred.

The Animal Cages occupied-cage save/reload test also passed. No additional
compatibility follow-up test is currently open.

## Feature checklist

1. [x] **Show protection information in the placed scarecrow's tooltip when looking
   at it, without chat messages.** Display the
   effective natural-spawn protection radius, its base radius and strongest
   equipment bonus, and the configured animals affected. Clearly distinguish
   natural-spawn blocking from animals' flee and attack behavior.
   Configured animal filters support legacy group prefixes and qualified exact/
   wildcard codes. Qualified tooltip labels use translated names plus `(modid)`;
   build and 33-language checks passed.
   Extend every existing language file with translated tooltip labels and messages.
   Resolve scarecrow, equipment and animal names through game/mod translations;
   format numbers in the player's chosen language and avoid hard-coded display text.
   Begin the section with a blank line and show fixed text in bold using the radius
   ring's color; keep numbers and names normally formatted.
   Implemented; automated checks and in-game validation passed.
2. [x] **Keep equipment when picking up and relocating a scarecrow.** Store tools,
   lanterns and wind chimes in the returned item and restore their slots on placement.
   Only empty scarecrows stack; equipped ones use individual slots, filling the
   hotbar before the backpack. Damage drops attachments separately and Shift + an
   empty hand removes individual attachments as before. These pickup and removal
   rules also apply in Creative mode; no custom middle-click feature is needed.
   Implemented; automated checks and in-game validation passed.
   Server interaction protection uses the owner UID plus claim access for pickup,
   attaching and removing equipment. Legacy ownerless entities retain claim-based
   access. The tooltip shows the owner with a bold light-blue label and normal name.
   Packed item tooltips list stored equipment in left-arm, right-arm, neck and
   foot order; the little scarecrow lists only its two existing slots. All labels
   are localized. Empty slots are omitted; empty inventories have no heading.
3. [x] **Make attached wind chime audio respond to the environment.** Silent in
   calm weather; original recordings play once with longer pauses in light wind
   and shorter pauses in stronger wind. Cover at the scarecrow or listener reduces
   volume using Wind Chimes' existing indoor settings. The outdoor volume boost is
   now 50%; optional-mod gating, animation and spawn bonus are preserved.
   Implemented; automated checks and in-game validation passed.
4. [ ] **Add visual customization — deferred.** Explore additional hats, heads,
   clothing and wood variants suitable for gardens and farms. No implementation
   is currently planned.
5. [ ] **Add optional wear and repair — deferred on dev.** I have kept the aging
   and repair implementation locally, including a backup branch with staged
   shapes, condition tooltips and persistent collapsed remains. The GitHub
   `dev-aging` branch was removed. Aging is not included in `dev`; further shape
   work and any future integration are postponed.
6. [x] **Improve interaction feedback.** Short in-game errors distinguish placement
   blocked by solid blocks or living entities, unsuitable equipment, occupied
   attachment points, owner-only access and denied claim access. Existing allowed-
   item interaction hints remain available. All 33 languages are covered; feedback
   is server-authoritative, does not write to chat and suppresses repeated failures.
   Implemented; automated checks and in-game validation passed.
7. [x] **Preserve user settings through configuration upgrades.** Added schema
   versioning, missing-default migration and a backup before changing an existing
   file. Customized values and unknown properties are preserved. Invalid files
   and newer schemas are not overwritten. The dev configuration has no aging
   options. Implemented; build and automated migration checks passed.
8. [x] **Clean up destroyed scarecrows.** Synchronize the empty equipment inventory
   after death and stop displaying dropped attachments and wind chimes. Remove
   pickup and equipment hints from dead scarecrows and end natural-spawn
   interception immediately. Implemented; build and automated inventory/drop and
   wind chime checks passed.

9. [x] **Shared mirrored arm transform for modders.** One optional ModelTransform
   attribute for both arms, static and animated equipment, plus a native
   transform-editor entry and local preview refresh. Existing categories and
   attachment permissions remain intact. Build and isolated checks passed;
   see the [arm transform settings](README.md#arm-transforms-for-modders).
   I completed the transform game test successfully. All existing arm
   attachment patches now use a shared transform where possible; neck and
   foot-only items are unchanged.
   Tool arm shapes have been replaced by original models. I tested and aligned
   bug nets, snow shovels, scythes, hoes, hayforks and rakes; right-arm tool
   geometry now mirrors correctly. Walking Sticks are also tested and aligned.
   Added a shared mirrored `onScarecrowTransform` and a both-arms editor target;
   individual-arm attributes and editor targets are removed. See
   [FORMODDERS.md](FORMODDERS.md#arm-transforms) for the current workflow.

10. [x] **General pendulum switch.** Add visual-only `scarecrowPendulum` for item
    and block arm models, optional pivots and custom pose points. Existing lights
    keep their motion by default, tools/Dreamcatchers stay still, and Wind Chimes
    retain their specialized parts. Build, item/block motion/switch checks and
    existing Primitive Survival checks passed. I completed the in-game
    test successfully.

## Completed compatibility reviews — updated 2026-11-23

I checked the supplied ZIP archives and unpacked directories under
`Mods_Scarecrow`, then completed the update, pet-mod and library reviews listed
below. Duplicate packages are not separate integrations. Earlier inventory
counts are historical snapshots, not a current total of supported mods.
The aquatic reviews concluded that no land/garden integration was needed.
Completed reviews do not imply exhaustive testing of every variant, setting or
mod combination; individual entries retain their actual validation scope.

- [x] Inventory ZIP/unpacked duplicates, manifest versions and current support.
- [x] Audit existing patch paths and native player-code/player-tag AI tasks.
- [x] Compare supplied older/newer versions and identify update-specific impacts.
- [x] **Feverstone Wilds 3.2.1:** repaired six shifted AI task-list paths;
  3.2.1 replaces the 3.1.1 baseline. New griffon ages follow native player AI.
  Implemented in JSON; build and game-resolver checks passed.
- [x] **Feverstone Wilds game test:** I tested the integration and found
  no issues in the observed behavior.
- [x] **Native player AI implementation:** the remaining 26 creature mods
  now use native player-code/tag tasks, retaining original action settings and
  player/prey behavior. Shared chick templates, Equus: Expanded generation and
  Bats' custom deterrents are covered separately. See the native creature guide;
  the original audit and CSV retain their initial findings.
- [x] **Existing creature migration game test:** I completed the batch's
  in-game test successfully. Automated checks also passed; this does not imply
  exhaustive coverage of every species, state or mod combination.
- [x] **Arctic Wolves 1.5.0:** native player-task adaptation already implemented
  and confirmed in game.
- [x] **Update implementation details:** Bovinae 1.0.3 animation maps and Great
  Bighorn 1.1.1 native tasks are retained in the migration. Feverstone's game test
  passed; the remaining creature batch's game test also passed.
- [x] Review Bats' custom AI and Equus/Equus: Expanded/Detailed Animals generators.
  Actual builders and the server startup adapter were tested. Bats retains its
  native feed-deterrent mechanism, not every hard-coded EntityPlayer interaction.
- [x] Leaf Litter 1.6.0: rake JSON and decompiled ItemRake unchanged, existing
  attachment patch applies; no update-specific code change identified.
- [x] Cervinae 0.1.19: breeding-key correction does not change AI or my patch paths.

### Reviewed version updates

- [x] **Flocks 0.22.2 source and patch review (2026-11-23):** all supplied assets
  match 0.22.1. Code changes mainly guard player/entity and connection-state
  access during login/logout. Native scarecrow recognition and its configuration
  are unchanged. Existing mouse flee/wake patches apply and preserve all other
  tasks. My 0.22.2 game test passed with the original shared mouse range.
  I subsequently split mouse detection into fixed 16/12/8-block tasks for the
  large/little/straw scarecrows; automated checks passed. My follow-up game test
  confirmed different mouse flee distances for each type. Bird protection is unchanged.

- [x] **Crows 0.2.8 patch update (2026-11-23):** move the idle wake list
  from behavior 12 to 13 and retain its player target. Replace the existing
  large/little radius arrays rather than appending with `addmerge`, giving
  exact guard/avoid values of 16/18 and 12/16; register the straw dummy at 8/12
  and retain unrelated native/mod entries. Version 0.2.8 replaces the indexed
  0.2.7 baseline. Native crow behavior remains in use; my 0.2.8 game test passed.

- [x] **Giant Scorpion 1.0.3 source and patch review (2026-11-23):** the
  entity definition and the three relevant native AI classes match 1.0.2.
  Existing patches apply; all five player-task copies retain native settings
  across both resolved variants (ten configurations). No patch change needed.
  My 1.0.3 game test confirmed the intended attacks.
- [x] **Leaf Litter 1.6.1 source and patch review (2026-11-23):** the rake
  definition only adds `miningspeed.soil = 1`; the decompiled `ItemRake` matches
  the retained 1.6.0 source. Existing patches preserve that addition, both arm
  model references resolve and the shared transform remains available. No patch
  change identified. My 1.6.1 game test confirmed correct attachment, removal,
  display and mirroring on both arms. The old original shape was unavailable for
  an exact geometry comparison; that limitation does not leave a gameplay fix open.

### Completed creature integrations

- [x] **Initial Crows implementation** (`crows`, 0.2.7): integrated through
  native `otherScarecrows` entries and wake lists, without a C# adapter. The
  0.2.8 update above supersedes this baseline and fixes the little scarecrow's
  radius-array merge and shifted wake path. Native flight, feeding and player/
  social behavior are retained.
- [x] **Crows game test:** I completed the in-game test successfully.
- [x] **Felinae implementation** (`felinae`, 0.2.16): native player-tag
  tasks for adult and young caracals/lynxes; native fear conditions retained.
- [x] **Manidae implementation** (`manidae`, 1.0.24): native player-tag
  tasks for adults/young, including stationary adult defense and swimming gates.
- [x] **Spheniscidae implementation** (`spheniscidae`, 1.0.20): native
  adult/chick land and swimming tasks; eggs unchanged.
- [x] **Additional FotSA game test:** I completed the in-game test
  successfully. Automated checks cover 132 variants and 414 enabled task
  configurations.
- [x] **Initial Flocks review and scoped integration** (`flocks`, 0.22.1): retain
  built-in corvid protection; add native mouse flee/wake patches. Seven custom
  bird definitions reviewed; no separate domesticated forms found. Other birds
  retain native player-only reactions; see [compatibility overview](README.md#optional-mod-compatibility).
  Automated checks passed.
- [x] **Flocks game test:** I completed the in-game test successfully.
- [x] **Fantasy Creatures implementation** (`fantasycreaturesupdate`, 0.8.3):
  14 definitions, 51 combat/fear task copies and five wake lists. Native hostile
  humanoid/undead behavior retained; blue-dragon following unchanged. Actual game
  resolver checks pass for 26 variants and 74 additional task configurations.
  See [compatibility overview](README.md#optional-mod-compatibility).
- [x] **Fantasy Creatures game test:** I completed the in-game test successfully.
- [x] **Pegasus implementation** (`pegasus`, 1.1.0-rc.1): three native
  wild-only injury combat/fear copies and four gated wake lists. Feral/tamed
  inherited additions disabled; Jaunt flight/owner controls unchanged. Resolver
  checks pass for all 12 variants. See [compatibility overview](README.md#optional-mod-compatibility).
- [x] **Pegasus game test:** I completed the in-game test successfully.
- [x] **Talonborne implementation** (`talonborne`, 1.2.0): extend adult/young
  grazing interruption only. No native player combat/fear tasks; bed-down,
  commanded rest, owner/flight controls, eggs and nests unchanged. Resolver
  checks pass for all 16 adult/young variants. See [compatibility overview](README.md#optional-mod-compatibility).
- [x] **Talonborne game test:** I completed the in-game test successfully.
- [x] **Giant Scorpion implementation** (`teapotsgiantscorpion`, 1.0.2):
  five native sting/claw/charge/pursuit copies; original reach, poison and emotion
  settings retained. Resolver checks pass for both variants/ten configurations;
  native DLL inheritance verified. See [compatibility overview](README.md#optional-mod-compatibility).
- [x] **Giant Scorpion game test:** I completed the in-game test successfully.
- [x] **Undead Legion implementation** (`teapotsundeadlegion`, 2.2.2):
  hostile undead attack/pursue scarecrows; 21 combat and nine wake lists extended
  directly, 12 existing wildcard lists retained. Bound/player-summoned forms
  excluded. Resolver checks pass for 180 hostile variants; 73 companion/nonliving
  variants unpatched. See [compatibility overview](README.md#optional-mod-compatibility).
- [x] **Undead Legion game test:** I completed the in-game test successfully.

### Aquatic reviews — no integration planned

- [x] **Relevance review — Iniidae Plus** (`iniidae`, 0.1.8): fully aquatic,
  deferred for a land/garden scarecrow; no implementation. Malformed supplied
  item JSON remains an upstream issue.
- [x] **Relevance review — Sirenia** (`sirenia`, 1.0.33): fully aquatic,
  deferred for a land/garden scarecrow; no implementation.

### Taming, pets and capture compatibility

- [x] **Animal Cages release integration** (`animalcages`, 5.0.1): optional
  server-side Harmony hook marks restored animals before spawning from either
  cage size, preserving saved state and avoiding false natural-spawn removal.
  Build and actual-DLL checks passed; natural spawning remains blocked normally.
- [x] **Animal Cages game test:** I captured naturally spawned hares and
  raccoons and released them beside a scarecrow. Both appeared successfully and
  their normal flee AI activated. My follow-up test also passed with both cage
  variants. My occupied-cage save/reload test also passed.
- [x] **Wolf Taming 5.0.1 source compatibility review:** wild wolves reuse the
  existing vanilla wolf definitions. The new dog adults/pups contain no normal
  player-target combat/fear tasks to adapt. Retain native pet aggression,
  ownership, commands, feeding, equipment and taming; no extra AI patch needed.
  Automated checks cover 25 vanilla targets in both patch orders (154 patch
  applications) and 15 resolved dog variants with 300 native task configurations.
- [x] **Fox Taming 1.7.6, Cats 5.0.2 and Pet Bunny 1.0.5 source review:**
  44 resolved pet variants / 820 task configurations contain no autonomous
  player-target fear/combat to copy. Keep the four kittens' native player-play
  tasks, owner commands, feeding and other pet behavior. No new AI patch needed.
  I tested all 120 batch-order permutations of these three mods, Wolf Taming and
  Scarecrow: 29 vanilla targets, 13,320 successful patch applications, no path
  errors. Wild fox/hare taming conversions remain present. These are server-side
  data checks, not a full runtime test of the installed combination.
- [x] **Pet AI / pet mods combined game test** (`petai`, 5.1.2;
  `wolftaming`, 5.0.1; `foxtaming`, 1.7.6; `cats`, 5.0.2; `bunnypet`, 1.0.5):
  I completed the prepared pet/creature/library test package successfully:
  tamed animals do not flee from any of the three scarecrow types. The reviewed
  2026-11-23 session logs contain no errors/exceptions or Scarecrow warnings;
  all 748 server patches applied without errors. This confirms the observed
  combination, not every possible command, variant or configuration.

### Completed equipment and decoration integrations

- [x] **Adventurer's Walking Stick implementation** (`walkingstick`, 3.0.12):
  16 complete definitions/28 variants on either full-size arm, original models,
  calibrated shared transforms and pendulum off. Preserve native categories/riding
  slots; exclude separate weapons, empty sheaths and spear-style Blackguard/Forlorn,
  which could not be attached with Shift + right-click in my game test.
  Original integration checks passed; no radius bonus or player-ability adapter.
- [x] **Walking Stick game test:** I completed the in-game test
  successfully, including alignment of all supported sticks. The shared per-item
  transform remains editable; both removed types have no patches or arm whitelist entries.

- [x] **Plumpkins attachment fix** (`plumpkins`, 1.1.3): apply the existing arm
  mount patches to the alternative full-size model, preserving its rounded head
  and existing tool/foot storage. Build, attachment checks and in-game test passed.

- [x] **Vanilla oil-lamp arm implementation:** ten filled colors using the native
  ceiling suspension/flame model, light, flame/smoke particles and pendulum motion;
  +1 radius bonus. Particle profiles are private per arm and follow the wick.
  Raw/empty lamps and other slots excluded; checks pass for 60 variants.
- [x] **Oil-lamp game test:** I completed the in-game retest successfully
  after correcting the particle source to the model's wick.

- [x] **Arm lantern implementation:** vanilla small/large and More Lanterns
  pendulum animation; no new full-size neck insertion. Little neck retained.
  Saved/packed neck lanterns migrate to a free arm, otherwise remain removable
  without loss. Variant, slot, fixed-pivot and migration checks passed.
- [x] **Arm lantern game test:** I completed the gameplay retest
  successfully after the inventory-based fix for duplicate static meshes.
  Automated checks also cover variant rules, migration and render exclusion.

- [x] **Wind chime motion implementation:** independent wind-driven hanging
  pieces; suppress shader wind on private
  attachment models and leave placed block animation unchanged. Geometry/motion checks
  pass for all seven supplied models and 39 moving subtrees.
- [x] **Wind chime motion attachment game test:** I confirmed successful
  animation and an error-free compatibility retest, limited to equipping
  scarecrows. Third-party placed-block invisibility/crashes were
  reproduced separately without Scarecrow and remain an upstream issue.

- [x] **Dreamcatchers implementation** (`dreamcatchers`, 1.0.2): finished
  decorations on either/both full-size arms, dedicated upright mounts, all 5,200
  variants checked. Supplied content-only mod has no gameplay protection effect;
  no radius bonus added. See [compatibility overview](README.md#optional-mod-compatibility).
- [x] **Dreamcatchers 1.0.2 attachment retest:** I completed the in-game
  retest successfully. Corrected patches use `block/dreamcatcher` instead of the
  incompatible 1.0.31 model path. **Known upstream 1.0.31 error:**
  `itemtypes/bead.json` is a shape instead of an item definition and prevents
  loading. Upstream files remain unchanged.
- [x] **censership implementation** (`censership`, 1.0.6): 39 inventory
  censers on either/both arms; original hanging models, light, cloned smoke
  profiles and pendulum motion. No native protection effect or radius bonus.
  Resolver/slot/source-preservation checks pass for 234 variants. See
  [compatibility overview](README.md#optional-mod-compatibility).
- [x] **censership game test:** I completed the in-game test successfully.

### Primitive Survival compatibility

- [x] **Little scarecrow neck lighting:** allow all 13 wooden lanterns and seven
  filled Firefly Jar types at `sc-neck`, with independent neck transforms and a
  localized editor target. Preserve +2/+1 radius bonuses and existing arm motion.
  Empty/baited jars, placed variants and foot/full-size-neck insertion stay excluded.
- [x] **Primitive Survival neck game test:** I completed the in-game test of the
  little scarecrow's neck-lighting extension successfully.

- [x] **Primitive Survival arm lighting** (`primitivesurvival`, 5.1.4): 13 wooden
  lanterns (+2 radius) and seven filled Firefly Jars (+1), native textures/light
  and wind-driven pendulum motion on either arm of both full-size models.
  Strongest-only bonuses; empty/baited jars and other slots excluded. Build and
  96-variant resolver checks passed. I excluded creature AI from scope.
- [x] **Primitive Survival game test:** I completed the in-game test
  successfully after implementing arm lighting, wind motion and radius bonuses.

### Completed library compatibility reviews

- [x] **Combined library game test — Genelib** (`genelib`, 3.2.1): base library
  used by the Equus creature mods; no separate integration currently identified.
  My combined Equus test passed: wild horses flee, tamed/trained horses remain;
  no interference with Scarecrow observed in the tested package.
- [x] **Combined library game test — Jaunt: Extended Riding System** (`jaunt`,
  3.3.1): base riding library used by the Equus mods. No separate integration
  currently identified. My combined Equus test passed: wild horses flee,
  tamed/trained horses remain; no interference with Scarecrow observed.
- [x] **Core of Arts** (`coreofartspatch`, 1.2.2): base library for the Art of
  mod family. It was loaded throughout my previous game tests without observed
  problems. No separate Scarecrow integration is currently needed.
