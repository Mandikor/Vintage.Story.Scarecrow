# Scarecrow

Keep unwelcome animals away from your garden in **Vintage Story**. Place a scarecrow
near your crops, berry bushes, beehives or chicken coop, then equip it with gardening
tools, a lantern and a watering can.

The mod adds a full-size scarecrow and a smaller version that does not require a
pumpkin. It also gives the vanilla straw dummy a role in protecting your garden.

[Download on the Vintage Story Mod Database](https://mods.vintagestory.at/scarecrow)
· [Changelog](CHANGELOG.md)
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
| Scarecrow | 16 blocks | Two tools, one lantern and one watering can |
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
The full-size scarecrow can also hold these lanterns in either arm instead of a
tool. With several lanterns attached, only the strongest radius bonus applies.

### How animal protection works

Scarecrow uses two complementary mechanisms:

**Animal reactions:** supported creature mods use their native player-target AI
as the basis for scarecrow reactions. This includes player tags, variant rules,
injury/hunger states and sleep interruption; see the
[native creature guide](docs/native-creature-compatibility.md), including the
runtime-generator and Bats exceptions.

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

A placed scarecrow records its owner. Only that owner can pick it up, attach
equipment or remove attachments, and claim permissions must also allow access.
These checks run on the server, including attachment packets. Old scarecrows with
no recorded owner remain accessible to players with claim permission; they are not
automatically assigned to the first player who interacts with them. Damage and
destruction still follow the game's existing rules.

Picking up a scarecrow packs its equipment into the returned item. Placing it again
restores each attachment to its previous slot, including its durability and other
item attributes. Empty scarecrows still stack up to four; equipped scarecrows stay
in separate inventory slots, even when their equipment is identical.

The inventory tooltip of a packed scarecrow lists its stored equipment under
**Inventory:**, ordered by left arm, right arm, lantern slot and foot slot. Each
line identifies the slot and uses the equipment's translated game/mod name. Empty
slots are marked as empty. The little scarecrow lists only its lantern and foot
slots; empty scarecrows have no extra inventory section. All labels are localized
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
block grid at the entity's feet. It is a horizontal guide to the protection radius,
not a visualization of the entire sphere or every animal's AI behavior.

While enabled, the overlay checks nearby scarecrows every half second and updates
when they are placed, moved, removed or loaded as you travel. It remains enabled
when no scarecrow is nearby, so newly encountered scarecrows appear automatically.
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
normal formatting. No chat messages are generated by this information display.

The tooltip updates with equipment changes and uses the server's configured spawn
list in multiplayer. An empty list is shown as disabled. Hares and raccoons have
translated group names; full entity codes use available game/mod translations.
Other custom prefixes are identified explicitly as code prefixes. All 33 existing
languages include the new labels. These figures describe natural-spawn protection;
animal flee and attack behavior remains species-specific.

## Equipment

| Attachment point | Accepted items |
| --- | --- |
| Left and right arms, full-size scarecrow only | Scythes, hoes, bug nets and snow shovels |
| Left and right arms with optional mods | Art of Growing hayforks and the Leaf Litter rake |
| Neck, both scarecrow variants | Small and large upright vanilla lanterns |
| Neck with More Lanterns | Upright paper and metal lanterns; floating and stone lanterns are excluded |
| Arms with More Lanterns, full-size scarecrow only | The same paper and metal lanterns, instead of tools |
| Arms with Wind Chimes, full-size scarecrow only | Bamboo, wood, bone, brass, copper and crystal wind chimes, instead of tools |
| Foot, both scarecrow variants | Fired vanilla watering cans |

Each point holds one item. The interaction hints show the items allowed at the
selected point.

The mod preserves the original attachment categories of tools and other equipment.
For example, hoes and scythes remain compatible with vanilla stands. The scarecrow
uses its own item-code whitelist to decide which of those tools it accepts.

### More Lanterns by Ape

Optional support for **More Lanterns 1.4.4** includes all eleven paper and metal
models that can be placed on walls or ceilings:

| Family | Supported models | Natural-spawn radius bonus |
| --- | --- | --- |
| Paper | Basic, Chinese, Decorated Chinese, Medieval 1 Small | +2 blocks |
| Metal | Modern 1, Modern 2, Gothic 1, Medieval 2, Medieval 3, Medieval 4, Medieval 6 | +4 blocks |

Attach their normal **upright inventory variants** with **Shift + right-click**.
Both scarecrow sizes accept a lantern at the neck. The full-size scarecrow can also
hold one in each arm instead of a tool, for up to three lanterns. Floating lanterns,
stone lanterns and unfinished lantern parts are not accepted.

The original hanging models and available colors, on/off states and brightness
variants are retained, covering 128 inventory variants. Each slot keeps its own
textures, so differently colored lanterns can be equipped together. Chinese paper
and metal lanterns have been enlarged by 50% from the initial attachment scale;
Basic and Medieval 1 Small paper lanterns retain their size. The arm mounts correct
the arms' angles so lanterns hang straight down in the resting pose.

Only the **strongest attached lantern's bonus** applies, up to **+4 blocks**;
bonuses do not stack. Removing a lantern uses the strongest remaining bonus or
restores the base radius. The outline updates automatically, and **Ctrl + H** still
switches it on and off. The bonus applies day and night, including on/off and
brightness variants, and changes only natural-spawn protection. Animal flee and
attack distances remain unchanged.

All compatibility JSON patches are under `compatibility/apelanterns/patches`.
Existing tool categories and world-block geometry are preserved. See the
[lantern compatibility guide](docs/lantern-compatibility.md) for asset and validation
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
textures and recordings are referenced directly. See the
[wind chime compatibility guide](docs/windchime-compatibility.md) for details.

## Configuration

On first startup, the mod creates `ModConfig/scarecrow.json` in the Vintage Story
data directory. For a dedicated server, edit the file in the server's data directory.

```json
{
  "ConfigVersion": 2,
  "DebugOutput": false,
  "ScaredAnimalPrefixes": [
    "hare",
    "raccoon"
  ]
}
```

| Setting | Purpose |
| --- | --- |
| `ConfigVersion` | Configuration schema, independent of the mod version. Updated automatically; currently `2`. |
| `DebugOutput` | Writes spawn-interception details to the server log when enabled. |
| `ScaredAnimalPrefixes` | Animal-code prefixes blocked by natural-spawn protection. Matching is case-insensitive and uses the code path, without the mod domain. For example, `hare` matches `hare-adult` and `hare-baby`. |

Restart the game or server after editing the file. An empty prefix list disables
the configured natural-spawn blocking while leaving the animal flee patches active.

If the JSON cannot be read, the mod logs the error and uses its defaults without
overwriting the invalid file. Correct the file and restart.

Older configs without `ConfigVersion` are treated as schema 1 and upgraded to
schema 2. Missing settings receive their defaults; existing values, empty animal
lists and unknown properties are preserved. Before changing an existing file,
the mod saves its original settings to `ModConfig/scarecrow.json.bak`.

Complete current files are not rewritten on startup. Newer schemas are read
without downgrading or rewriting them. If a backup or save fails, the loaded
settings remain active and the error is logged. Configuration versioning does
not enable or include scarecrow aging in this branch.

### Custom attachment rules

Attachment whitelists are asset settings, separate from `ModConfig/scarecrow.json`.
They live in the entity definitions under
`behaviorConfigs/scarecrowattachments/wearableSlots`.

Each slot has an `allowedCodes` list, for example:

```json
"allowedCodes": [
  "game:hoe-*",
  "game:scythe-*"
]
```

Patterns include the mod domain. `game:hoe-*` accepts vanilla hoe variants but not
an unrelated mod's item with a similar name. These rules apply independently to
each slot, and existing attachments remain removable after a rule changes.

Adding an item code does not create a custom attachment model. Additional items may
also need attachment metadata and a shape for the relevant slot. For maintained
customizations, use a separate compatibility patch rather than editing a downloaded
release ZIP.

## Optional mod compatibility

The repository includes compatibility assets for the following mods and families:

| Mod or family | Included support |
| --- | --- |
| Art of Growing and its patched version | Crafting adjustments and hayfork attachment shapes |
| Art of Growing Breeding Addon patch | Crafting adjustments |
| Plumpkins | Alternative pumpkin shape for the scarecrow and its item |
| Grass to Ropes | Alternative crafting recipes |
| Leaf Litter | Rake attachment shapes |
| More Lanterns by Ape | Paper and metal lantern attachments with natural-spawn radius bonuses |
| Wind Chimes by Ape | Upright arm attachments with original spatial chime sounds |
| Equus, Equus Ferus and Monoceros | Animal behavior patches |
| Equus: Shared Ancestry and Equus: Expanded | Wild donkey reactions and inherited pony reactions |
| Vara's Arctic Wolves | Native player-target AI adapted for scarecrows, including attacks, injury fear, pup fleeing and sleep interruption |
| Crows 0.2.7 | Native flight/feeding/perch exclusion for all three scarecrow types; little scarecrow radius aligned to 12 blocks |
| Flocks 0.22.1 | Built-in crow protection at Flocks' own radius; mouse flee/wake patches. Other birds retain native behavior; see [scope and settings](docs/flocks-compatibility.md) |
| Fantasy Creatures Mod 0.8.3 | Native player-target combat/fear for goblins, orks, skeletons, slimes, tree creatures and dragons; hostile creatures can attack scarecrows. See [behavior guide](docs/fantasycreatures-compatibility.md) |
| Pegasus: Winged horses 1.1.0-rc.1 | Wild-only injury defense/fear and rest interruption; feral/tamed mounts and Jaunt flight controls unchanged. See [scope](docs/pegasus-compatibility.md) |
| Feverstone's Horses | Wild land-animal reactions |
| Feverstone Wilds 3.2.1 | Native player-target tasks for wild land animals, griffon flight attacks and all young griffon ages; tame variants excluded |
| Sonya's Quality Fowl and More Animals | Flee rules for ducks and other small birds |
| Teapot's Giant Boar, Giant Redriver and Great Bighorn | Fleeing, injury-triggered adult defense and young-animal protection |
| The Critters Pack coral fork | Wild-animal reactions and updated native scarecrow targets |
| Bats | Native feeding deterrents at the three protection radii |
| Truth and Beauty: Detailed Animals | Generated chicks inherit vanilla chicken flee rules |
| FotSA family | Animal behavior patches for Bovinae, Caninae, Capreolinae, Casuariidae, Cervinae, Dinornithidae, Elephantidae, Felinae, Machairodontinae, Manidae, Meiolaniidae, Moschidae, Pantherinae, Rhinocerotidae, Spheniscidae, Thylacinidae, Viverridae and Vombatidae |

The [additional FotSA guide](docs/fotsa-additional-compatibility.md) covers the
new Felinae, Manidae and Spheniscidae patches, native fear/defense conditions and
the deliberate exclusion of fully aquatic Iniidae and Sirenia.

Compatibility depends on the installed versions and the asset definitions supplied
by those mods. Include the relevant mod versions when reporting a compatibility issue.

See [animal compatibility details](docs/animal-compatibility.md) for the current
size and age rules, injury-triggered defense and the mod versions used for the audit.

## Feedback and contributions

Bug reports, compatibility fixes and translation improvements are welcome.
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

The [local mod compatibility audit](docs/mod-library-audit-2026-11-19.md) lists
checked versions, known issues and proposed integrations. Its six Feverstone Wilds
path failures are now corrected for **3.2.1**, with native player AI replacing
the older generic rules. **3.1.1 is no longer the supported baseline.** See the
[Feverstone guide](docs/feverstonewilds-compatibility.md),
[native creature migration](docs/native-creature-compatibility.md) and
[to-do list](todo.md) for the implementation and successful batch game test.

See the [source structure and extension guide](docs/source-structure.md) for code
responsibilities, optional-mod adapters and inventory/synchronization rules.
