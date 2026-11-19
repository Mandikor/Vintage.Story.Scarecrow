# Changelog

All notable changes to the Scarecrow mod, newest first. The version numbers match
`scarecrow/modinfo.json`, the dates are the dates of the release commits.

## Unreleased

### Vara's Arctic Wolves: native AI reactions

- Revised the existing 1.5.0 integration to use every player-target AI task as
  its basis: six adult tasks and one pup flee task, preserving native priorities,
  damage, timing, movement, animations, temperature ranges and emotion conditions.
- Added scarecrow targets to both existing sleep wake-up lists while retaining
  player detection. Original player/prey tasks remain intact; added scarecrow
  tasks cannot inherit arbitrary attackers or trigger instant flight on damage.
- Kept all patches under `compatibility/varasarcticwolves/patches`, with no C#
  adapter or changes to the natural-spawn blocking configuration.

### Source organization and contributor documentation

- Grouped optional-mod C# adapters under `src/combat`, including Wind Chimes
  rendering, spatial sound and attachment hooks. Shared inventory rules remain
  in `Wearables`; JSON patches retain their per-mod compatibility directories.
- Moved interaction feedback to `Interactions`, documented important code
  sections and added a source structure and extension guide. Registered names,
  namespaces, save formats and gameplay behavior are preserved.

### Versioned configuration migration

- Added `ConfigVersion` schema 2. Existing 1.9.3 configs without a version are
  treated as schema 1 and upgraded automatically, adding only missing settings
  with defaults while preserving customized values and unknown properties.
- Back up the original configuration to `scarecrow.json.bak` before upgrading.
  Current complete files are left untouched; files with newer schema versions
  are never downgraded or rewritten. Invalid JSON, invalid known settings or
  invalid version values are not overwritten.
- Retain the loaded runtime settings if a backup or save fails. Missing settings
  are also repaired in partially edited configs already marked with the current
  version; case-insensitive property names do not create duplicate entries.

### Cleanup after scarecrow destruction

- Synchronize the empty equipment inventory after death and stop rendering dropped
  attachments and wind chimes on dead scarecrows. Dead scarecrows no longer offer
  equipment or pickup hints, and natural-spawn interception stops immediately.

### Interaction failure feedback

- Added short in-game error messages for placement blocked by solid blocks or
  living entities, unsuitable equipment, occupied attachment points, owner-only
  access and denied claim access. Messages use the requesting player's language;
  all 33 existing language files include the six new messages.
- Server-authoritative feedback avoids duplicate client-prediction messages and
  never writes to chat. Repeated attempts at the same failure remain suppressed
  until a 2.5-second gap; a different failure is shown immediately. Successful
  actions remain silent, and failed placement or attachment preserves the items.

### Environment-dependent wind chime audio

- Attached wind chimes now remain silent in calm weather and play their original
  recordings once with randomized pauses instead of continuously looping. Light
  wind creates longer pauses; strong wind shortens them, including existing pauses.
- Added cover-based sound attenuation for the scarecrow and listener, respecting
  Wind Chimes' `WindChimeIndoorVolume` and `DisableIndoorMuffle` settings. Active
  sounds update their volume as cover changes. Increased the outdoor volume boost
  from 10% to 50% of the configured held-item volume, capped at full volume.
  Hearing range, ambient channel and mute behavior are preserved.
- Kept environmental audio client-only and optional, with no weather queries when
  Wind Chimes is absent or no chime is attached. Weather queries and spatial sound
  positions preserve dimensions. Rendering, wind animation and spawn protection
  are unchanged.
- In-game testing confirmed environmental playback and the adjusted volume.

### Ownership and server interaction protection

- Applied the same owner-UID and claim-access checks to targeted pickup, attaching
  equipment and removing equipment, including attachment client packets. Denied
  interactions no longer fall through to vanilla attachment handling. Existing
  ownerless scarecrows retain claim-based access; damage handling is unchanged.
- Added an owner tooltip line with a bold light-blue label and a normally formatted
  player name. Names are synchronized by the server and resolved for offline owners
  from saved player data. Added owner, ownerless and unknown-name text to all 33
  language files. Existing spawn-protection labels retain the radius ring's color.

### Equipment preservation when relocating

- Added a stored-equipment section to the inventory tooltip of packed scarecrow
  items: left arm, right arm, lantern slot and foot slot, in that order. Little
  scarecrows list only their existing lantern and foot slots. Uses native item/mod
  names, marks empty slots and safely handles unavailable items without changing
  saved equipment. Added all labels to the 33 existing language files.

- Targeted pickup with Ctrl + an empty hand now packs attached equipment into the
  scarecrow item. Placement restores attachments to their original slots with
  durability and custom item attributes intact, including supported mod equipment.
- Only empty scarecrows can stack. Equipped scarecrows occupy individual slots,
  even with identical equipment. Pickup fills the hotbar before the backpack;
  if both are full, the packed item drops at the original position.
- Separated targeted pickup from death through damage: damage still drops equipped
  items separately. Shift + empty-hand removal of individual attachments is unchanged.

### More Lanterns compatibility

- Added optional support for More Lanterns by Ape (`apelanterns` 1.4.4): all eleven
  wall/ceiling-capable metal and paper models, covering 128 upright inventory
  variants with their available colors, on/off states and brightness levels.
  Floating lanterns, stone lanterns and lantern parts are excluded.
- Both scarecrow sizes accept these lanterns at the neck. The full-size scarecrow
  also accepts one in each arm slot instead of a tool, allowing up to three lanterns.
  Existing tool attachment points and equipment categories are preserved.
- Paper lanterns add two blocks to natural-spawn protection and metal lanterns add
  four. Only the strongest equipped lantern's bonus applies, including arm lanterns.
  Removing a lantern immediately uses the strongest remaining bonus or restores
  the base radius. The radius overlay follows automatically; animal flee and
  attack distances are unchanged. On/off and brightness variants use the same bonus.
- Reused More Lanterns' hanging models and variant textures directly. Separate
  texture prefixes allow differently colored lanterns in all three slots without
  texture conflicts. World-block geometry remains unchanged.
- Increased the attached Chinese and decorated Chinese paper lanterns and all metal
  lanterns by 50% over the initial attachment scale. Basic and Medieval 1 Small paper
  lantern sizes are preserved. Counter-rotated both arm mounts so lanterns hang
  vertically in the resting pose without moving their hanging points.
- Kept all four compatibility JSON patch files under
  `assets/scarecrow/compatibility/apelanterns/patches`. Added a lantern compatibility
  guide and automated checks; in-game tests confirmed attachment, scaling, radius
  bonuses and corrected arm rotation.

### Added

- Added a localized protection tooltip for placed scarecrows, little scarecrows
  and straw dummies. It shows the effective natural-spawn radius, base radius,
  strongest equipped bonus and configured animal groups, or an explicit disabled
  state. Server settings are synchronized through entity attributes so multiplayer
  tooltips do not use a client's local spawn list.
- Added tooltip translations to all 33 existing language files. The new section
  starts with a blank line; fixed labels are bold in the radius ring's gold color,
  while numbers and translated equipment/animal names retain normal formatting.
  Equipment changes update the display without chat messages.
- Added optional Wind Chimes by Ape (`windchimes` 1.5.3 and 1.5.4) support for both arm slots
  of the full-size scarecrow: bamboo, wood, bone, brass, copper and crystal models,
  covering all eleven variants in 1.5.4 and the ten variants in 1.5.3.
  Original shapes and textures are reused with upright
  arm mounts, independent of More Lanterns; both compatibility packs can coexist.
- Attached wind chimes play their original material recordings as spatial ambient
  sound, using Wind Chimes' held volume and hearing-distance settings. Sounds follow
  the scarecrow and stop on removal, death, despawn or leaving hearing range.
  Wind chimes add six blocks to natural-spawn protection, taking precedence over
  lanterns without stacking. Removing one uses the strongest remaining equipment
  bonus; the radius outline updates automatically. All compatibility
  JSON patches are kept under `compatibility/windchimes`.
- Check Wind Chimes availability once during attachment initialization. Clients
  without the mod skip periodic audio inventory scans and sound processing entirely;
  servers never enable the sound path.
- Included the bone wind chime from the supplied 1.5.4 package, which is absent
  from the upstream 1.5.3 assets. It retains the original bone textures and the
  wood recording selected by that version's held-sound definition. Older versions
  without the bone model remain supported.
- Attached small vanilla lanterns now increase natural-spawn protection by two
  blocks; large lanterns add four. Both scarecrow sizes and the radius overlay use
  the current attachment bonus, which disappears when the lantern is removed.
  Animal flee and attack distances are unchanged.
- Added animal compatibility for Vara's Arctic Wolves, Bats, Equus: Shared Ancestry,
  Equus: Expanded, Feverstone's Horses, Feverstone Wilds, Sonya's Quality Fowl,
  More Animals, Teapot's Giant Boar, Giant Redriver and Great Bighorn, The Critters
  Pack coral fork, and Truth and Beauty: Detailed Animals.
- Added optional rules for 84 animal definitions and native feeding-deterrent
  settings for two bat definitions. Small and young animals flee; large herbivores
  defend during injury aggression; large predators approach and attack. Separate
  tame, aquatic and non-animal definitions retain their original behavior.
- Added regular JSON patches in separate mod-ID folders under `compatibility`,
  following the existing optional-mod asset structure. Existing mixed-target AI
  tasks retain their other targets. Generated ponies and chicks inherit existing
  rules; no additional C# loader or hard mod dependencies are required.
- Preserved native griffon flight attacks and bat feeding AI, with bat deterrent
  ranges aligned to 16/12/8 blocks. Documented supplied versions and exclusions.

### Fixed

- Corrected Wind Chimes renderer selection to patch server-loaded entity types,
  rather than only the client asset copy. Only remove wind chimes from the normal
  entity mesh when the separate renderer is actually active, preventing invisible
  attachments if that renderer is unavailable.
- Render attached wind chimes separately with the normal block shader so their
  authored wind modes and moving shape regions match the ceiling variants. The
  entity shader otherwise reinterprets their wind flags. Original block models
  remain unchanged; arm alignment, scale and shadows are retained.
- Removed blanket scarecrow fleeing from adult vanilla black, brown, polar and sun
  bears, wolves and hyenas. They now approach and attack scarecrow targets while
  respecting their native injury-fear state; pandas use injury-triggered defense.
- Corrected vanilla age and species selection so elder boars use mature-animal
  rules, smaller deer and goats flee, and young animals cannot inherit adult attacks.
- Corrected animal-mod scarecrow reactions that made small predators attack or left
  large herbivores without injury-triggered defensive attacks. Young animals remain
  on flee-only rules.
- Corrected the extinct duck `chenonettafinschi` movement pattern and selected an
  available charge animation for young elephants in the compatibility patches.
- Scarecrow and little scarecrow placement now checks the full collision box against
  solid blocks and overlapping living entities before consuming the item. Empty
  entity-intersection results correctly allow placement.
- Restored compatibility with vanilla stands by removing the global attachment-category
  overrides. Tools retain their original game or mod categories.
- Forbidden tools are rejected on both the client and server, and attachment hints
  only show items allowed at the selected attachment point.

### Changed

- Increased attached wind chime sound volume by 10% relative to the configured
  held-item volume, capped at full volume. A zero volume remains silent.
- The enabled radius overlay now refreshes automatically every half second after
  placement, movement, removal or changes to nearby loaded scarecrows. It stays
  enabled when no scarecrow is nearby, updates highlights only when the outline
  changes, and stops refreshing when switched off.
- Audited all 36 installed vanilla animal entity files and revised the 13 existing
  vanilla animal patch files, covering 25 definitions and 181 variants. Applied the
  same size, diet and injury-state rules as the animal-mod compatibility patches.
- Preserved vanilla attack damage, animations, timing and variant-specific cooldowns.
  Vanilla task priorities are scaled to each animal's original combat priorities;
  added tasks remain restricted to scarecrow targets. Fish, invertebrates, separate
  tamed elk definitions and debug bots retain their original specialized behavior.
- Audited and revised 46 animal compatibility patch files against the supplied assets
  of the 18 already supported animal mods. Reactions are selected by species size,
  diet and age: herbivores flee, large herbivores can defend themselves while in
  `aggressiveondamage`, small predators flee, and large predators approach and attack.
- Preserved native movement, attack damage and timing where applicable, aligned flee
  ranges to 16/12/8 blocks, and separated species masks so smaller animals and juveniles
  cannot inherit adult attack tasks. Documented the classification and audited mod
  versions in `docs/animal-compatibility.md`.
- Scarecrow attachments now extend the vanilla attachment system with an item-code
  whitelist per slot. The `allowedCodes` lists are defined in the entity JSON files
  and support domain-qualified wildcard patterns.
- Slot categories are derived from the permitted collectibles without replacing their
  original categories or attachment interfaces. A fallback category is supplied only
  for previously uncategorized items with scarecrow-specific attachment shapes.
- Existing attachments are preserved when loading a world and remain removable after
  a whitelist changes.
- Leaf Litter rake attachment shapes and textures now come from a compatibility JSON
  patch instead of the previous dedicated C# registration. The patch no longer
  overwrites the rake's category.
- Explicitly enabled the Art of Growing hayfork attachment patch.
- Added a reference to the game's survival assembly for the vanilla attachment classes.
- Added attachment regression checks covering vanilla stand compatibility, whitelist
  enforcement and category preservation. The `tests` directory is kept local and
  excluded from version control and distribution.

### Documentation

- Updated the MIT copyright notice to cover 2023-2026; the license terms are unchanged.
- Expanded the README with installation instructions, protection radii, animal behavior,
  crafting, controls, equipment, configuration, attachment rules, optional mod
  compatibility, contribution guidance and license information.
- Corrected the little scarecrow's documented protection radius to 12 blocks and
  clarified the difference between natural-spawn protection and animal flee behavior.
- Removed the build-from-source instructions and the local regression-test command
  from the public README.

## 1.9.4 (2026-11-19)

- Fixed animals fleeing from players after taking damage, including bears.
  Scarecrow-specific flee tasks no longer trigger instant flight when an animal
  is hurt and do not react to attackers outside their configured scarecrow targets.
- Applied the correction to vanilla animals and all existing animal-mod patches.
  Existing scarecrow detection ranges, movement settings and natural-spawn
  protection remain unchanged.

## 1.9.3 (2026-11-13)

- Scarecrows no longer remove animals that are born nearby or released from a cage. The spawn
  protection now only affects natural spawns, while newborn and player-placed animals simply
  flee from the scarecrow.
- Vanilla chickens, bears, foxes, hyenas and wolves now flee from scarecrows, little
  scarecrows and straw dummies. Flight distances and priorities were aligned with the
  existing animal patches, and the gazelle, sheep, pig and raccoon patches were corrected.
- Documented the pattern rules for the attachable item list in the source code.
- Ctrl+H (rebindable) toggles an outline on the ground that shows the area a scarecrow keeps
  animals away from. All scarecrows within 64 blocks are shown at once, and the outline follows
  the block grid instead of drawing a circle.
- French, Japanese, Russian and Ukrainian translations completed and corrected against the
  English file, and a typo in the English item description fixed.
- Added 27 more languages: Arabic, Belarusian, Bulgarian, Chinese (Simplified and Traditional),
  Czech, Danish, Dutch, Esperanto, Finnish, Hungarian, Icelandic, Italian, Korean, Lithuanian,
  Norwegian, Polish, Portuguese (Brazil and Portugal), Serbian, Slovak, Spanish (Spain and Latin
  America), Swedish, Thai, Turkish and Vietnamese. These translations are a best effort, so
  native review is welcome.

## 1.9.2 (2026-11-12)

- Attachment categories for the scarecrow slots are now part of the mod instead of the
  shared `attachableToEntity` item data. Another mod that patches that field can no longer
  stop scythes, hoes, bug nets, snow shovels, watering cans, hay forks or rakes from being
  attached.
- The vanilla `categoryCode` of those items is left untouched again.
- The Leaf Litter rake configuration no longer writes a category code either; shapes and
  textures still come from the json patches.

## 1.9.1 (2026-11-11)

- The Leaf Litter rake can be attached to the scarecrow arm slots.
- Compatibility with the FotSA mod family: Bovinae, Capreolinae, Cervinae, Moschidae,
  Elephantidae, Rhinocerotidae, Meiolaniidae, Vombatidae, Caninae, Casuariidae,
  Dinornithidae, Machairodontinae, Pantherinae, Thylacinidae and Viverridae.
- Flight distances and the Bovinae behavior adjusted.
- Scarecrow entities refactored, a spawn hook leak fixed and a mod config added.

## 1.9.0 (2026-11-08)

- Lanterns, watering cans, scythes and hoes can be attached to the scarecrow, with
  attachment points, selection boxes, shift-attach and drops on death.
- Bug nets and snow shovels fit both arm slots; new models, textures and mirror variants
  added; the scythe and hoe attachments revised.
- Hayforks and rakes can be attached to the arm slots.
- Equipment is set aside when the scarecrow is removed.
- Placement hints added.
- Scarecrow models and animations extensively revised; scarecrows and the straw dummy
  expanded.
- Recipe requirements adjusted, grass bundle requirements reduced.
- Mannequins accept scythes and hoes.
- Support for Plumpkins, Art of Growing and the Art of Growing Breeding Addon.
- AI: per-target flight and attacks for the Ferus Wild, per-entity behavior for the wild
  monoceros, per-target behavior for the wild horse.
- Escape behavior for Nandiniidae and Viverridae.
- Preparation for the whole FotSA mod family.

## 1.8.3 (2026-10-01)

- Mod description updated.

## 1.8.2 (2026-05-24)

- Flee entity tasks for animals expanded.

## 1.8.1 (2026-05-14)

- Support for Art of Growing, the Art of Growing Breeding Addon and Grass to Ropes
  restored.
- New recipes, escape behavior for baby monoceros and updated flee entity logic for the
  wild monoceros.
- Refactoring and style improvements, unused files removed.

## 1.8.0 (2026-04-25)

- Updated to .NET 10.0.
- File structure adapted to the vanilla layout, code cleaned up.
- Straw dummy recipe patch rewritten for better compatibility.
- Support for the patched versions of the Art of Growing mods.
- Flee conditions of supported mods adjusted.
- ConfigLib text removed.

## 1.7.1 (2025-10-12)

- Updated to Vintage Story 1.21.1, config removed.
- `modinfo.json` completed; the mod now ships as a DLL.

## 1.7.0 (2025-08-31)

- Updated to Vintage Story 1.21.0 and .NET 8.

## 1.6.10 (2025-06-09)

- Support for the Grass to Ropes mod.
- Support for the Monoceros and Equus mods.
- Update for FotSA: Viverridae.
- All vanilla creatures that eat seeds, fruit or vegetables added to the list of animals
  the scarecrow scares away.

## 1.6.9 (2025-06-01)

- Gazelles added to the list.
- Basic support for the FotSA: Viverridae Plus mod.
- Patching corrected.

## 1.6.8 (2025-05-26)

- Support for the Art of Growing mod and the Plumpkins x Art of Growing: Breeding Addon.
- Recipes finalized for those mods.
- Basic game version set back to 1.20.0.

## 1.6.7 (2025-05-24)

- Support for the Plumpkins x Art of Growing: Breeding Addon.

## 1.6.6 (2025-05-24)

- Recompiled for Vintage Story 1.20.10.
- Adjustments after the game update.

## 1.6.5 (2025-02-09)

- Recipes with dry grass and thatch added.
- Ukrainian translation corrected.

## 1.6.4 (2025-02-07)

- The scarecrow can only be picked up and placed with an empty hand.
- Ukrainian translation by m1RAGE added.
- `LICENSE.txt` updated.

## 1.6.3 (2025-01-26)

- Russian translation by FNX added.
- French translation fixed.

## 1.6.2 (2025-01-25)

- French translation by Mokradin added.

## 1.6.1 (2024-12-07)

- Updated to Vintage Story 1.20.0-rc.1.

## 1.6.0 (2024-11-13)

- Recompiled for Vintage Story 1.20.
- Configuration system cleaned up and a configuration error fixed.
- Texts for the ConfigLib mod added to the language files.
- The unnecessary activation of the scarecrows removed.
- Namespaces changed and `.editorconfig` introduced.
- `LICENSE.txt` updated.

## 1.5.1 (2024-08-18)

- Some additions.
- English language file updated.
- Mod icon fixed.

## 1.5.0 (2024-08-11)

- First release tracked in this repository.
- Straw dummy integration fixed, shapes renamed and adjusted to the original size, json
  additions and cleanup work.
