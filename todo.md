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

## Compatibility review — 2026-11-19

The [local mod audit](docs/mod-library-audit-2026-11-19.md) inventories 81 mod IDs:
38 already supported directly or through inheritance, and 43 not explicitly
integrated. This is a review backlog, not approval to implement every candidate.
Previously deferred cosmetics and aging remain deferred.
After Crows and the additional FotSA batch, 42 IDs are supported and 39 have no
explicit integration. Two of those are reviewed/deferred fully aquatic mods;
37 remain review candidates. The audit retains the original inventory.

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
- [ ] **P2 — Flocks** (`flocks`, 0.22.1): Review all bird entities and custom flock AI; distinguish wild birds from domesticated forms.
- [ ] **P2 — Fantasy Creatures Mod** (`fantasycreaturesupdate`, 0.8.3): Review each species and humanoid separately; copy only relevant player reactions.
- [ ] **P2 — Pegasus: Winged horses** (`pegasus`, 1.1.0-rc.1): Review generated wild versus owned/mount variants and flight AI.
- [ ] **P2 — Talonborne** (`talonborne`, 1.2.0): Mostly mount AI: inspect custom proximity/bed-down hooks and ownership before adding reactions.
- [ ] **P2 — Teapot's Giant Scorpion** (`teapotsgiantscorpion`, 1.0.2): Adapt the five native player-target tasks and validate attack/reach behavior.
- [ ] **P3 — Teapot's Undead Legion** (`teapotsundeadlegion`, 2.2.2): Discuss whether scarecrows should attract hostile undead; do not silently treat them as crop pests.

### Aquatic creature candidates

- [x] **Relevance review — Iniidae Plus** (`iniidae`, 0.1.8): fully aquatic,
  deferred for a land/garden scarecrow; no implementation. Malformed supplied
  item JSON remains an upstream issue.
- [x] **Relevance review — Sirenia** (`sirenia`, 1.0.33): fully aquatic,
  deferred for a land/garden scarecrow; no implementation.

### Taming, pets and capture compatibility

- [ ] **P1 — Animal cages** (`animalcages`, 5.0.1): Verify released/captured animals remain outside natural-spawn blocking; no new pet hostility.
- [ ] **P1 — Catch Livestock** (`catchlivestock`, 0.2.4): Verify release behavior and owner preservation with the existing spawn interceptor.
- [ ] **P1 — Pet AI** (`petai`, 5.1.2): Verify custom pet combat, owner commands and inventory handling; infrastructure, not a new species.
- [ ] **P1 — Tamables: Critters** (`tamablescritters`, 0.2.6): Review runtime changes against Critter Pack patches; compare 0.2.5 and 0.2.6 DLL behavior.
- [ ] **P1 — Tamables: FOTSA** (`tamablesfotsa`, 0.8.7): Review runtime/taming patches against FotSA; keep owned animals out of wild-only reactions.
- [ ] **P2 — Feral Kinship** (`feralkinship`, 0.5.8): Review generated/tamed variants and owner-aware player behavior.
- [ ] **P2 — Universal Husbandry** (`universalhusbandry`, 1.2.1): Review changed husbandry/taming behavior without globally repelling livestock.
- [ ] **P2 — Pet Bunny** (`bunnypet`, 1.0.5): Ensure pet bunnies and newborns are not treated as natural wild spawns.
- [ ] **P2 — Cats** (`cats`, 5.0.2): Review domestic versus wild behavior and owner protection.
- [ ] **P2 — Fox taming** (`foxtaming`, 1.7.6): Review custom petseekentity/petmeleeattack tasks; do not clone owner commands.
- [ ] **P2 — Wolf taming** (`wolftaming`, 5.0.1): Review custom pet AI, fetching, following and ownership; avoid attacks on scarecrows by pets.

### Equipment and decoration candidates

- [ ] **P2 — Immersive Lanterns** (`immersivelanterns`, 0.4.1): Check whether modified vanilla lanterns already work before adding patches or duplicate bonuses.
- [ ] **P2 — Dreamcatchers** (`dreamcatchers`, 1.0.31): Discuss arm attachments and possible gameplay effect; model/attachment feasibility is not yet approved.
- [ ] **P2 — censership** (`censership`, 1.0.6): Review eligible hanging objects and intended effects before adding slot rules.
- [ ] **P3 — Heraldry Add-On** (`heraldryaddon`, 1.1.0): Explore decorative attachments only after the deferred cosmetic item is reopened.
- [ ] **P3 — Heraldry : Banners** (`heraldrybanners`, 2.0.0): Explore banner mounting and compatibility with existing equipment slots.
- [ ] **P3 — More Banners** (`morebanners`, 1.3.3): Decorative candidate; two supplied Better Ruins recipe JSON files fail parsing.

### Other scarecrow implementations and farming mods

- [ ] **P3 — canhalloween** (`canhalloween`, 1.0.4): Review existing scarecrow/decorative entities and overlap; no blanket hostile-creature integration.
- [ ] **P3 — Flowerbeds** (`flowerbeds`, 1.0.3): Review whether its scarecrow/decor behavior should share protection.
- [ ] **P3 — LTOFarmingPlus** (`ltofarmingplus`, 5.0.2): Review farming/scarecrow entities and behavior overlap.
- [ ] **P3 — Scarecrow Modded** (`scarecrowm`, 1.7.3): Alternate Scarecrow Modded package; ships scarecrow.dll. Review assembly/class overlap before joint use.
- [ ] **P3 — Vintage Scarecrows** (`vintagescarecrow`, 0.0.8): Review other scarecrow entities, ownership and overlapping protection.

### Libraries and general utility mods: assess need, do not promise a patch

- [ ] **P2 — Genelib** (`genelib`, 3.2.1): Dependency used by creature generators; verify in the full creature-mod combination.
- [ ] **P2 — Jaunt: Extended Riding System** (`jaunt`, 3.3.1): Riding-system dependency; review only where mounts interact with scarecrows.
- [ ] **P2 — Carry On** (`carryon`, 2.0.0-rc.1): Assess carrying interactions/ownership; do not duplicate normal scarecrow pickup.
- [ ] **P2 — CarryOnLib** (`carryonlib`, 1.0.0-rc.1): Dependency of Carry On; generally no separate creature patch.
- [ ] **P2 — Core of Arts** (`coreofartspatch`, 1.2.2): Dependency of Art of Growing; check shared registration/recipes as a combination.
- [ ] **P3 — Attribute Rendering Library** (`attributerenderinglibrary`, 3.2.0): Rendering library: no direct scarecrow integration identified.
- [ ] **P3 — ConfigKit** (`configkit`, 1.5.2): Configuration library: no direct scarecrow integration identified.
- [ ] **P3 — Heraldry** (`heraldry`, 2.0.0): Dependency for banner integrations, not a creature integration.
- [ ] **P3 — Integrated Mod Manager** (`integratedmodmanager`, 1.1.4): Management utility: no direct scarecrow integration identified.
