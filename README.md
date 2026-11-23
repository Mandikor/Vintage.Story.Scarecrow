# Scarecrow

Keep unwelcome animals away from your garden in **Vintage Story**. Place a scarecrow
near your crops, berry bushes, beehives or chicken coop, then equip it with gardening
tools, a lantern and a watering can.

I added a full-size scarecrow and a smaller version that does not require a
pumpkin. I also gave the vanilla straw dummy a role in protecting your garden.

[Download on the Vintage Story Mod Database](https://mods.vintagestory.at/scarecrow)
· [Changelog](CHANGELOG.md)
· [Modder guide](FORMODDERS.md)
· [Report an issue](https://github.com/Mandikor/Vintage.Story.Scarecrow/issues)

## Features

- Three protection levels: scarecrow, little scarecrow and vanilla straw dummy.
- Flee and attack behavior for supported vanilla animals and animals from compatible mods.
- Natural-spawn protection against hares and raccoons by default.
- Tool holders on the full-size scarecrow, plus lantern and watering-can holders on
  both scarecrow variants.
- A keyboard shortcut to display nearby protection radii.
- Pickup and relocation with an empty hand.
- Additional crafting recipes and optional mod compatibility patches.
- Multiple interface languages.

## Requirements and installation

The current source targets **Vintage Story 1.22.0** and **.NET 10**. When downloading
a release, choose a file intended for your game version.

1. Download the mod ZIP from the [Mod Database](https://mods.vintagestory.at/scarecrow).
2. Place the ZIP in your Vintage Story data directory's `Mods` folder.
3. Start or restart the game.

For multiplayer, install the mod on **both the server and every connecting client**.
The optional mods listed below are not required to use Scarecrow.

## Choose your scarecrow

| Type | Natural-spawn protection radius | Attachments |
| --- | --- | --- |
| Scarecrow | 16 blocks | Two arm items (tools, lanterns or supported decorations) and one watering can |
| Little scarecrow | 12 blocks | One lantern and one watering can |
| Vanilla straw dummy | 8 blocks | No additional attachment slots from this mod |

These radii come from the entity definitions. Natural-spawn protection measures
distance in three dimensions, so its actual area is a sphere.

An attached **small vanilla lantern adds 2 blocks** to natural-spawn protection;
a **large vanilla lantern adds 4 blocks**. This gives the scarecrow a radius of
18 or 20 blocks, and the little scarecrow 14 or 16 blocks. Removing the lantern
removes the bonus. The radius outline updates automatically while enabled.
The bonus applies whenever the lantern is attached, during both day and night.
Animal flee and attack distances remain unchanged.

With **More Lanterns** installed, supported paper lanterns add **2 blocks** and
metal lanterns add **4 blocks** to the same natural-spawn protection. Floating
lanterns, stone lanterns and lantern parts are excluded. See
[More Lanterns support](#more-lanterns-by-ape) below for supported models and slots.
The full-size scarecrow holds vanilla and supported mod lanterns in either arm
instead of a tool, with no new neck attachments. The little scarecrow retains
its neck lantern. With several lanterns attached, only the strongest bonus applies.

### How animal protection works

Scarecrow uses two complementary mechanisms:

**Animal reactions:** supported creature mods use their native player-target AI
as the basis for scarecrow reactions. This includes player tags, variant rules,
injury/hunger states and sleep interruption. Runtime-generated creatures inherit
their supported templates; Bats uses its native feeding-deterrent mechanism.

The remaining vanilla rules generally make small animals and young animals flee. Large
herbivores can defend themselves when injury triggers aggression. Large predators,
including adult black, brown, polar and sun bears, wolves and hyenas, can approach
and attack a scarecrow. Pandas use the defensive herbivore rule.

Vanilla patches cover hares, raccoons, chickens, foxes, bears, hyenas, wolves, deer,
gazelles, goats, moose, pigs and sheep, with species-specific rules inside mixed
groups. Behavior depends on the animal's AI tasks, priorities and emotional state;
a scarecrow is not a physical barrier or a guarantee of safety from every animal.

**Natural-spawn protection:** the mod blocks natural spawns of configured animal
types within the radius. The default list contains hares and raccoons. Animals born
nearby or placed by players, such as animals released from cages, are not removed by
this protection. They can still react according to their species' AI rules.

Changing the spawn-protection list does not add flee or attack behavior to an animal. Those
behaviors are defined separately in the animal patches.

## Crafting

The simplest recipes are arranged vertically in the crafting grid:

| Result | Ingredients, from top to bottom |
| --- | --- |
| Scarecrow | Straw hat → mature pumpkin → straw dummy |
| Little scarecrow | Straw hat → straw dummy |
| Upgrade a little scarecrow | Mature pumpkin → little scarecrow |

There are also recipes using sticks with hay blocks, dry grass or thatch. The mod
enables and adjusts the vanilla straw dummy recipe and adds an alternative using
dry grass or thatch.

Use the **in-game handbook** for the exact layouts and quantities available in your
mod setup. Compatibility patches can change the available recipes or ingredients.

## Placement and controls

| Action | Default control |
| --- | --- |
| Place a scarecrow | Hold its item and right-click a block face |
| Attach an item | Hold the item, aim at its attachment point and **Shift + right-click** |
| Remove an attachment | Empty your right hand, aim at the attachment point and **Shift + right-click** |
| Pick up a scarecrow or straw dummy | Empty your right hand and **Ctrl + right-click** |
| Toggle the radius outline | **Ctrl + H**, rebindable in the controls settings |

Placement checks the space needed by the entire scarecrow. Solid blocks or
overlapping living entities prevent placement without consuming the item.
Land-claim permissions also apply.

Failed actions show a short, localized in-game error rather than a chat message:
blocked placement distinguishes solid blocks from living entities, and equipment
errors distinguish unsuitable items from occupied attachment points. Ownership
and claim failures also explain why access was denied. Holding the interaction
key does not repeatedly flash the same failure; successful actions stay silent.

A placed scarecrow records its owner. By default, only that owner can pick it up,
attach equipment or remove attachments. The owner can grant access to individual
players; claim permissions must also allow access.
These checks run on the server, including attachment packets. Old scarecrows with
no recorded owner remain accessible to players with claim permission; they are not
automatically assigned to the first player who interacts with them. Damage and
destruction still follow the game's existing rules.

To share a scarecrow, look at it from within eight blocks and use:

```text
/scarecrow access add PlayerName equipment
/scarecrow access add PlayerName full
/scarecrow access remove PlayerName
/scarecrow access list
```

`equipment` permits attaching and removing gear. `full` additionally permits
targeted pickup. Repeating `add` changes an existing player's permission. Only the
owner can change or list grants; invited players cannot pass their access on.
Names can belong to online players or players already known to the server.
Permissions are saved by player UID, so a name change does not revoke access.
Pickup preserves equipment but not grants: the next placing player becomes the
owner with an empty access list. The normal tooltip continues to show the owner;
the access list appears only when requested with the command.

Picking up a scarecrow packs its equipment into the returned item. Placing it again
restores each attachment to its previous slot, including its durability and other
item attributes. Empty scarecrows still stack up to four; equipped scarecrows stay
in separate inventory slots, even when their equipment is identical.

The inventory tooltip of a packed scarecrow lists its stored equipment under
**Inventory:**, ordered by left arm, right arm, neck and foot. Each occupied
slot uses the equipment's translated game/mod name; empty rows are omitted.
The little scarecrow lists only occupied neck and foot slots; empty scarecrows
have no extra inventory section. All labels are localized
in the 33 supported languages.

Pickup fills the hotbar first, then the player's backpack inventory. If neither has
room, the packed item drops at the scarecrow's position. Removing an individual
attachment with **Shift + right-click** works as before. Destruction through damage
still drops the attachments separately.

Pickup and individual attachment removal follow the same rules in Creative mode
as in Survival. Creative placement keeps the item as usual.

### Radius outline

The overlay shows ground outlines for loaded scarecrows and straw dummies within
64 blocks horizontally and vertically of the player. Each outline follows the
block grid on nearby solid surfaces, including slopes and built terraces. Its
horizontal circle keeps the same radius regardless of terrain height. This is an
orientation guide: actual spawn protection remains spherical, so its horizontal
reach decreases on higher or lower ground.

The display selects the nearest clear surface to the scarecrow's height in each
column. It does not draw every floor of a multilevel building or the entire sphere,
and does not show animal AI ranges. The ring also follows water surfaces.
Unloaded ground and unsupported surfaces are omitted; partial blocks such as slabs
are not modeled exactly.

While enabled, the overlay checks nearby scarecrows every half second and updates
when they are placed, moved, removed or loaded as you travel, and when the ground
changes. It remains enabled when no scarecrow is nearby, so newly encountered
scarecrows appear automatically.
Press **Ctrl + H** again to turn it off.

### Protection tooltip

The tooltip also shows the scarecrow's owner. The fixed **Owner:** label is bold
and light blue; the player name uses normal text. The server supplies the name,
including the last known name of an offline owner. Ownerless scarecrows and unknown
names have localized fallback text. Ownership is checked by player UID, not name.

Look at a placed scarecrow, little scarecrow or straw dummy to see its current
natural-spawn protection radius, base radius, strongest equipment bonus and
affected animal groups. The extra tooltip section begins with a blank line;
spawn-protection labels are bold and use the radius ring's gold color. Values and names keep
normal formatting. A blank line separates the owner from protection information.
The base-radius breakdown uses a generic equipment label and the strongest bonus
number without a plus sign or item name. After the animal groups, another blank
line introduces the inventory, listing occupied left-arm, right-arm, neck and
foot slots only. Only the inventory heading is gold; its item rows use normal
text. If no equipment is present, the entire inventory section is omitted. The
vanilla straw dummy has no equipment inventory and omits the base/equipment
breakdown. No chat messages are generated by this information display.

The tooltip updates with equipment changes and uses the server's configured spawn
list in multiplayer. An empty list is shown as disabled. Hares and raccoons have
translated group names; full entity codes use available game/mod translations.
Qualified filters show a translated group/representative animal name followed by
the mod ID in parentheses, such as **Hares (game)**. Unknown groups use a short
group-name fallback, never the complete qualified code. Plain custom prefixes
retain their previous display. All 33 existing languages include the labels.
These figures describe natural-spawn protection;
animal flee and attack behavior remains species-specific.

### Animal Cages

With **Animal Cages 5.0.1**, animals released from either cage size are treated as
player releases rather than natural spawns. A server-side Harmony patch is
installed only when `animalcages` is active. It marks the restored animal before
spawning, so naturally caught hares and raccoons are not removed when released
inside a scarecrow's protection radius. Other configured animal prefixes receive
the same treatment. Saved animal data and ordinary flee/attack rules remain intact;
natural spawns are still blocked normally. See the
[Animal Cages details](#animal-cages).

## Equipment

| Attachment point | Accepted items |
| --- | --- |
| Left and right arms, full-size scarecrow only | Scythes, hoes, bug nets and snow shovels |
| Left and right arms with optional mods | Art of Growing hayforks and the Leaf Litter rake |
| Arms, full-size scarecrow only | Small and large upright vanilla lanterns, instead of tools |
| Arms, full-size scarecrow only | Filled vanilla oil lamps in all ten colors, with original ceiling suspension, flame/smoke particles and +1 block natural-spawn radius bonus (does not stack) |
| Neck, little scarecrow only | Small/large vanilla lanterns, supported More Lanterns, Primitive Survival wooden lanterns (+2 blocks) and filled Firefly Jars (+1 block) |
| Arms with More Lanterns, full-size scarecrow only | The same paper and metal lanterns, instead of tools |
| Arms with Wind Chimes, full-size scarecrow only | Bamboo, wood, bone, brass, copper and crystal wind chimes, instead of tools |
| Arms with censership, full-size scarecrow only | Metal Open, Metal Gills, Metal Dome and Carryable censers, with smoke, light and pendulum motion; no radius bonus |
| Arms with Primitive Survival, full-size scarecrow only | Wooden lanterns (+2 blocks) and filled Firefly Jars (+1 block), with original light and wind-driven pendulum motion |
| Foot, both scarecrow variants | Fired vanilla watering cans |

Each point holds one item. The interaction hints show the items allowed at the
selected point.

The mod preserves the original attachment categories of tools and other equipment.
For example, hoes and scythes remain compatible with vanilla stands. The scarecrow
uses its own item-code whitelist to decide which of those tools it accepts.

### Arm transforms for modders

The [modder's guide](FORMODDERS.md#arm-transforms) documents one shared
`onScarecrowTransform` with automatic right-arm mirroring,
the native `.tfedit` workflow, coordinate units, defaults and complete JSON examples.
Both full-size shapes contain shared arm mounts directly; supported hanging mods
reuse these points with item-specific transforms. The little scarecrow's neck
setup is unchanged.

### Optional pendulum motion

See [Pendulum motion](FORMODDERS.md#pendulum-motion) for all switches, pivots,
attachment-point mappings, defaults and specialized-model exceptions. The
[complete integration example](FORMODDERS.md#complete-integration-example) combines
attachment permission, models, transforms and wind motion in ready-to-adapt patches.

### Adventurer's Walking Stick

With **Adventurer's Walking Stick 3.0.12**, both full-size scarecrow models accept
16 complete walking-stick types (28 variants, including 13 lantern materials) on
either arm. Attach them with **Shift + right-click** instead of another tool.
Separate hidden blades/guns and empty sheaths are excluded; complete sheathed
walking sticks are included. Neck/foot slots and the little scarecrow are excluded.
Blackguard and Forlorn are excluded: these spear-style sticks did not support
Shift + right-click attachment in my game test.

The patches reference the original item models and preserve existing `utilities`
categories and riding-animal attachments. I tested and aligned the supported
sticks through the shared `.tfedit` transform; positions remain editable.
Pendulum motion initially stays off and may be
enabled in the patches. No radius bonus or active player ability is added. See the
[Walking Stick details](#adventurers-walking-stick). All JSON files are under
`compatibility/walkingstick/patches`.

### Primitive Survival

With **Primitive Survival 5.1.4**, attach any of its 13 wooden lanterns or seven
filled Firefly Jar types to either arm using **Shift + right-click**. Both the
standard full-size model and the rounded Plumpkins model are supported. The
little scarecrow also accepts them at its neck. Foot slots and new full-size neck
attachments remain excluded.

Wooden lanterns use their original ceiling model and add **2 blocks** to the
natural-spawn protection radius. Filled Firefly Jars retain their species-specific
textures and light colors, hang from the cork and add **1 block**. Both swing with
the local wind on the full-size arms. Neck lights stay still and use separate
neck scaling/placement without changing the calibrated arm transforms. Use the
normal inventory variants; empty/baited jars and placed
orientation variants are excluded.

Only the strongest equipment bonus applies. The radius outline and tooltip update
when either light is attached or removed; **Ctrl + H** retains its toggle behavior.
All JSON patches are under `compatibility/primitivesurvival/patches`. This
integration adds no creature AI tasks or placed-block behavior changes.
The little scarecrow reaches 14 blocks with a wooden lantern or 13 with a filled
Firefly Jar. For neck placement adjustments, select **Scarecrow – Neck** in
`.tfedit` and save the exported `onScarecrowNeckTransform` to the matching patch.

### More Lanterns by Ape

Optional support for **More Lanterns 1.4.4** includes all eleven paper and metal
models that can be placed on walls or ceilings:

| Family | Supported models | Natural-spawn radius bonus |
| --- | --- | --- |
| Paper | Basic, Chinese, Decorated Chinese, Medieval 1 Small | +2 blocks |
| Metal | Modern 1, Modern 2, Gothic 1, Medieval 2, Medieval 3, Medieval 4, Medieval 6 | +4 blocks |

Attach their normal **upright inventory variants** with **Shift + right-click**.
The full-size scarecrow holds one in each arm instead of a tool, for up to two
lanterns; the little scarecrow retains its neck lantern. Floating lanterns,
stone lanterns and unfinished lantern parts are not accepted.

The original hanging models and available colors, on/off states and brightness
variants are retained, covering 128 inventory variants. Each slot keeps its own
textures, so differently colored lanterns can be equipped together. Chinese paper
and metal lanterns have been enlarged by 50% from the initial attachment scale;
Basic and Medieval 1 Small paper lanterns retain their size. The arm mounts correct
the arms' angles so lanterns hang straight down in the resting pose.

Vanilla and More Lanterns arm attachments now swing with local wind and damped
follow-through, including with Immersive Lanterns installed. The little
scarecrow's neck lantern keeps its existing appearance. On loading an older
full-size scarecrow, its neck lantern moves to the first free arm. If both arms
are occupied, it remains safely stored and removable in the legacy neck slot;
new items cannot be attached there. Packed equipment uses the same migration.

Only the **strongest attached lantern's bonus** applies, up to **+4 blocks**;
bonuses do not stack. Removing a lantern uses the strongest remaining bonus or
restores the base radius. The outline updates automatically, and **Ctrl + H** still
switches it on and off. The bonus applies day and night, including on/off and
brightness variants, and changes only natural-spawn protection. Animal flee and
attack distances remain unchanged.

All compatibility JSON patches are under `compatibility/apelanterns/patches`.
Existing tool categories and world-block geometry are preserved. See the
[More Lanterns settings](#more-lanterns-by-ape) for asset and validation
details.

### Wind Chimes by Ape

With **Wind Chimes 1.5.3 or 1.5.4**, the full-size scarecrow can hold a wind chime in either
arm slot instead of a tool or lantern. Use **Shift + right-click** to attach one;
use an empty hand with the same control to remove it. Bamboo, wood, brass, copper
and crystal models are supported, plus the bone wind chime added in 1.5.4, for
eleven material variants in the newer version.

Wind chimes hang upright and use the original ceiling models' wind movement.
They play their original material sounds spatially from
the scarecrow. Each arm can hold a different chime. Sounds follow the scarecrow
and stop when the chime is removed, the scarecrow dies or despawns, or the listener
leaves hearing range. They use the game's ambient sound channel and Wind Chimes'
held-volume and hearing-distance settings, with a 50% volume increase for attached
wind chimes. Muting remains effective.

Attached wind chimes are silent in calm weather. When wind picks up, each arm plays
its original recording once, followed by a randomized pause. Light wind produces
longer pauses; stronger wind shortens them, including pauses already in progress.
The pause ranges from roughly 8–20 seconds in very light wind to 0.5–3 seconds in
strong wind, in addition to the recording's duration.

Sounds become quieter when the scarecrow or listener is under cover. This uses
Wind Chimes' `WindChimeIndoorVolume` setting (default: 10% of outdoor volume) and
respects `DisableIndoorMuffle`. Scarecrow only reads these settings; it does not
change the Wind Chimes configuration file. Wind-dependent playback does not change
the wind animation or the six-block protection bonus.

An attached wind chime adds **6 blocks** to natural-spawn protection, increasing
the full-size scarecrow's radius from **16 to 22 blocks**. Only the strongest
equipment bonus applies: two wind chimes or a wind chime together with lanterns
still give +6 blocks. Removing the last wind chime restores the strongest remaining
lantern bonus, or the base radius. The radius outline follows automatically;
animal flee and attack distances remain unchanged.

This support works independently of More Lanterns and can be used alongside it.
All JSON patches are under `compatibility/windchimes/patches`; original models,
textures and recordings are referenced directly. Attached wind chimes use
independent wind-driven hanging-piece animation with damped follow-through,
including when **Immersive Lanterns** is installed. Shader wind is suppressed
on private attachment model copies to avoid double movement. Placed wind chimes, sound
and radius bonuses are unchanged.
See the
[Wind Chimes settings](#wind-chimes-by-ape) for details.

## Configuration

### Personal equipment information and handbook

I provide a **Scarecrows — guide** page in the survival handbook. It links to the
three scarecrow item pages for recipes and explains placement, protection,
equipment, pickup and ownership. Supported equipment pages show their permitted
attachment positions, any natural-spawn radius bonus and arm wind motion, with
a link to the guide. The large scarecrow's alternative pumpkin model uses the
same equipment rules.

Extra inventory tooltips are optional and **off by default**. Each player has a
separate `ModConfig/scarecrow-client.json` in their own Vintage Story data directory:

```json
{
  "ConfigVersion": 1,
  "ShowEquipmentTooltips": false
}
```

Set `ShowEquipmentTooltips` to `true` and restart the client to show the additional
equipment section at the end of inventory tooltips, after the complete item/mod
description. It starts with a blank line and a bold
gold heading; details use normal text. Zero bonuses are omitted. The handbook
information remains available with either preference, without repeating the
equipment details twice. This local file is not synchronized by the server;
existing placed-scarecrow tooltips and the server settings below are independent.
All 33 existing languages include the new labels and guide.

### Diagnose natural-spawn protection

With the `controlserver` privilege, use `/scarecrow diagnose <modid:entitycode>`
while looking at a living scarecrow within eight blocks. Supply an exact
registered entity code, including its mod ID, without wildcards. The command
checks whether a natural spawn of that entity would be blocked at your current
position. It reports the matching filter (or `—`), base radius, equipment bonus,
total radius and three-dimensional distance. A negative result identifies a
missing filter or an out-of-range position.

Alternatively, look at a living animal and use `/scarecrow diagnose` without a
code. This reads its saved `origin` and compares its current position with loaded
living scarecrows in the same dimension. An actual blocker takes priority over
other candidates; otherwise the command prefers a matching filter, then the
nearest scarecrow. Missing origin data is reported as unknown, not assumed to be
player placement. Only `entityspawner` is subject to natural-spawn interception;
other origins, including reproduction and player/cage placement, are exempt.

Results appear only for the caller in the **Info Log** channel, in their language,
without a duplicate in the input chat channel. Command errors still use the normal
command feedback channel. Each report starts with a blank line; labels through
their colons are bold white, while values retain normal formatting. The command does not spawn,
remove or change animals and does not reconstruct earlier spawn events. Its
natural-spawn result is hypothetical even for an already existing animal; it
does not diagnose flee/attack AI. It requires a player and cannot run from the
server console.

### Server protection settings

On first startup, the mod creates `ModConfig/scarecrow.json` in the Vintage Story
data directory. For a dedicated server, edit the file in the server's data directory.

```json
{
  "ConfigVersion": 3,
  "DebugOutput": false,
  "ScarecrowRadius": 16,
  "LittleScarecrowRadius": 12,
  "StrawDummyRadius": 8,
  "ScaredAnimalPrefixes": [
    "hare",
    "raccoon"
  ]
}
```

| Setting | Purpose |
| --- | --- |
| `ConfigVersion` | Configuration schema, independent of the mod version. Updated automatically; currently `3`. |
| `DebugOutput` | Writes spawn-interception details to the server log when enabled. |
| `ScarecrowRadius` | Large scarecrow base radius; default `16` blocks. |
| `LittleScarecrowRadius` | Little scarecrow base radius; default `12` blocks. |
| `StrawDummyRadius` | Vanilla straw dummy base radius; default `8` blocks. |
| `ScaredAnimalPrefixes` | Natural-spawn filters. Plain entries retain case-insensitive path-prefix matching across mods; entries containing `:` match the full domain/path, with optional `*` wildcards in the path. |

Both formats may be mixed in the same list:

| Entry | Match | Tooltip example |
| --- | --- | --- |
| `hare` | Any domain whose entity path starts with `hare`, as before | Hares |
| `game:hare-*` | Vanilla hare variants only | Hares (game) |
| `modid:tiergruppe-*` | Matching variants in exactly `modid` | Translated group name (modid) |
| `modid:tier-adult` | Exactly this entity code; no implicit prefix matching | Translated animal/group name (modid) |
| `*:hare-*` | Matching hare variants in any domain | Hares (*) |
| `modid:*` | All natural entity spawns in that mod within protection range | Animals (modid) |

Qualified filters match the whole path; use `*` explicitly to include variants.
The domain must be an exact mod ID or the single wildcard `*`. Invalid qualified
filters are ignored; regex expressions are not accepted. Unqualified entries
remain literal prefixes, so use `*:hare-*` rather than `hare-*` for a wildcard
across mods. Existing filter entries retain their meaning when upgrading the config.

Base radii must be whole numbers from `1` to `64` blocks. Equipment adds its
strongest permitted bonus to that base. These settings change natural-spawn
protection, not the flee or attack distances defined by entity JSON patches.

After saving the file, use `/scarecrow reload` in chat or `scarecrow reload` in
the server console to apply it without restarting. The command requires the
`controlserver` privilege. It reports how many loaded scarecrows were updated;
scarecrows loaded or placed later use the new settings too. An empty prefix list
disables the configured natural-spawn blocking while leaving the animal flee patches active.

Reload updates base radii, natural-spawn filters, debug logging and synchronized
protection tooltip data. It does not remove existing animals, change JSON-patched
AI tasks or reload assets. The enabled radius overlay continues updating
automatically and follows the server's new radius and equipment bonuses. The
personal `scarecrow-client.json` is not reloaded by this command.

If the JSON cannot be read at startup, the mod logs the error and uses its defaults
without overwriting the invalid file. During reload, a missing or invalid file
instead leaves the previous active settings unchanged. Correct the file and run
the command again; details are recorded in the server log.

Older configs without `ConfigVersion` are treated as schema 1 and upgraded to
schema 3. Schema 2 files also receive the new radius defaults. Missing settings
receive their defaults; existing values, empty animal lists and unknown properties
are preserved. Before changing an existing file,
the mod saves its original settings to `ModConfig/scarecrow.json.bak`.

Complete current files are not rewritten on startup. Newer schemas are read
without downgrading or rewriting them. If a backup or save fails, the loaded
settings remain active and the error is logged. Configuration versioning does
not enable or include scarecrow aging in this branch.

### Custom attachment rules

Attachment rules are asset settings, separate from `ModConfig/scarecrow.json`.
See [FORMODDERS.md](FORMODDERS.md) for the complete slot/code whitelist, native
category and model setup, textures, variant patches, arm transforms, pendulum
motion, radius bonuses and testing instructions.

## Optional mod compatibility

The repository includes compatibility assets for the following mods and families:

| Mod or family | Included support |
| --- | --- |
| Art of Growing and its patched version | Crafting adjustments and hayfork attachment shapes |
| Art of Growing Breeding Addon patch | Crafting adjustments |
| Plumpkins | Alternative pumpkin shape for the scarecrow and its item, retaining the standard equipment mounts and animated arm attachments |
| Primitive Survival 5.1.4 | Wooden lanterns and filled Firefly Jars on full-size arms with wind motion or on the little scarecrow's neck; +2/+1 radius bonuses |
| Animal Cages 5.0.1 | Cage releases remain outside natural-spawn blocking; optional server-side Harmony integration |
| Wolf Taming 5.0.1 / Pet AI 5.1.2 | Wild wolves retain the existing vanilla scarecrow reactions and pup taming. Dogs keep native pet/owner behavior; no extra AI patch required. Combined game test passed: tamed pets do not flee from any scarecrow type |
| Fox Taming 1.7.6, Cats 5.0.2, Pet Bunny 1.0.5 / Pet AI 5.1.2 | No extra scarecrow fear/combat tasks needed for these pets. Existing wild fox/hare reactions and taming coexist; kitten player-play tasks remain native. Combined game test passed: tamed pets do not flee from any scarecrow type |
| Adventurer's Walking Stick 3.0.12 | Complete walking sticks on either full-size arm, original models and editable transforms |
| Grass to Ropes | Alternative crafting recipes |
| Leaf Litter | Rake attachment shapes |
| More Lanterns by Ape | Paper and metal lantern attachments with natural-spawn radius bonuses |
| Wind Chimes by Ape | Upright arm attachments with original spatial chime sounds |
| Dreamcatchers 1.0.2 | Tested decorative attachments on either/both full-size arms; no gameplay bonus. Newer 1.0.31 has an upstream loading error |
| censership 1.0.6 | Hanging arm censers with original smoke/light and wind motion; no protection bonus |
| Equus, Equus Ferus and Monoceros | Animal behavior patches |
| Equus: Shared Ancestry and Equus: Expanded | Wild donkey reactions and inherited pony reactions |
| Vara's Arctic Wolves | Native player-target AI adapted for scarecrows, including attacks, injury fear, pup fleeing and sleep interruption |
| Crows 0.2.8 | Native flight/feeding/perch exclusion for all three scarecrow types; guard/avoid radii 16/18 (large), 12/16 (little), 8/12 (straw dummy); updated idle wake path. Replaces the 0.2.7 patch baseline |
| Flocks 0.22.2 | Built-in crow protection at Flocks' own radius; mouse detection ranges 16/12/8 blocks for large/little/straw scarecrows, plus wake patches. Fixed AI ranges exclude equipment bonuses. Other birds retain native behavior |
| Fantasy Creatures Mod 0.8.3 | Native player-target combat/fear for goblins, orks, skeletons, slimes, tree creatures and dragons; hostile creatures can attack scarecrows |
| Pegasus: Winged horses 1.1.0-rc.1 | Wild-only injury defense/fear and rest interruption; feral/tamed mounts and Jaunt flight controls unchanged |
| Talonborne 1.2.0 | Adult/young grazing interruption only; no fleeing or attacks added to raised mounts. Commanded rest and flight unchanged |
| Teapot's Giant Scorpion 1.0.3 | Surface/cave native sting, claws, charge and climbing pursuit; scorpions can attack scarecrows |
| Teapot's Undead Legion 2.2.2 | Hostile undead attack/pursue scarecrows using native melee, ranged and Lich combat; bound companions and player summons unchanged |
| Feverstone's Horses | Wild land-animal reactions |
| Feverstone Wilds 3.2.1 | Native player-target tasks for wild land animals, griffon flight attacks and all young griffon ages; tame variants excluded |
| Sonya's Quality Fowl and More Animals | Flee rules for ducks and other small birds |
| Teapot's Giant Boar, Giant Redriver and Great Bighorn | Fleeing, injury-triggered adult defense and young-animal protection |
| The Critters Pack coral fork | Wild-animal reactions and updated native scarecrow targets |
| Bats | Native feeding deterrents at the three protection radii |
| Truth and Beauty: Detailed Animals | Generated chicks inherit vanilla chicken flee rules |
| FotSA family | Animal behavior patches for Bovinae, Caninae, Capreolinae, Casuariidae, Cervinae, Dinornithidae, Elephantidae, Felinae, Machairodontinae, Manidae, Meiolaniidae, Moschidae, Pantherinae, Rhinocerotidae, Spheniscidae, Thylacinidae, Viverridae and Vombatidae |

The [compatibility overview](#optional-mod-compatibility) covers the
new Felinae, Manidae and Spheniscidae patches, native fear/defense conditions and
the deliberate exclusion of fully aquatic Iniidae and Sirenia.

Compatibility depends on the installed versions and the asset definitions supplied
by those mods. Include the relevant mod versions when reporting a compatibility issue.

See [compatibility overview](#optional-mod-compatibility) for the current
size and age rules, injury-triggered defense and the mod versions used for the audit.

## Feedback and contributions

I welcome bug reports, compatibility fixes and translation improvements.
When reporting a problem, include:

- Your Vintage Story and Scarecrow versions.
- Whether it happens in singleplayer or on a server.
- Relevant additional mods and their versions.
- Steps to reproduce the problem and relevant log messages.

See the [changelog](CHANGELOG.md) for release history and the
[issue tracker](https://github.com/Mandikor/Vintage.Story.Scarecrow/issues) for reports.

## License

Scarecrow is distributed under the [MIT License](LICENSE.txt).

## Source code

The published `dev` branch contains `scarecrow/`, `CHANGELOG.md`, `LICENSE.txt`,
`TODO.md`, `FORMODDERS.md` and this README. Development scripts, IDE settings, extended guides,
audit notes and local tests are kept locally and are not included in the branch.

Code is under [scarecrow/src](scarecrow/src), with optional-mod adapters under
`scarecrow/src/combat` and JSON compatibility patches under
`scarecrow/assets/scarecrow/compatibility`. Shared equipment rules remain separate
from optional-mod integrations. See the [to-do list](TODO.md) for current work.

Feverstone Wilds **3.2.1** is the supported baseline; the older 3.1.1 paths were
replaced and the native creature migration passed its batch game test.
