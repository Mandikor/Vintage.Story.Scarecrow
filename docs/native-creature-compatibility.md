# Native player AI for existing creature integrations

This documents the next migration batch on `dev`, following the already tested
Arctic Wolves and Feverstone Wilds integrations. The author completed this batch's
in-game test successfully. Supplied versions are local baselines, not claims about ModDB's
latest releases.

## What the patches do

The three scarecrow types represent a stationary player for the added tasks.
The native animal's own reactions are the source of truth; the old universal
small/large predator/herbivore rules no longer override supported creature mods.
Native flee, attack, pursuit, emotion, temperature, swimming and sleep behavior
may therefore differ from the earlier Scarecrow version.

- Add separate tasks targeting only `scarecrow`, `little-scarecrow` and
  `strawdummy`. Both explicit player codes and positive player-tag selectors
  are recognized; tasks that explicitly exclude players are not copied.
- Replace target selectors in the copies without globally tagging scarecrows as
  players. Preserve native action settings, including species-dependent movement
  and animation maps, priorities, cooldowns and emotion conditions.
- Disable attacker substitution and damage-triggered instant flight only in added
  tasks. Original player/prey combat settings remain authored by the source mod.
- Extend existing player wake lists instead of duplicating sleep/idle tasks.
  Retain original player/predator entries and exclude tame variants from added
  reactions and wake changes. Separate tame/semitame files remain untouched.
- Remove only superseded native scarecrow-only tasks and trim our three targets
  from mixed native lists. Existing player, prey and third-party scarecrow targets
  remain. This replaces the earlier Critter Pack/Teapot edits that deleted or
  replaced broader native behavior.
- Keep generic enabled-by-type selectors where possible so generated breeds can
  inherit them. The checked static outputs require no exact-code eligibility
  fallback maps. Wild young animals get exactly their native player reactions;
  they are not forced into an invented age-wide attack/flee rule.

## Static batch and versions

The static batch covers **26 creature-mod IDs plus the shared Detailed Animals
chick template**: **134 current entity/template definitions**, **334 native task
copies**, and **143 sleep/proximity lists**. The game's actual variant resolver
verified **5,536 enabled** and **95 excluded** task configurations.
The [per-entity migration table](native-creature-migration.csv) records targets,
task paths, copies, wake lists and superseded native scarecrow tasks.

| Mod / template group | Supplied version | Definitions | Native copies | Wake lists |
| --- | --- | ---: | ---: | ---: |
| `bovinae` | 1.0.3 | 2 | 6 | 0 |
| `caninae` | 1.1.10 | 8 | 24 | 0 |
| `capreolinae` | 2.0.21 | 6 | 18 | 0 |
| `casuariidae` | 1.1.14 | 8 | 24 | 0 |
| `cervinae` | 0.1.19 | 2 | 6 | 0 |
| `detailedanimals` | 0.8.4 | 1 | 1 | 1 |
| `dinornithidae` | 1.0.27 | 17 | 51 | 0 |
| `elephantidae` | 1.0.23 | 2 | 6 | 1 |
| `equus` | 1.4.0 | 1 | 4 | 4 |
| `equusferus` | 1.0.6 | 1 | 5 | 4 |
| `equussharedancestry` | 1.6.0 | 1 | 5 | 4 |
| `feverstonehorses` | 2.4.2 | 3 | 5 | 3 |
| `fowlmod` | 1.0.1 | 2 | 3 | 3 |
| `machairodontinae` | 1.1.13 | 2 | 6 | 0 |
| `meiolaniidae` | 0.1.13 | 2 | 6 | 0 |
| `monoceros` | 1.4.0-rc.1 | 1 | 2 | 4 |
| `moreanimals` | 1.7.1 | 8 | 16 | 8 |
| `moschidae` | 1.0.1 | 2 | 6 | 0 |
| `pantherinae` | 1.2.18 | 4 | 12 | 0 |
| `rhinocerotidae` | 1.0.29 | 2 | 6 | 0 |
| `teapotsgiantboar` | 1.1.1 | 2 | 6 | 0 |
| `teapotsgiantredriver` | 1.0.2 | 2 | 9 | 2 |
| `teapotsgreatbighorn` | 1.1.1 | 2 | 6 | 0 |
| `thecritterpackcontinued` | 1.4.5 | 41 | 65 | 109 |
| `thylacinidae` | 0.1.9 | 2 | 6 | 0 |
| `viverridae` | 1.0.11 | 4 | 12 | 0 |
| `vombatidae` | 0.4.8 | 6 | 18 | 0 |

Bovinae now uses the 1.0.3 per-species flee animation maps. Cervinae uses 0.1.19
and Great Bighorn uses 1.1.1. These do not imply compatibility with arbitrary
future task-array changes. Equus files with redundant per-scarecrow patch lists
were consolidated; patches remain in each manifest-ID compatibility folder.

### Critter Pack snails

The formerly patched `helix-snail.json` and `snail-default.json` contain no
corresponding native player reaction. Their artificial Scarecrow flee patches
were removed rather than inventing a player behavior. The original snail AI is
unchanged. The 134 current definitions exclude these two removed patch targets.

## Runtime generators

### Equus: Expanded 0.5.0

Equus: Expanded runs its builder after JSON patches. It can add new player guard
attacks, remove player flee targets and alter inherited damage/ranges. Template
patches alone cannot accurately cover those breed-specific results.

`src/combat/equusponies/EquusPoniesCompatibility.cs` therefore runs once during
server `AssetsFinalize`, after variant resolution. With `equusponies` installed
it processes resolved wild types in the `equusferus` domain, removes inherited
scarecrow copies and rebuilds them from the final enabled player tasks. It
preserves native tasks, replaces copied behavior JSON instead of editing shared
read-only input, and is idempotent. There is no per-tick scan and no dependency
on the optional mod's DLL. Client, absent-mod, tame and foreign-domain cases are
excluded.

Tests invoke the actual 0.5.0 builder for nine wild definitions and 64 age/gender
variants, including guard breeds. They compare the final copied settings, verify
idempotence and unchanged input, and exercise the startup hook against the game's
resolved entity-property classes.

### Detailed Animals 0.8.4

Detailed Animals clones the vanilla chicken-baby template after JSON patching.
The chick portion of the existing vanilla chicken patch now uses the native
player task, so both generated chick variants inherit it. This also updates the
shared vanilla chick; other vanilla animal patches are not migrated in this batch.
The actual Detailed Animals clone method was exercised for both generated sexes.

## Bats 1.3.2: deliberate custom-AI exception

Bats' `AiTaskBat` has hard-coded `EntityPlayer` sensing/combat checks, not ordinary
configurable player-target tasks. A scarecrow is not an `EntityPlayer`. The
existing two JSON patches therefore continue to use its authored `feedDeterrents`
mechanism: full-size 16 blocks, little 12, straw dummy 8. The native flight, player
interaction and feeding system is retained; the integration does not claim to
emulate every player-only bat behavior. No generic ground flee task or replacement
flight AI was added.

The native deterrent code was inspected and both patches were checked for target
codes/ranges and preservation of all settings outside the deterrent field.

## Validation and game testing

All shipped static operations were applied to the supplied raw assets and resolved
with the real game variant resolver. Checks preserve original action/player/prey
settings, validate target isolation, enabled-by-type rules and wake lists, and
read the written patches back. Runtime tests use the actual external builders.
The author also completed the batch's in-game test successfully. These checks
do not exhaustively cover navigation, scheduling or every mod combination.

Suggested in-game samples: large/small FotSA predators and herbivores, adults and
young, swimming cases, Critter Pack birds and snails, Teapot adults/babies, wild
Equus/Monoceros, generated guard/flight breeds, Detailed Animals chicks and Bats
feeding deterrence. Compare against real-player behavior without a scarecrow,
then each scarecrow type, including injured/saturated/sleeping and tame cases.

Natural-spawn blocking remains separate and defaults to hares/raccoons. Lantern
and wind-chime equipment bonuses do not change these native AI ranges. New-mod
candidates from the library audit are not implemented by this migration.
