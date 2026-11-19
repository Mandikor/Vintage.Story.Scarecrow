# Teapot's Undead Legion compatibility

Baseline: supplied **Teapot's Undead Legion 2.2.2** (`teapotsundeadlegion`),
which requires Pet AI 5.1.1. Scarecrows represent players for hostile undead;
this integration does not repel them or make gardens safe from them.

## Included and excluded forms

Six hostile definitions are covered: `revenant`, `revenant-brute`,
`revenant-hunter`, `spectre`, `spectre-raised` and `lich`. Hostile creatures can
approach and attack both scarecrow sizes and the vanilla straw dummy. A raised
spectre is a hostile Lich minion; it is distinct from a player-summoned spectre.

Bound `revenant-tame`, `revenant-tame-brute` and `revenant-tame-hunter`, the mule,
palanquin, player-summoned spectres and the hunter-javelin entity are unchanged.
No additional autonomous scarecrow targeting is given to these companions.
Original owner commands, ally checks and defensive behavior remain in effect.

## Native AI integration

Six JSON files under `compatibility/teapotsundeadlegion/patches/` append
`scarecrow`, `little-scarecrow` and `strawdummy` to 21 existing player-target
combat lists and nine explicit player wake/idle-interruption lists.

The existing tasks are extended directly instead of duplicated. This preserves
one set of cooldowns and internal state per attack, including the hunter's
javelin, disengage and hold-range behavior. Twelve player-containing wildcard
lists already accept scarecrows and require no additions. Existing wildcard-only
pursuit, exclusions and all original player/prey targets remain unchanged.

The hunter retains native ranged attacks and spacing; disengagement is its
existing ranged-combat maneuver, not newly introduced fear. Spectres retain
weapon/shield-specific settings and the sword-only attack gate. Revenants retain
tier/equipment settings and native hostility. No flee tasks or spawn-blocking
prefixes are introduced.

The supplied DLL was reviewed for custom targeting. Hunter tasks use configured
target lists and retain owner/ally checks. Raised spectres keep their existing
raiser-defense and command-target paths. The Lich's raise-dead task detects
existing combat targets, and its lightning task collects targets from itself
and its raised minions. Those mechanics can therefore react to acquired
scarecrow targets without duplicating spells or changing C# code. Spell damage,
area effects, summoning limits and cooldowns remain upstream behavior.

## Validation

Automated checks apply all **90 append operations** to the supplied assets and
use the game's actual variant resolver for **180 hostile variants**. After
removing only the appended targets, complete resolved definitions match their
originals, including task counts, damage, ranges, conditions and animation.
The **73 companion/nonliving variants** have no matching patch files.
Build passes; the author completed the in-game test successfully.

Test melee revenants/brutes, hunter javelins and spacing, spectres, Lich-raised
minions and Lich combat near both scarecrow sizes and the straw dummy. Confirm
bound companions retain their original commands and behavior. Actual projectile
hits, collision reach and spell effects require gameplay validation.
