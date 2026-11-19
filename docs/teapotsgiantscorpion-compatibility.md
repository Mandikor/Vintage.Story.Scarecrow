# Teapot's Giant Scorpion compatibility

Baseline: supplied **Teapot's Giant Scorpion 1.0.2**
(`teapotsgiantscorpion`). Both `scorpion-surface` and `scorpion-cave` are covered.
No separate young, tame or owned forms exist in the supplied entity definition.

## Behavior

The five original player-target tasks receive isolated copies targeting
`scarecrow`, `little-scarecrow` and `strawdummy`:

- Poison sting using `teapotsgiantscorpion-poisonmeleeattack`.
- Claw attack using the same native task class without configured poison.
- Charge using `teapotsgiantscorpion-charge`.
- Injury-state pursuit using `teapotsgiantscorpion-seek`.
- Ordinary pursuit using the same native climbing task.

Scorpions can approach and attack scarecrows; no flee behavior is introduced.
Original player/hyena/wolf/bear tasks remain unchanged. Copies retain emotion
conditions/triggers, priorities, cooldowns, sounds, animation, damage, poison,
climbing and charge mechanics. Retaliation is disabled only in the copies,
preventing those tasks from selecting unrelated attackers.

The native melee vertical-distance setting remains **1.75**. Charge retains
**16-block** seeking range and **2-block** reach distance; pursuit retains
**24 blocks** for the injury-state task and **20 blocks** for ordinary pursuit.
Surface/cave sting damage remains 4/6, with native poison damage 3/6 over
60/120 seconds and 30/60 ticks. These are upstream settings, not new bonuses.

## Custom task review

The supplied DLL's poison task derives from the standard targetable melee task;
it does not limit hits to `EntityPlayer`. Its poison effect requires an actual
health decrease and a surviving target with a health behavior. Climbing seek
derives from standard entity pursuit, and charge extends that climbing task.
These original classes can use the additional JSON target codes without a C#
adapter. Their attack and poison implementation is unchanged.

The only patch file is
`compatibility/teapotsgiantscorpion/patches/scorpion.json`. It appends five tasks
to the existing server task-AI array. Spawning, ore-density rules, loot, original
AI and player poison-cure behavior remain unchanged. No spawn prefixes are added.

## Validation

Automated checks apply the shipped operations to the supplied definition and
use the game's actual variant resolver for both variants: five copies and ten
resolved task configurations. They verify complete original-definition
preservation, isolated targets and retained reach/poison/movement settings.
Reflection against the actual mod DLL verifies the native task inheritance.
Build passes; the author completed the in-game test successfully.

Test both surface/cave forms against both scarecrow sizes and a straw dummy,
including ordinary pursuit, charge, claws and sting. Check normal reactions to
players remain intact. Actual pathfinding, collision reach and damage delivery
require this gameplay test; resolver checks alone do not prove them.
