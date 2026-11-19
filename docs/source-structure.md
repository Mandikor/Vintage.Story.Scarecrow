# Source structure and extension points

This guide describes the `dev` branch, which does not include scarecrow aging.
Folder placement describes responsibility; C# types retain the `Scarecrow`
namespace and their existing names so registered behavior names and saved data
remain compatible. The `combat` folder contains optional-mod compatibility code.

## C# responsibilities

| Location under `scarecrow/src` | Responsibility |
| --- | --- |
| `Core.cs` | Register item/entity/behavior names on both sides, load configuration, finalize collectible metadata and delegate optional client registrations. |
| `Config/` | User settings and versioned JSON migration. Preserve unknown properties and custom values; back up existing files before migration writes. |
| `Entities/` | Entity lifecycle, ownership and claims, localized protection information, natural-spawn interception and pickup interactions. |
| `Item/` | Item placement and collision checks, stored-equipment tooltips, stack rules and hotbar-first pickup transfers. |
| `Wearables/` | Shared inventory, whitelist/category checks, equipment persistence, radius bonuses and death cleanup. |
| `Interactions/` | Localized server feedback and per-player repeat suppression. |
| `Client/` | Optional radius highlights controlled by Ctrl+H; no server-side rendering. |
| `combat/ModCompatibility.cs` | Central entry point for optional client registrations and server entity finalization. |
| `combat/windchimes/` | Wind Chimes sound, block-shader rendering and attachment lifecycle hooks. |
| `combat/equusponies/` | One-time server adaptation of resolved wild Equus: Expanded types after breed-specific AI generation. |

`ScarecrowAttachments` is a partial class: the shared behavior lives in
`Wearables/ScarecrowAttachments.cs`, while
`combat/windchimes/ScarecrowAttachments.WindChimes.cs` implements the optional
sound lifecycle and renderer-specific gear exclusion. Both files compile into
the same type, preserving the registered `scarecrowattachments` behavior and
inventory format. Shared inventory hooks call the small compatibility helpers;
optional-mod detection, settings, sound handles and rendering decisions stay in
`combat`.

## Assets and optional mods

JSON-only integrations stay in
`scarecrow/assets/scarecrow/compatibility/<modId>/`. This includes More Lanterns,
animal mods, recipes and other asset patches. Do not add a C# adapter when JSON
patches provide the complete integration. Vanilla patches remain under
`assets/scarecrow/patches/`.

- Add permitted item patterns to the relevant `wearableSlots` whitelist through
  compatibility patches. Patterns include the mod domain.
- Preserve existing `attachableToEntity.categoryCode` values and collectible
  interfaces. The shared attachment behavior derives accepted categories from
  resolved collectibles rather than replacing another mod's category.
- Optional equipment can provide `scarecrowRadiusBonus` in its attributes. The
  shared behavior clamps the bonus to 0–6 and applies only the strongest bonus.
  This extends natural-spawn protection, not animal AI detection distances.
- Wind Chimes also uses the `scarecrowWindChime` marker. Its adapter reads the
  original sound settings without writing the other mod's configuration, plays
  positional recordings on the client and disposes sounds and meshes on removal,
  death and despawn. The adapter is gated by the installed `windchimes` mod.

See the [lantern](lantern-compatibility.md),
[wind chime](windchime-compatibility.md) and
[animal](animal-compatibility.md) guides for integration-specific asset rules.

## Lifecycle and synchronization

1. Register the stable JSON-facing class names in `Core.Start` on both sides.
2. Finalize attachment metadata after JSON patches in `AssetsFinalize`.
3. Initialize each entity's filtered inventory from its own copied behavior
   configuration. Never mutate shared entity definitions while preparing slots.
4. Enforce ownership, claims and item rules before server-side transfers. Keep
   client checks for prediction, but do not trust client permission decisions.
5. Serialize carried equipment by stable slot code and restore it before use.
   Damage instead drops equipment through vanilla handling, then synchronizes
   the empty inventory. Never render stale attachments on dead entities.
6. Unsubscribe spawn hooks and dispose optional sound/mesh resources when their
   lifetime ends. Radius highlights have their own client-only listener.

Refactors should preserve public type names, registered names, attribute keys,
slot order, patch paths and the distinction between natural spawning and
player-placed/newborn animals. Run the build and relevant local regression
checks before an in-game test. Local `tests/` and `scarecrow/unused/` directories
remain excluded from Git.
