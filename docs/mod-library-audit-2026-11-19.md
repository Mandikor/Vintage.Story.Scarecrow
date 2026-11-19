# Local mod compatibility audit — 2026-11-19

Audit baseline: Scarecrow `dev` at `381f5b3`. Sources are the supplied local
`Mods_Scarecrow` library, not ModDB's live release catalogue.

Follow-up on `dev`: [Feverstone Wilds 3.2.1 has now been adapted](feverstonewilds-compatibility.md),
including the six path repairs, native player tasks and new griffon ages. The
tables and CSV below retain the original audit findings at `381f5b3`; they are
historical evidence, not the current completion status. The author completed an
in-game test without finding issues. Remaining work is tracked in [todo.md](../todo.md).

The [remaining existing creature migration](native-creature-compatibility.md) is
also implemented on `dev`, including tag-based tasks and generator handling;
its in-game batch test was completed successfully by the author. The original counts and findings below remain
unchanged as the starting-point audit.

The [Crows 0.2.7 follow-up](crows-compatibility.md) verified built-in support for
both Scarecrow entities in the currently supplied package. The new JSON patch
aligns the little radius and adds straw-dummy exclusion plus wake targets; its
game test was completed successfully by the author. Crows is therefore no longer
an unimplemented candidate.
Current totals are 39 supported IDs and 42 remaining review candidates. The
original tables below are historical and do not override this follow-up.

The [additional FotSA follow-up](fotsa-additional-compatibility.md) now implements
Felinae 0.2.16, Manidae 1.0.24 and Spheniscidae 1.0.20; its game test passed.
Iniidae and Sirenia were reviewed and deferred as fully aquatic. Current totals
are therefore **42 supported IDs** and **39 without explicit integration**, of
which two are deliberately deferred aquatic mods and 37 remain review candidates.
This supersedes the Crows-only follow-up totals above; the inventory tables and
CSV retain their original audit baseline.

## Inventory and scope

- 165 package copies (ZIP files and unpacked directories), representing **81 mod IDs**.
- **38 IDs** already have direct or inherited support; **43** are not explicitly integrated.
- Grouped by manifest ID/version, not folder names; selected the highest supplied version,
  preferring the unpacked directory when the versions match.
- No differences were found between same-ID/same-version copies in the relevant JSON
  compared. This does not certify identical textures, models, binaries or complete ZIP contents.
- Parsed entities, items, blocks, recipes, patches and configuration. Compared DLL
  fingerprints between versions. Texture/model changes were not generally diffed;
  individual shapes referenced by our patches were loaded for path checks.
- Inspected **198 patch-target/asset records** against the selected source assets
  and our own assets. Recipes were parsed separately; patch operations were applied
  to their targets. All referenced targets resolved. Six creature
  patch files have invalid paths; see below. JSON paths are checked independently
  per patch file, not as a simulation of every mod's runtime load order.
- Reviewed **158 currently patched external creature definitions**: **377 native
  player-target tasks**, with **370 non-equivalent copies** and **168 wake/proximity
  lists** still lacking scarecrow support. These are migration gaps, not 370 proven
  defects. Generic size/diet rules deliberately differ from the requested new basis.
- `player` can occur in `entityCodes`, by-type target lists or `entityTags`
  (including FotSA's `anyOf`/`allOf`). Counting direct codes alone misses most
  FotSA reactions. A zero in the table is not proof of no player interaction: custom
  C# tasks and auxiliary proximity fields also need review.

This audit changes documentation and the work plan only. Existing gameplay patches
are deliberately recorded as needing implementation and in-game validation rather
than marked as fixed. No external mod files were modified.

## Immediate findings and implementation order

1. **P0: Feverstone Wilds 3.2.1 breaks six numeric task-list paths.**
2. **P1: replace the generic supported-creature rules with native player-task
   adaptations**, in batches with per-mod game tests. Twenty-seven already patched
   creature mods have detected setting/proximity gaps. Arctic Wolves is already
   adapted; Bats and runtime generators require separate handling.
3. **P1: preserve tame/owned animals and spawn provenance** when the pet, capture
   and taming mods are present.
4. **P2/P3: add new mods gradually**, using the full 43-ID backlog below.

### Feverstone Wilds path failures

| Entity file | Task AI behavior index in 3.1.1 | Correct index in 3.2.1 |
| --- | --- | --- |
| `bison-calf.json` | 11 | 13 |
| `bison.json` | 13 | 15 |
| `foal.json` | 11 | 13 |
| `griffon-cub.json` | 11 | 14 |
| `griffon.json` | 15 | 21 |
| `horse.json` | 12 | 14 |

The shipped patches still use the old indices. They address behaviors without an
`aitasks` array in 3.2.1, so the added reactions cannot be applied there. A fix must
explicitly decide whether to retain 3.1.1 support or require 3.2.1; silently moving
indices would break the older files. New juvenile/adolescent griffons also need
classification. Original tame creatures, fish and humanoids remain separate cases.

## Supported-mod review

`Targets` counts currently patched external entity definitions. `Player tasks`
counts explicit player-code/tag task configurations, before resolving species
variants. `Different` counts native tasks without an equivalent added task after
ignoring only target selection and attacker-safety settings. A task with adjusted
priority, range, movement or emotion conditions therefore counts as different.
`Wake gaps` counts native player wake/proximity lists not extended to scarecrows.

| Mod ID | Latest supplied | Targets | Player tasks | Different | Wake gaps | Result / next action |
| --- | --- | ---: | ---: | ---: | ---: | --- |
| `aogbreedingaddonpatch` | 1.2.2 | 0 | 0 | 0 | 0 | Asset integration; paths applied |
| `apelanterns` | 1.4.4 | 0 | 0 | 0 | 0 | Asset integration; paths applied |
| `artofgrowingpatch` | 1.2.2 | 0 | 0 | 0 | 0 | Asset integration; paths applied |
| `bats` | 1.3.2 | 2 | 0 | 0 | 0 | Custom bat AI; C# behavior needs separate review |
| `bovinae` | 1.0.3 | 2 | 6 | 6 | 0 | Rework native player basis |
| `caninae` | 1.1.10 | 8 | 24 | 24 | 0 | Rework native player basis |
| `capreolinae` | 2.0.21 | 6 | 18 | 18 | 0 | Rework native player basis |
| `casuariidae` | 1.1.14 | 8 | 24 | 24 | 0 | Rework native player basis |
| `cervinae` | 0.1.19 | 2 | 6 | 6 | 0 | Rework native player basis |
| `detailedanimals` | 0.8.4 | 0 | 0 | 0 | 0 | Generated/inherited support; inspect runtime additions |
| `dinornithidae` | 1.0.27 | 17 | 51 | 51 | 0 | Rework native player basis |
| `elephantidae` | 1.0.23 | 2 | 6 | 6 | 1 | Rework native player basis |
| `equus` | 1.4.0 | 1 | 5 | 5 | 4 | Rework native player basis |
| `equusferus` | 1.0.6 | 1 | 5 | 5 | 4 | Rework native player basis |
| `equusponies` | 0.5.0 | 0 | 0 | 0 | 0 | Generated/inherited support; inspect runtime additions |
| `equussharedancestry` | 1.6.0 | 1 | 5 | 5 | 4 | Rework native player basis |
| `feverstonehorses` | 2.4.2 | 3 | 1 | 1 | 3 | Rework native player basis |
| `feverstonewilds` | 3.2.1 | 19 | 43 | 43 | 28 | P0: invalid paths on latest version; then native-AI rework |
| `fowlmod` | 1.0.1 | 2 | 3 | 3 | 3 | Rework native player basis |
| `grasstoropes` | 1.2.4 | 0 | 0 | 0 | 0 | Asset integration; paths applied |
| `leaflitter` | 1.6.0 | 0 | 0 | 0 | 0 | Asset integration; paths applied |
| `machairodontinae` | 1.1.13 | 2 | 6 | 6 | 0 | Rework native player basis |
| `meiolaniidae` | 0.1.13 | 2 | 6 | 6 | 0 | Rework native player basis |
| `monoceros` | 1.4.0-rc.1 | 1 | 2 | 2 | 4 | Rework native player basis |
| `moreanimals` | 1.7.1 | 8 | 16 | 16 | 8 | Rework native player basis |
| `moschidae` | 1.0.1 | 2 | 6 | 6 | 0 | Rework native player basis |
| `pantherinae` | 1.2.18 | 4 | 12 | 12 | 0 | Rework native player basis |
| `plumpkins` | 1.1.3 | 0 | 0 | 0 | 0 | Asset integration; paths applied |
| `rhinocerotidae` | 1.0.29 | 2 | 6 | 6 | 0 | Rework native player basis |
| `teapotsgiantboar` | 1.1.1 | 2 | 6 | 6 | 0 | Rework native player basis |
| `teapotsgiantredriver` | 1.0.2 | 2 | 9 | 9 | 2 | Rework native player basis |
| `teapotsgreatbighorn` | 1.1.1 | 2 | 6 | 6 | 0 | Rework native player basis |
| `thecritterpackcontinued` | 1.4.5 | 43 | 62 | 62 | 107 | Rework native player basis |
| `thylacinidae` | 0.1.9 | 2 | 6 | 6 | 0 | Rework native player basis |
| `varasarcticwolves` | 1.5.0 | 2 | 7 | 0 | 0 | Native basis complete; previous in-game test passed |
| `viverridae` | 1.0.11 | 4 | 12 | 12 | 0 | Rework native player basis |
| `vombatidae` | 0.4.8 | 6 | 18 | 18 | 0 | Rework native player basis |
| `windchimes` | 1.5.4 | 0 | 0 | 0 | 0 | Asset integration; paths applied |

The list is a statement of the current source review, not a certification of every
variant or runtime combination. Existing tame/semitame entities, immobile nestlings,
carcasses and aquatic creatures are intentionally not automatically added. Equus:
Expanded and Detailed Animals generate additional entities in C#; their previously
verified inheritance is not sufficient to claim all new native player tasks are
covered. Bats uses a custom `bat` task rather than ordinary player-target tasks.

### Required native-AI adaptation policy

- Keep original player/prey task lists and their priorities intact. Add isolated
  scarecrow-target tasks for the full-size, little and vanilla straw-dummy entities.
- Retain original movement, attack timings, sounds, cooldowns, temperature rules,
  fear/aggression/hunger states, land/swimming gates and enabled-by-type settings.
- Do not put the `player` tag on scarecrows globally. Retarget copied tag-based
  selectors so unrelated predators/prey cannot leak into the added task. Review
  weight/tag exclusions and custom target checks explicitly.
- Disable attacker substitution and unconditional damage-triggered instant flight
  only in added scarecrow tasks. Leave original damage responses intact.
- Extend original wake/nearby lists while retaining `player`; do not duplicate
  competing sleep/idle tasks. Inspect auxiliary fields such as `spookEntityCodes`
  (Crows), `onNearbyEntityCodes` (Talonborne) and custom C# player searches.
- Do not clone following, owner-command or pet-attack behavior as ordinary wildlife
  reactions. Do not treat equippable decoration support as approval for new bonuses.
- Keep JSON in `compatibility/<manifest-modId>/`; add C# under `src/combat` only
  if the supplied custom AI actually requires it.

## Supplied version changes

| Mod | Manifest versions | Evidence and consequence |
| --- | --- | --- |
| Feverstone Wilds | 3.1.1 → 3.2.1 | Six invalid task-list indices; new sleep/AI behavior and new griffon stages/assets. DLL changed. Needs first implementation batch. |
| Bovinae | 1.0.0 → 1.0.3 | Adult/baby flee animations gain species-dependent `animationByType`; baby speed overrides also change. Existing patch paths apply. Retain these maps during native-task adaptation. |
| Cervinae | 0.1.18 → 0.1.19 | Breeding key corrected from `requiresNearbyEntityCodeByType` to plural `requiresNearbyEntityCodesByType`. AI tasks unchanged, paths apply; no update-specific scarecrow adjustment detected. |
| Teapot's Great Bighorn | 1.0.5 → 1.1.1 | Extra scream idle task in wild/tame adults; original player-target combat settings retained, task AI index remains valid. No positional repair needed; native-basis migration still pending. Folder labeled 1.1.0 actually declares 1.0.5. |
| Leaf Litter | 1.5.1 → 1.6.0 | Needle-duff assets and fruit-litter variants change; rake JSON unchanged, patch applies. Decompiled `LeafLitter.ItemRake` is identical in both DLLs, though the complete DLL differs. No rake attachment change identified. |
| Crows | 0.2.6 → 0.2.7 | Task AI moves from index 11 to 12; hop/wander animation speeds change and DLL differs. No current Scarecrow patch exists; use 0.2.7 as the future baseline. Native `strawdummy` flee task is already present. |
| Tamables: Critters | 0.2.5 → 0.2.6 | Relevant JSON unchanged; DLL fingerprint differs. Runtime taming/AI compatibility needs code and game review, not a fabricated JSON change. |

No older baseline was supplied for the other mod IDs, so “unchanged version” does
not mean their upstream history has been compared. The already supplied Wind
Chimes 1.5.4 and More Lanterns 1.4.4 targets resolve; their existing dedicated
regressions are separate from this static inventory. Binary fingerprint changes
prove a changed file, not a particular gameplay change.

## Unsupported / not explicitly integrated backlog

These are **43 distinct mod IDs**, not 43 required compatibility patches. A mod
may already work through vanilla items, inheritance or its own integration. Each
row is a review candidate; libraries generally need combination tests rather than
a new adapter. P1 comes before P2 and P3.

### New land/flying creature candidates

| Mod ID | Supplied version | Priority | Review / proposed scope |
| --- | --- | --- | --- |
| `crows` | 0.2.7 | P1 | Retain native straw-dummy avoidance; add both scarecrows. Review custom flight, crop/egg theft, keep-distance and curiosity tasks in C#. |
| `felinae` | 0.2.16 | P2 | Adapt player-tag reactions for wild small cats, retaining native states instead of assigning a generic size rule. |
| `manidae` | 1.0.24 | P2 | Adapt player-tag reactions for pangolins, including native defensive states. |
| `spheniscidae` | 1.0.20 | P2 | Separate land and swimming reactions for adult and baby penguins. |
| `flocks` | 0.22.1 | P2 | Review all bird entities and custom flock AI; distinguish wild birds from domesticated forms. |
| `fantasycreaturesupdate` | 0.8.3 | P2 | Review each species and humanoid separately; copy only relevant player reactions. |
| `pegasus` | 1.1.0-rc.1 | P2 | Review generated wild versus owned/mount variants and flight AI. |
| `talonborne` | 1.2.0 | P2 | Mostly mount AI: inspect custom proximity/bed-down hooks and ownership before adding reactions. |
| `teapotsgiantscorpion` | 1.0.2 | P2 | Adapt the five native player-target tasks and validate attack/reach behavior. |
| `teapotsundeadlegion` | 2.2.2 | P3 | Discuss whether scarecrows should attract hostile undead; do not silently treat them as crop pests. |

### Aquatic creature candidates

| Mod ID | Supplied version | Priority | Review / proposed scope |
| --- | --- | --- | --- |
| `iniidae` | 0.1.8 | P3 | Dolphins/related aquatic creatures: decide whether waterborne reactions are useful; malformed supplied item JSON also needs upstream review. |
| `sirenia` | 1.0.33 | P3 | Decide the intended effect on fully aquatic wildlife before implementing underwater AI. |

### Taming, pets and capture compatibility

| Mod ID | Supplied version | Priority | Review / proposed scope |
| --- | --- | --- | --- |
| `animalcages` | 5.0.1 | P1 | Verify released/captured animals remain outside natural-spawn blocking; no new pet hostility. |
| `catchlivestock` | 0.2.4 | P1 | Verify release behavior and owner preservation with the existing spawn interceptor. |
| `petai` | 5.1.2 | P1 | Verify custom pet combat, owner commands and inventory handling; infrastructure, not a new species. |
| `tamablescritters` | 0.2.6 | P1 | Review runtime changes against Critter Pack patches; compare 0.2.5 and 0.2.6 DLL behavior. |
| `tamablesfotsa` | 0.8.7 | P1 | Review runtime/taming patches against FotSA; keep owned animals out of wild-only reactions. |
| `feralkinship` | 0.5.8 | P2 | Review generated/tamed variants and owner-aware player behavior. |
| `universalhusbandry` | 1.2.1 | P2 | Review changed husbandry/taming behavior without globally repelling livestock. |
| `bunnypet` | 1.0.5 | P2 | Ensure pet bunnies and newborns are not treated as natural wild spawns. |
| `cats` | 5.0.2 | P2 | Review domestic versus wild behavior and owner protection. |
| `foxtaming` | 1.7.6 | P2 | Review custom petseekentity/petmeleeattack tasks; do not clone owner commands. |
| `wolftaming` | 5.0.1 | P2 | Review custom pet AI, fetching, following and ownership; avoid attacks on scarecrows by pets. |

### Equipment and decoration candidates

| Mod ID | Supplied version | Priority | Review / proposed scope |
| --- | --- | --- | --- |
| `immersivelanterns` | 0.4.1 | P2 | Check whether modified vanilla lanterns already work before adding patches or duplicate bonuses. |
| `dreamcatchers` | 1.0.31 | P2 | Discuss arm attachments and possible gameplay effect; model/attachment feasibility is not yet approved. |
| `censership` | 1.0.6 | P2 | Review eligible hanging objects and intended effects before adding slot rules. |
| `heraldryaddon` | 1.1.0 | P3 | Explore decorative attachments only after the deferred cosmetic item is reopened. |
| `heraldrybanners` | 2.0.0 | P3 | Explore banner mounting and compatibility with existing equipment slots. |
| `morebanners` | 1.3.3 | P3 | Decorative candidate; two supplied Better Ruins recipe JSON files fail parsing. |

### Other scarecrow implementations and farming mods

| Mod ID | Supplied version | Priority | Review / proposed scope |
| --- | --- | --- | --- |
| `canhalloween` | 1.0.4 | P3 | Review existing scarecrow/decorative entities and overlap; no blanket hostile-creature integration. |
| `flowerbeds` | 1.0.3 | P3 | Review whether its scarecrow/decor behavior should share protection. |
| `ltofarmingplus` | 5.0.2 | P3 | Review farming/scarecrow entities and behavior overlap. |
| `scarecrowm` | 1.7.3 | P3 | Alternate Scarecrow Modded package; ships scarecrow.dll. Review assembly/class overlap before joint use. |
| `vintagescarecrow` | 0.0.8 | P3 | Review other scarecrow entities, ownership and overlapping protection. |

### Libraries and general utility mods: assess need, do not promise a patch

| Mod ID | Supplied version | Priority | Review / proposed scope |
| --- | --- | --- | --- |
| `genelib` | 3.2.1 | P2 | Dependency used by creature generators; verify in the full creature-mod combination. |
| `jaunt` | 3.3.1 | P2 | Riding-system dependency; review only where mounts interact with scarecrows. |
| `carryon` | 2.0.0-rc.1 | P2 | Assess carrying interactions/ownership; do not duplicate normal scarecrow pickup. |
| `carryonlib` | 1.0.0-rc.1 | P2 | Dependency of Carry On; generally no separate creature patch. |
| `coreofartspatch` | 1.2.2 | P2 | Dependency of Art of Growing; check shared registration/recipes as a combination. |
| `attributerenderinglibrary` | 3.2.0 | P3 | Rendering library: no direct scarecrow integration identified. |
| `configkit` | 1.5.2 | P3 | Configuration library: no direct scarecrow integration identified. |
| `heraldry` | 2.0.0 | P3 | Dependency for banner integrations, not a creature integration. |
| `integratedmodmanager` | 1.1.4 | P3 | Management utility: no direct scarecrow integration identified. |

## Input issues and validation limits

- `iniidae:itemtypes/animals.json` is truncated in the supplied directory and
  ZIP. This is one distinct source error observed in two package copies.
- More Banners has two malformed Better Ruins recipe JSON files:
  `bannerclutter-thread.json` and `_bannerclutter-notcollected-thread.json`.
  These are upstream input findings, not edits to Scarecrow or to the originals.
- No general claim is made about graphics, flight navigation, custom task runtime,
  spawning with every mod combination, or owner safety from a static JSON check.
- Missing coverage outside the current wild/mobile patch set includes tame
  Equus/donkey definitions, three immobile FotSA nestlings, Feverstone fish,
  humanoids and newly introduced griffon ages. Classify them rather than blindly
  patching every entity containing a player reference.
- The [per-entity audit](mod-library-audit-entities.csv) records the 158 current
  creature targets and their gaps for implementation. Local raw snapshots and
  checking tools stay under ignored `tests/`; external mod sources stay read-only.

Before marking implementation complete, test player combat without a scarecrow,
all three scarecrow targets, normal/injured/saturated/sleeping states, young and
tame variants, mod absence and representative full-mod combinations.
