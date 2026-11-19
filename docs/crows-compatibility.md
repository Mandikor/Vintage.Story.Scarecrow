# Crows compatibility

## Supplied package

The integration targets the supplied **Crows 0.2.7**, manifest ID `crows`, from
`02_SUPPORTING_CREATURE_MODS/CrowBros_v0.2.7`. The current ZIP and unpacked crow
entity JSON match. The checked DLL SHA-256 is
`A27EDF33A8184A843F4BE15876B26CA8708158FC306B8691FD6D4F1D59DF3655`.
Use the actual supplied package as the baseline: the earlier library snapshot
under the same declared version did not capture this native integration correctly.

Crows already registers the large and little scarecrow in its own
`attributes.otherScarecrows` map. Its `CrowScare` helper is used by the native
flight, landing/perching and feeding logic. Additional generic flee tasks or a
replacement flight behavior are unnecessary.

## JSON changes

All changes are in `compatibility/crows/patches/crow.json` and are server-side:

| Entity | Native exclusion/guard radius | Remembered avoidance radius |
| --- | ---: | ---: |
| Full-size scarecrow | 16 | 18 |
| Little scarecrow | 12 | 16 |
| Vanilla straw dummy | 8 | 12 |
| Crows' own scarecrow | 8, unchanged | 12, unchanged |

The little scarecrow's native registration was 8/12; the patch updates it to
12/16 to match Scarecrow's current 12-block base radius and retain its four-block
avoidance buffer. The full-size 16/18 entry retains Crows' authored values.
The vanilla straw dummy gains an entry in the same native exclusion mechanism,
in addition to the original straw-dummy flee task that remains intact.

Each external map entry is added/replaced individually, preserving registrations
from other mods. The existing player-triggered idle wake list also gains the
three scarecrow codes. Its player target, range and original task remain.

These are **Crows' own AI exclusion zones**, not a guarantee that the whole area
will always be empty of crows. Native ground/perch takeoff distances and flight
scheduling are unchanged. The two-game-hour remembered avoidance duration is
unchanged. Equipment bonuses do not enlarge these AI zones, and crows are not
added to Scarecrow's default hare/raccoon natural-spawn blocking list.

Player trust, grudges, gifts, curiosity, attacks, crop raids, food selection and
egg theft keep their original AI settings. Scarecrows are recognized through
Crows' own stationary-threat mechanism, rather than spoofing a player identity.
No Crows-specific C# adapter or dependency on `crows.dll` is shipped by Scarecrow.

## Validation and game test

Local checks apply the shipped JSON operations to the actual supplied entity
file and compare all untouched data, including player/social and flight tasks.
Tests invoke the real Crows DLL's `CrowScare.IsScarecrow`, `Covers`, `Remember`,
`Avoiding` and the flight task's threat check. They verify all three codes and
radii, dead-target exclusion, zone boundaries, memory expiry, missing external
types and unchanged values for Crows' own scarecrow. The upstream files remain
untouched. Build and automated checks pass, and the author completed the in-game
test successfully.

Suggested in-game test:

1. Place each scarecrow near a crop/food/perch area and check native takeoff,
   exclusion from eating/landing and remembered avoidance after being scared off.
2. Check the little scarecrow near the eight-to-twelve-block band and compare
   the full-size scarecrow and straw dummy with the table above.
3. Let crows idle near a scarecrow, then remove/destroy it and confirm normal
   behavior can resume after native avoidance memory expires.
4. Check that real-player feeding, trust and other social behavior still works.

The automated checks do not simulate complete navigation or every mod combination.
