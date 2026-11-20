# Scarecrow ideas

Status for the `dev` branch. Deferred items require renewed discussion before
implementation. Tool bonuses were removed from the plan at the author's request.

1. [x] **Show protection information in the placed scarecrow's tooltip when looking
   at it, without chat messages.** Display the
   effective natural-spawn protection radius, its base radius and strongest
   equipment bonus, and the configured animals affected. Clearly distinguish
   natural-spawn blocking from animals' flee and attack behavior.
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
   Packed item tooltips list stored equipment in left-arm, right-arm, lantern and
   foot order; the little scarecrow lists only its two existing slots. All labels
   are localized, and empty slots are identified explicitly.
3. [x] **Make attached wind chime audio respond to the environment.** Silent in
   calm weather; original recordings play once with longer pauses in light wind
   and shorter pauses in stronger wind. Cover at the scarecrow or listener reduces
   volume using Wind Chimes' existing indoor settings. The outdoor volume boost is
   now 50%; optional-mod gating, animation and spawn bonus are preserved.
   Implemented; automated checks and in-game validation passed.
4. [ ] **Add visual customization — deferred.** Explore additional hats, heads,
   clothing and wood variants suitable for gardens and farms. No implementation
   is currently planned.
5. [ ] **Add optional wear and repair — deferred on dev.** The aging and repair
   implementation remains exclusively on `dev-aging`, including staged shapes,
   condition tooltips and persistent collapsed remains. It is not included in
   `dev`; further shape work and any future integration are postponed.
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

9. [x] **General per-arm transforms for modders.** Optional native ModelTransform
   attributes for left/right arms, static and animated equipment, plus native
   transform-editor entries and local preview refresh. Existing categories and
   attachment permissions remain intact. Build and isolated checks passed;
   see the [modder guide](docs/scarecrow-arm-transforms.md).
   The author completed the transform game test successfully. All existing arm
   attachment patches now include neutral left/right transform defaults; neck and
   foot-only items are unchanged.
   Tool arm shapes have been replaced by original models; the author's calibrated
   right-arm bug net is preserved. Remaining tool transforms await manual tuning.

## Compatibility review — updated 2026-11-20

Reconciled with manifests in both ZIP archives and unpacked directories under
`Mods_Scarecrow` after the local library cleanup. Duplicate copies and versions
count once per mod ID: 58 IDs are present (including Animal Cages, re-added after
cleanup), of which 52 have direct integration
or inherit supported content. Six have no explicit integration: two aquatic
mods remain reviewed/deferred, and four remain open review candidates below.
Absent mods have been removed from this active backlog; existing compatibility code remains
unchanged. Previously deferred cosmetics and aging remain deferred.
The [original local mod audit](docs/mod-library-audit-2026-11-19.md) is a historical
snapshot of 81 IDs and does not describe the cleaned current library.

- [x] Inventory ZIP/unpacked duplicates, manifest versions and current support.
- [x] Audit existing patch paths and native player-code/player-tag AI tasks.
- [x] Compare supplied older/newer versions and identify update-specific impacts.
- [x] **P0 — Feverstone Wilds 3.2.1:** repaired six shifted AI task-list paths;
  3.2.1 replaces the 3.1.1 baseline. New griffon ages follow native player AI.
  Implemented in JSON; build and game-resolver checks passed.
- [x] **Feverstone Wilds game test:** the author tested the integration and reported
  no issues in the observed behavior.
- [x] **P1 — Native player AI implementation:** the remaining 26 creature mods
  now use native player-code/tag tasks, retaining original action settings and
  player/prey behavior. Shared chick templates, Equus: Expanded generation and
  Bats' custom deterrents are covered separately. See the native creature guide;
  the original audit and CSV retain their initial findings.
- [x] **Existing creature migration game test:** the author completed the batch's
  in-game test successfully. Automated checks also passed; this does not imply
  exhaustive coverage of every species, state or mod combination.
- [x] **Arctic Wolves 1.5.0:** native player-task adaptation already implemented
  and confirmed in game.
- [x] **P1 — Update implementation details:** Bovinae 1.0.3 animation maps and Great
  Bighorn 1.1.1 native tasks are retained in the migration. Feverstone's game test
  passed; the remaining creature batch's game test also passed.
- [x] Review Bats' custom AI and Equus/Equus: Expanded/Detailed Animals generators.
  Actual builders and the server startup adapter were tested. Bats retains its
  native feed-deterrent mechanism, not every hard-coded EntityPlayer interaction.
- [x] Leaf Litter 1.6.0: rake JSON and decompiled ItemRake unchanged, existing
  attachment patch applies; no update-specific code change identified.
- [x] Cervinae 0.1.19: breeding-key correction does not change AI or our patch paths.

### Updates newly present in the cleaned library

- [ ] **P1 — Giant Scorpion 1.0.3:** compare native player-target tasks and patch
  paths with the implemented/tested 1.0.2 integration; validate any required changes.
- [ ] **P2 — Leaf Litter 1.6.1:** compare rake definitions, attachment geometry and
  relevant code with the reviewed 1.6.0 baseline. No new-version validation claimed.

### New creature candidates

- [x] **P1 — Crows implementation** (`crows`, 0.2.7): verified the built-in
  `otherScarecrows` integration, aligned the little scarecrow to 12/16, registered
  the straw dummy at 8/12 and extended the original wake list. Keep native flight,
  feeding and player/social behavior; no C# adapter. Automated checks passed.
- [x] **Crows game test:** the author completed the in-game test successfully.
- [x] **P2 — Felinae implementation** (`felinae`, 0.2.16): native player-tag
  tasks for adult and young caracals/lynxes; native fear conditions retained.
- [x] **P2 — Manidae implementation** (`manidae`, 1.0.24): native player-tag
  tasks for adults/young, including stationary adult defense and swimming gates.
- [x] **P2 — Spheniscidae implementation** (`spheniscidae`, 1.0.20): native
  adult/chick land and swimming tasks; eggs unchanged.
- [x] **Additional FotSA game test:** the author completed the in-game test
  successfully. Automated checks cover 132 variants and 414 enabled task
  configurations.
- [x] **P2 — Flocks review and scoped integration** (`flocks`, 0.22.1): retain
  built-in corvid protection; add native mouse flee/wake patches. Seven custom
  bird definitions reviewed; no separate domesticated forms found. Other birds
  retain native player-only reactions; see [scope](docs/flocks-compatibility.md).
  Automated checks passed.
- [x] **Flocks game test:** the author completed the in-game test successfully.
- [x] **P2 — Fantasy Creatures implementation** (`fantasycreaturesupdate`, 0.8.3):
  14 definitions, 51 combat/fear task copies and five wake lists. Native hostile
  humanoid/undead behavior retained; blue-dragon following unchanged. Actual game
  resolver checks pass for 26 variants and 74 additional task configurations.
  See [behavior guide](docs/fantasycreatures-compatibility.md).
- [x] **Fantasy Creatures game test:** the author completed the in-game test successfully.
- [x] **P2 — Pegasus implementation** (`pegasus`, 1.1.0-rc.1): three native
  wild-only injury combat/fear copies and four gated wake lists. Feral/tamed
  inherited additions disabled; Jaunt flight/owner controls unchanged. Resolver
  checks pass for all 12 variants. See [scope](docs/pegasus-compatibility.md).
- [x] **Pegasus game test:** the author completed the in-game test successfully.
- [x] **P2 — Talonborne implementation** (`talonborne`, 1.2.0): extend adult/young
  grazing interruption only. No native player combat/fear tasks; bed-down,
  commanded rest, owner/flight controls, eggs and nests unchanged. Resolver
  checks pass for all 16 adult/young variants. See [scope](docs/talonborne-compatibility.md).
- [x] **Talonborne game test:** the author completed the in-game test successfully.
- [x] **P2 — Giant Scorpion implementation** (`teapotsgiantscorpion`, 1.0.2):
  five native sting/claw/charge/pursuit copies; original reach, poison and emotion
  settings retained. Resolver checks pass for both variants/ten configurations;
  native DLL inheritance verified. See [guide](docs/teapotsgiantscorpion-compatibility.md).
- [x] **Giant Scorpion game test:** the author completed the in-game test successfully.
- [x] **P3 — Undead Legion implementation** (`teapotsundeadlegion`, 2.2.2):
  hostile undead attack/pursue scarecrows; 21 combat and nine wake lists extended
  directly, 12 existing wildcard lists retained. Bound/player-summoned forms
  excluded. Resolver checks pass for 180 hostile variants; 73 companion/nonliving
  variants unpatched. See [guide](docs/teapotsundeadlegion-compatibility.md).
- [x] **Undead Legion game test:** the author completed the in-game test successfully.

### Aquatic creature candidates

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
- [x] **Animal Cages game test:** the author captured naturally spawned hares and
  raccoons and released them beside a scarecrow. Both appeared successfully and
  their normal flee AI activated. Cage-size variants and occupied-cage save/reload
  were not separately reported as tested in game.
- [ ] **P1 — Pet AI** (`petai`, 5.1.2): Verify custom pet combat, owner commands and inventory handling; infrastructure, not a new species.

### Equipment and decoration candidates

- [x] **Plumpkins attachment fix** (`plumpkins`, 1.1.3): apply the existing arm
  mount patches to the alternative full-size model, preserving its rounded head
  and existing tool/foot storage. Build, attachment checks and in-game test passed.

- [x] **Vanilla oil-lamp arm implementation:** ten filled colors using the native
  ceiling suspension/flame model, light, flame/smoke particles and pendulum motion;
  +1 radius bonus. Particle profiles are private per arm and follow the wick.
  Raw/empty lamps and other slots excluded; checks pass for 60 variants.
- [x] **Oil-lamp game test:** the author completed the in-game retest successfully
  after correcting the particle source to the model's wick.

- [x] **Arm lantern implementation:** vanilla small/large and More Lanterns
  pendulum animation; no new full-size neck insertion. Little neck retained.
  Saved/packed neck lanterns migrate to a free arm, otherwise remain removable
  without loss. Variant, slot, fixed-pivot and migration checks passed.
- [x] **Arm lantern game test:** the author completed the gameplay retest
  successfully after the inventory-based fix for duplicate static meshes.
  Automated checks also cover variant rules, migration and render exclusion.

- [x] **Wind chime motion implementation:** independent wind-driven hanging
  pieces; suppress shader wind on private
  attachment models and leave placed block animation unchanged. Geometry/motion checks
  pass for all seven supplied models and 39 moving subtrees.
- [x] **Wind chime motion attachment game test:** the author confirmed successful
  animation and an error-free compatibility retest, limited to equipping
  scarecrows. Third-party placed-block invisibility/crashes were
  reproduced separately without Scarecrow and remain an upstream issue.

- [x] **P2 — Dreamcatchers implementation** (`dreamcatchers`, 1.0.2): finished
  decorations on either/both full-size arms, dedicated upright mounts, all 5,200
  variants checked. Supplied content-only mod has no gameplay protection effect;
  no radius bonus added. See [guide](docs/dreamcatchers-compatibility.md).
- [x] **Dreamcatchers 1.0.2 attachment retest:** the author completed the in-game
  retest successfully. Corrected patches use `block/dreamcatcher` instead of the
  incompatible 1.0.31 model path. **Known upstream 1.0.31 error:**
  `itemtypes/bead.json` is a shape instead of an item definition and prevents
  loading. Upstream files remain unchanged.
- [x] **P2 — censership implementation** (`censership`, 1.0.6): 39 inventory
  censers on either/both arms; original hanging models, light, cloned smoke
  profiles and pendulum motion. No native protection effect or radius bonus.
  Resolver/slot/source-preservation checks pass for 234 variants. See
  [guide](docs/censership-compatibility.md).
- [x] **censership game test:** the author completed the in-game test successfully.

### Primitive Survival compatibility

- [x] **Primitive Survival arm lighting** (`primitivesurvival`, 5.1.4): 13 wooden
  lanterns (+2 radius) and seven filled Firefly Jars (+1), native textures/light
  and wind-driven pendulum motion on either arm of both full-size models.
  Strongest-only bonuses; empty/baited jars and other slots excluded. Build and
  96-variant resolver checks passed. The author excluded creature AI from scope.
- [x] **Primitive Survival game test:** the author completed the in-game test
  successfully after implementing arm lighting, wind motion and radius bonuses.

### Libraries and general utility mods: assess need, do not promise a patch

- [ ] **P2 — Genelib** (`genelib`, 3.2.1): Dependency used by creature generators; verify in the full creature-mod combination.
- [ ] **P2 — Jaunt: Extended Riding System** (`jaunt`, 3.3.1): Riding-system dependency; review only where mounts interact with scarecrows.
- [ ] **P2 — Core of Arts** (`coreofartspatch`, 1.2.2): Dependency of Art of Growing; check shared registration/recipes as a combination.
