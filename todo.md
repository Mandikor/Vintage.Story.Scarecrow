# Scarecrow ideas

Status for the `dev` branch. Deferred items require renewed discussion before
implementation. Tool bonuses were removed from the plan at the author's request.

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
3. [x] **Make attached wind chime audio respond to the environment.** Silent in
   calm weather; original recordings play once with longer pauses in light wind
   and shorter pauses in stronger wind. Cover at the scarecrow or listener reduces
   volume using Wind Chimes' existing indoor settings. The outdoor volume boost is
   now 50%; optional-mod gating, animation and spawn bonus are preserved.
   Implemented; automated checks and in-game validation passed.
4. [ ] **Add visual customization — deferred.** Explore additional hats, heads,
   clothing and wood variants suitable for gardens and farms. No implementation
   is currently planned.
5. [ ] **Add optional wear and repair — deferred on dev.** The aging and repair
   implementation remains exclusively on `dev-aging`, including staged shapes,
   condition tooltips and persistent collapsed remains. It is not included in
   `dev`; further shape work and any future integration are postponed.
6. [x] **Improve interaction feedback.** Short in-game errors distinguish placement
   blocked by solid blocks or living entities, unsuitable equipment, occupied
   attachment points, owner-only access and denied claim access. Existing allowed-
   item interaction hints remain available. All 33 languages are covered; feedback
   is server-authoritative, does not write to chat and suppresses repeated failures.
   Implemented; automated checks and in-game validation passed.
7. [x] **Preserve user settings through configuration upgrades.** Added schema
   versioning, missing-default migration and a backup before changing an existing
   file. Customized values and unknown properties are preserved. Invalid files
   and newer schemas are not overwritten. The dev configuration has no aging
   options. Implemented; build and automated migration checks passed.
8. [x] **Clean up destroyed scarecrows.** Synchronize the empty equipment inventory
   after death and stop displaying dropped attachments and wind chimes. Remove
   pickup and equipment hints from dead scarecrows and end natural-spawn
   interception immediately. Implemented; build and automated inventory/drop and
   wind chime checks passed.
