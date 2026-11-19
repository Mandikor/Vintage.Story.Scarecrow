# Flocks compatibility

Reviewed against the supplied **Flocks 0.22.1** (`flocks`) assets and DLL.

## Native birds

Flocks already recognizes living `scarecrow:scarecrow`,
`scarecrow:little-scarecrow` and `game:strawdummy` entities. Its corvid behavior
avoids protected crop raids, feeding grounds and landing/perching destinations
and leaves protected areas. The resident-crow spawn check rejects protected
locations. Dead scarecrows are ignored.

This uses Flocks' own `ScarecrowRadius`, **15 blocks by default**, for all three
types. It is independent of Scarecrow's spawn radius and equipment bonuses;
the displayed spawn ring does not represent Flocks' radius. Flocks supplies its
own translated scarecrow handbook additions.

All seven supplied birds were reviewed: crow, eagle, greylag, gull, heron,
mallard and owl. They use custom corvid, raptor, waterfowl and owl behaviors.
Only corvid behavior and the resident-crow spawn veto use the scarecrow helper.
The other birds' player searches do not treat scarecrows as players. No generic
flee tasks are added to interfere with native flying, hunting or lake wariness.
Full player-equivalent reactions for these birds would require a separate
custom-AI integration and are not implemented here. No separate domesticated
entity definitions were found in this version.

## Mouse patch

The mouse uses standard task AI. Its player/predator flee task is copied with
only the three scarecrow targets, retaining priority, speed, animation and its
**8-block** seeking range. Only the copy disables damage-triggered instant
flight and retaliation, preventing reactions to unrelated attackers. Original
player, predator and owl tasks remain intact.

The sleeping task's wake list retains player/predator entries and adds the three
scarecrow targets at its native **5-block** wake range. The patch is in
`compatibility/flocks/patches/mouse.json`. No C# adapter or new natural-spawn
prefixes are added; mouse spawning is unchanged.

## Validation

Automated checks apply the shipped operations to the installed mouse definition,
verify original tasks remain unchanged, and invoke the actual Flocks DLL to
check living/dead target recognition and the default radius. Build passes.
The author completed the in-game test successfully.

Suggested checks: mouse flight/sleeping near both sizes and a straw dummy,
natural crow visits/crop raids inside and outside Flocks' configured radius,
and unchanged movement of other birds.
