# Feverstone Wilds compatibility

## Version and scope

The `dev` patches now target the supplied **Feverstone Wilds 3.2.1** assets.
Version 3.1.1 has different task-list positions and is no longer the supported
baseline for these patches. The game's JSON patch dependencies check mod IDs,
not version ranges; two conflicting sets of numeric paths cannot be selected
by mod version without an additional mechanism. This integration remains
JSON-only under `assets/scarecrow/compatibility/feverstonewilds/patches`.

The six previously broken positions are corrected: bison calf 13, bison 15,
foal 13, griffon cub 14, adult griffon 21 and horse 14. The new juvenile and
adolescent griffon definitions are included at task AI index 14.

The scope is 21 wild land-animal definitions: the existing 19 plus the two new
griffon ages. Separate tame definitions, fish, faunlings, golems, eggs and nests
are not newly integrated. Mixed wild/tamed files are handled per variant.

## Native player-task basis

The three scarecrow entities represent a stationary player for the added AI
reactions. Original player and prey combat tasks remain intact. Added tasks
retain their native priorities, ranges, speeds, damage, sounds, cooldowns,
animations and emotion conditions, rather than using the earlier size/diet
policy or fixed 16/12/8-block flee ranges.

- 47 added tasks cover player-facing fleeing, seeking, melee attacks and the
  adult griffon's `flightseekentity` and `flightmeleeattack` tasks.
- 32 native sleep/proximity lists also recognize scarecrows on wild variants.
  The original player and predator entries remain; sleep/idle tasks are not
  duplicated. Mixed files use by-type wake lists to preserve tame behavior.
- Both case spellings of the source's `entityCodesByType` fields are respected.
  Explicit enabled maps retain native age/sex restrictions and exclude tamed
  variants, including mixed-file horses, foals, bison and griffons.
- Added tasks disable `retaliateAttacks` and any damage-triggered instant flight.
  They cannot inherit a real player attacker through those mechanisms. Original
  player-facing damage reactions are preserved.

Young animals follow their own authored player reactions. This can include a
native melee task, as with cockatrice chicks; an unconditional “all young flee”
rule is no longer imposed. Giraffes keep the original player-facing seek/flee
tasks; no new player-facing melee task is invented from a predator-only attack.

### Griffon cub source-pattern correction

The source cub flee task uses `*-wild-*` despite the actual variant being named
`griffon-cub-wild`. The added scarecrow task explicitly enables that wild cub and
disables `griffon-cub-tamed`. This corrects the selection for the new reaction
only; the original mod's player task is not edited.

### Custom flight tasks

The 3.2.1 flight seek/melee implementations were inspected: they inherit the
game's targetable AI base and use `IsTargetableEntity` to select their target.
They therefore accept explicit scarecrow codes without a Scarecrow C# adapter.
Their native flight/landing and player-controlled-flight checks are preserved.
This is not a new implementation of griffon flight or nest-defense behavior.

## Validation and game test

Local checks apply all shipped operations to the actual supplied 3.2.1 files and
use Vintage Story's real by-type resolver. They verify 102 enabled resolved task
configurations, 51 excluded tame task configurations, all 32 wake/proximity lists,
and unchanged original player/prey/owner task settings after resolution. The
upstream files are read-only. The build and static checks pass, and the author's
in-game test found no issues in the observed behavior. This does not exhaustively
verify every pathfinding situation or combination with other mods.

Suggested test cases with each of the three scarecrow types:

1. Bison and calf, wild horses and foals: approach, injury reactions and sleeping.
2. Adult griffons on the ground and in flight; cub, juvenile and adolescent fleeing.
3. Tamed horses, bison and griffons: no newly added scarecrow reaction or wake-up.
4. Direwolves, hellboars, cockatrices, giraffes and representative small animals.
5. Combat against real players without a nearby scarecrow: retain native behavior.

Existing original tasks may still outrank a scarecrow task, as they do with other
targets. Equipment bonuses continue to affect natural-spawn blocking only; this
integration does not add Feverstone animals to the default hare/raccoon spawn list.
