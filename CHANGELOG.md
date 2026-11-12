# Changelog

All notable changes to the Scarecrow mod, newest first. The version numbers match
`scarecrow/modinfo.json`, the dates are the dates of the release commits.

## Unreleased

- Documented the pattern rules for the attachable item list in the source code.

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
