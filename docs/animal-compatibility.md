# Animal compatibility behavior

The [Giant Scorpion integration](teapotsgiantscorpion-compatibility.md) copies
native sting, claw, charge and climbing pursuit tasks for both surface/cave
forms. Scorpions attack scarecrows; they do not gain a flee task.

The [Talonborne integration](talonborne-compatibility.md) only extends native
grazing interruption for raised adult/young mounts. It adds no combat or fear
and does not override commanded rest.

The [Pegasus integration](pegasus-compatibility.md) extends only wild forms'
native injury-dependent defense/fear and resting wake lists. Inherited additions
are disabled for feral/tamed mounts; Jaunt flight controls are unchanged.

The [Fantasy Creatures integration](fantasycreatures-compatibility.md) adapts
native combat/fear tasks, including hostile humanoids and undead. These creatures
can attack scarecrows; player following is unchanged.

The [Flocks integration](flocks-compatibility.md) retains built-in corvid
protection and adds mouse flee/wake patches. Other custom Flocks birds retain
their native player-only reactions.

The vanilla and compatibility patches add reactions to the scarecrow, little
scarecrow and vanilla straw dummy. They extend the animals' existing AI task lists.
The [Crows integration](crows-compatibility.md) instead extends that mod's own
exclusion-zone registration and wake list, preserving its custom flight AI.
The [additional FotSA batch](fotsa-additional-compatibility.md) adds Felinae,
Manidae and Spheniscidae with their native player-tag conditions. Its in-game
test was completed successfully; fully aquatic Iniidae and Sirenia remain deferred.

The [2026-11-19 local-library audit](mod-library-audit-2026-11-19.md) records newer
supplied versions, detected patch-path failures and the planned migration to
native player-target tasks. The six Feverstone Wilds failures are now corrected
on `dev` for 3.2.1; see the [dedicated guide](feverstonewilds-compatibility.md).
The [remaining existing creature integrations are now migrated on dev](native-creature-compatibility.md),
including generator checks and the custom Bats exception; the author completed
the batch's in-game test successfully. The size/diet rules below are historical for mod integrations and still
describe the remaining vanilla policy. The shared vanilla chick template was
updated for Detailed Animals; other vanilla animals are unchanged in this batch.

## Reaction rules

| Animal group | Normal reaction | After injury |
| --- | --- | --- |
| Young animals, including juvenile and yearling Ferus horses | Flee | Flee |
| Small herbivores and small birds | Flee | Flee |
| Large adult herbivores and large adult flightless birds | Flee | Approach and attack while `aggressiveondamage` is active; otherwise flee |
| Small adult predators | Flee | Flee |
| Large adult predators | Approach and attack | Attack unless the native `fleeondamage` state takes precedence |

Except for the native player-task adaptations for Vara's Arctic Wolves and Feverstone Wilds described
below, `fleeentity` uses a detection range of **16 blocks** for the scarecrow, **12 blocks**
for the little scarecrow and **8 blocks** for the straw dummy. Its escape-distance
setting extends four blocks beyond that range. Actual movement still depends on
terrain and pathfinding.

An attacking animal receives a `seekentity` task as well as `meleeattack`, allowing
it to approach a scarecrow before striking. Large herbivores require the injury
aggression state for both tasks; territorial aggression alone does not enable them.

The patches preserve the original mods' emotion-state probabilities and durations.
Being injured does not guarantee an attack: the animal may enter a native fear
state instead. Immediate survival tasks, such as native panic responses with
higher priorities, can still interrupt the added reactions.

For the original 18 audited animal mods, added task priorities are 8.5 for fleeing, 8.7 for approaching and 8.8 for
melee attacks. This prevents lower-priority native attack tasks from defeating the
small-animal flee rule while leaving higher-priority emergency behavior available.
Retaliation is disabled for the added tasks so their scarecrow target lists cannot
be bypassed by a recent attacker. The original tasks still handle other targets.

## How size is classified

Size is a gameplay choice based on the weights configured in the supplied animal
assets, rather than a claim about real-world taxonomy:

- Large herbivorous mammals and reptiles: a species reaches **100 kg**.
- Large birds: a species reaches **30 kg**.
- Large predators: a species reaches **25 kg**.

The largest configured adult weight for a species determines its group, so males
and females of the same species receive the same behavior. Young animals always
use the young-animal rule regardless of their species. The already supported wild
horse and monoceros adults are treated as large herbivores.

Examples of the resulting assignments:

| Group | Examples |
| --- | --- |
| Large herbivores with injury-triggered defense | Cattle and buffalo, moose and giant deer, larger deer species, elephants, rhinoceroses, wild horses, monoceros, giant horned turtles and giant wombats |
| Large birds with injury-triggered defense | Larger cassowaries, emus, Genyornis and moa |
| Smaller herbivores that flee | Roe deer, musk deer, saola, four-horned antelope and smaller wombats |
| Small predators that flee | Jackals, coyotes, dingoes, foxes, clouded leopards, Tasmanian devils, civets and genets |
| Large predators that attack | Wolves and dire wolves, larger wolf-like canids, the extinct large bush dog and Dusicyon avus, Panthera cats, saber-toothed cats and marsupial lions |

Mixed families are split by species. For example, a gray wolf attacks while a
jackal flees; Panthera cats attack while clouded leopards flee. The dwarf cassowary
falls below the large-bird threshold and flees.

## Audited asset versions

| Mod ID | Supplied version |
| --- | --- |
| `bovinae` | 1.0.0 |
| `caninae` | 1.1.10 |
| `capreolinae` | 2.0.21 |
| `casuariidae` | 1.1.14 |
| `cervinae` | 0.1.18 |
| `dinornithidae` | 1.0.27 |
| `elephantidae` | 1.0.23 |
| `equus` | 1.4.0 |
| `equusferus` | 1.0.6 |
| `machairodontinae` | 1.1.13 |
| `meiolaniidae` | 0.1.13 |
| `monoceros` | 1.4.0-rc.1 |
| `moschidae` | 1.0.1 |
| `pantherinae` | 1.2.18 |
| `rhinocerotidae` | 1.0.29 |
| `thylacinidae` | 0.1.9 |
| `viverridae` | 1.0.11 |
| `vombatidae` | 0.4.8 |

The audit covers the existing wild adult and mobile-young patch targets. Separate
tamed and semi-tamed entity definitions, carcasses, nests and immobile nestlings
remain unchanged. In particular, the Acanthisittidae, Aegothelidae and Strigopidae
nestling definitions deliberately have zero movement speed in the original mod.

## Additional animal mods

The additional compatibility rules cover the following supplied packages. Versions
come from their manifests, rather than the names of their extracted directories.

| Mod ID | Supplied version |
| --- | --- |
| `varasarcticwolves` | 1.5.0 |
| `bats` | 1.3.2 |
| `detailedanimals` | 0.8.4 |
| `equusponies` | 0.5.0 |
| `equussharedancestry` | 1.6.0 |
| `feverstonehorses` | 2.4.2 |
| `feverstonewilds` | 3.2.1 (native player-task adaptation; replaces the 3.1.1 baseline) |
| `fowlmod` | 1.0.1 |
| `moreanimals` | 1.7.1 |
| `teapotsgiantboar` | 1.1.1 |
| `teapotsgiantredriver` | 1.0.2 |
| `teapotsgreatbighorn` | 1.0.5 (directory labeled 1.1.0) |
| `thecritterpackcontinued` | 1.4.5 |

Small wildlife, small birds and young animals flee. Adult wild donkeys and horses,
bison, giraffes, ostriches, yaks and Teapot's large herbivores use injury-triggered
defense. Giant boars follow the defensive omnivore rule used for vanilla pigs.
Adult Arctic wolves use their native player-target reactions as described below.
Feverstone Wilds also follows its native player tasks, including flight attacks,
young-animal exceptions and by-type tame exclusions; see its dedicated guide.
The Critter Pack's
waxwing remains a small fleeing bird despite its unusually high configured weight.

Separate tamed definitions and tame variants in mixed definitions are excluded.
Aquatic animals, humanoid fantasy creatures, carcasses and objects are unchanged.
The Critter Pack's isopods and woodlice retain their specialized AI without added
ground flee tasks. Scarecrow targets in the original entity task lists are updated;
other targets remain intact. Lower-priority flee tasks added by Critter Pack's own
compatibility patches can coexist with these rules.

Griffons retain their native flight approach and attack tasks alongside ground
reactions. Bats retain their native flight and feeding AI: existing scarecrow food
deterrents use 16/12/8-block ranges and retain the mod's radius scaling. Little brown
bats have no such feeding deterrents and remain unchanged. This is feeding
protection, not a general flight-flee task.

The 86 JSON patch files in eleven mod-ID folders under
`assets/scarecrow/compatibility/<modId>/patches` supply rules for 84 animal
definitions and feeding settings for two bat definitions. They use the game's
existing optional-mod compatibility mechanism, just like the earlier patches;
there is no additional C# loader. Patch paths refer to task-list locations in the
audited assets. New task priorities are adapted to native combat priorities in
the same way as the vanilla rules. Native damage, timing and animations are kept.

Equus: Expanded generates ponies from the already patched Ferus templates, and
Detailed Animals generates chicks from the patched vanilla chicken template.
They inherit the existing rules without separate duplicate tasks. None of these
additions changes natural-spawn protection: its default list remains hares and
raccoons.

### Vara's Arctic Wolves: native player-task adaptation

The patches for version 1.5.0 live under `compatibility/varasarcticwolves/patches`.
The manifest mod ID is `varasarcticwolves`, while entity assets use the
`arcticwolves` domain. No C# adapter is required.

Scarecrows represent a stationary player for this integration. Instead of the
earlier generic predator/young-animal rules, each task whose `entityCodes`
contains `player` receives a separate copy targeting only `scarecrow`,
`little-scarecrow` and `strawdummy`. Native task priorities are retained rather
than raised above the original tasks.

- Adults receive both melee attacks, the injury-fear flee task and all three seek
  tasks. Damage, cooldowns, animations, leap settings, hunger and injury conditions,
  and the increased seeking range below -20 degrees remain as authored by Vara.
- Pups receive their original flee task, including its eight-block detection
  range. The same native range applies to all three scarecrow types.
- For both ages, the existing sleep task keeps its `player` wake-up target and
  gains the three scarecrow targets. Sleep duration, priority and wake-up range
  remain unchanged; a duplicate sleep task is not added.
- Only the additional tasks disable `retaliateAttacks` and any
  `instafleeOnDamageChance`. Otherwise a recent player attacker could bypass the
  scarecrow target filter. Original player/prey tasks and their damage responses
  remain intact.

These rules preserve the original animal AI settings; they do not turn
scarecrows into actual players or extend natural-spawn blocking to Arctic wolves.
The copied tasks can still be affected by native AI scheduling and emotion states.

## Vanilla animals

All 36 animal entity files in the installed game's survival assets were inspected.
The 13 existing vanilla patch files cover 25 wild land-animal definitions and 181
valid variants, including young animals, genders and elder Eurasian boars.

| Vanilla group | Scarecrow reaction |
| --- | --- |
| Adult black, brown, polar and sun bears | Approach and attack, except during native injury fear |
| Adult pandas | Flee normally; approach and attack during injury aggression |
| Adult wolves and hyenas | Approach and attack, except during native injury fear |
| Foxes and raccoons | Flee |
| Hares and chickens | Flee |
| Larger deer: whitetail, marsh deer, caribou, elk, chital and fallow deer | Flee normally; defend during injury aggression |
| Smaller deer: red brocket, water deer, pudu, taruca, guemal and pampas deer | Flee |
| Moose | Flee normally; defend during injury aggression |
| Larger goats: Alpine ibex, markhor, muskox, Nubian, golden takin and Dagestan tur | Flee normally; defend during injury aggression |
| Smaller goats: Angora, Nubian ibex, mountain goat, Sirohi and Valais | Flee |
| Gazelles | Flee |
| Adult pigs, elder Eurasian boars and adult sheep | Flee normally; defend during injury aggression |
| Young animals, including poultry poults | Flee only |

These assignments use the game's configured weights. For example, sheep are
configured at 130 kg, adult pigs at 150 kg and muskoxen at 350 kg. Pig feeding
settings are omnivorous, but pigs follow the large-animal defensive rule rather
than actively attacking scarecrows like large predators. The panda asset defines
a vegetable-only diet, so pandas also receive the defensive rule.

Vanilla priorities are adapted to each animal's original combat tasks: fleeing is
0.1 above its highest native attack/approach priority, approaching is 0.2 above,
and melee attack is 0.3 above. The injury gates still decide which reaction is
eligible. Native damage values, attack timing, animations and variant-specific
cooldowns are retained.

The remaining 11 vanilla entity files were reviewed and left unchanged:

- Three fish definitions use underwater AI rather than land-animal reactions.
- Bee swarms and butterflies use flight or specialized butterfly tasks.
- Earthworms and grubs have their own slow movement behavior.
- The two separate tamed/semi-tamed elk definitions retain their riding and ownership behavior.
- The two animal-bot definitions are debug entities rather than wildlife.

## Validation and game testing

The static regression checks cover **46 patch files**, **72 entity definitions**
and **958 distinct entity variants**, including age, gender and coat variants.
They use Vintage Story's own variant resolver to check enabled tasks, emotion
gates, movement speeds, damage values, attack timing and target codes. Model and
entity animation definitions are checked together, since some animations exist
only in the shape files.

The vanilla audit additionally checks **13 patch files**, **25 entity definitions**
and **181 variants**, including 611 enabled task configurations. It also verifies
the panda/predatory-bear split, elder-boar exclusions and coverage of every wild
vanilla mammal and chicken definition.

The initial additional rules were checked against raw mod assets and the game's variant
resolver: 219 variant/file combinations, including 34 excluded tame cases, and
639 enabled task configurations. Checks include applying the shipped JSON patch
operations to the original assets, animation availability and injury gates.
The actual Equus: Expanded builder was exercised for nine wild
definitions and 64 age/gender variants; the actual Detailed Animals clone routine
was checked for both generated chick variants.

The revised Arctic Wolves patches were separately applied to the supplied 1.5.0
adult and baby assets. Checks cover all seven player-target task copies and both
sleep wake-up lists, preserve the original tasks and all non-target settings,
and reject arbitrary-attacker responses in the additional tasks. The author also
completed the revised Arctic Wolves integration's in-game test successfully.

These checks do not simulate pathfinding, task scheduling or a complete running
mod set. In-game testing should include a small herbivore, a large herbivore before
and after injury, a small predator, a large predator and young animals, each around
all three scarecrow types. Other mods that rewrite the same behavior lists can
affect the results. Re-audit the patch paths and variant patterns when updating
the animal mods.
