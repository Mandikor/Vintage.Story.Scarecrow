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
2. [x] **Keep equipment when picking up and relocating a scarecrow.** Store tools,
   lanterns and wind chimes in the returned item and restore their slots on placement.
   Only empty scarecrows stack; equipped ones use individual slots, filling the
   hotbar before the backpack. Damage drops attachments separately and Shift + an
   empty hand removes individual attachments as before. These pickup and removal
   rules also apply in Creative mode; no custom middle-click feature is needed.
   Implemented; automated checks and in-game validation passed.
   Server interaction protection uses the owner UID plus claim access for pickup,
   attaching and removing equipment. Legacy ownerless entities retain claim-based
   access. The tooltip shows the owner with a bold light-blue label and normal name.
   Packed item tooltips list stored equipment in left-arm, right-arm, lantern and
   foot order; the little scarecrow lists only its two existing slots. All labels
   are localized, and empty slots are identified explicitly.
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
