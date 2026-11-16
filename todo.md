# Scarecrow ideas

Remaining items are proposals for discussion, not approved implementation work.

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
2. [ ] **Keep equipment when picking up and relocating a scarecrow.** Store tools,
   lanterns and wind chimes in the returned item and restore their slots on placement.
3. [ ] **Give tools distinct benefits.** Consider small natural-spawn radius bonuses
   or improved durability, with a clear rule for combining equipment effects.
4. [ ] **Make attached wind chime audio respond to the environment.** Consider
   silence in calm weather, occasional sounds in light wind, more frequent sounds
   in stronger wind, and reduced volume under cover.
5. [ ] **Add visual customization.** Explore additional hats, heads, clothing and
   wood variants suitable for gardens and farms.
6. [ ] **Add optional wear and repair.** Animal attacks could cause visible damage;
   straw and sticks could repair it. Make maintenance optional.
7. [ ] **Improve interaction feedback.** Explain failed placement due to blocks or
   creatures and identify permitted equipment when an attachment is rejected.
