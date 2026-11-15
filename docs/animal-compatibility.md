# Animal compatibility behavior

The vanilla and compatibility patches add reactions to the scarecrow, little
scarecrow and vanilla straw dummy. They extend the animals' existing AI task lists.

## Reaction rules

| Animal group | Normal reaction | After injury |
| --- | --- | --- |
| Young animals, including juvenile and yearling Ferus horses | Flee | Flee |
| Small herbivores and small birds | Flee | Flee |
| Large adult herbivores and large adult flightless birds | Flee | Approach and attack while `aggressiveondamage` is active; otherwise flee |
| Small adult predators | Flee | Flee |
| Large adult predators | Approach and attack | Attack unless the native `fleeondamage` state takes precedence |

`fleeentity` uses a detection range of **16 blocks** for the scarecrow, **12 blocks**
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

For the audited animal mods, added task priorities are 8.5 for fleeing, 8.7 for approaching and 8.8 for
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
Additional animal mods present in the supplied directory were not added to the
compatibility list by this change.

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

These checks do not simulate pathfinding, task scheduling or a complete running
mod set. In-game testing should include a small herbivore, a large herbivore before
and after injury, a small predator, a large predator and young animals, each around
all three scarecrow types. Other mods that rewrite the same behavior lists can
affect the results. Re-audit the patch paths and variant patterns when updating
the animal mods.
