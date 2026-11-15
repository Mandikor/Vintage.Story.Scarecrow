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
- Flee behavior for supported vanilla animals and animals from compatible mods.
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

### How animal protection works

Scarecrow uses two complementary mechanisms:

**Flee behavior:** animal patches make supported animals move away from scarecrows.
Vanilla patches cover hares, raccoons, chickens, foxes, bears, hyenas, wolves, deer,
gazelles, goats, moose, pigs and sheep. Behavior depends on the animal's AI tasks,
priorities and emotional state; a scarecrow is not a physical barrier or a guarantee
of safety from every animal.

**Natural-spawn protection:** the mod blocks natural spawns of configured animal
types within the radius. The default list contains hares and raccoons. Animals born
nearby or placed by players, such as animals released from cages, are not removed by
this protection. They can still react to the flee behavior.

Changing the spawn-protection list does not add flee behavior to an animal. Those
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

A placed scarecrow records its owner. Pickup is available to that owner, or when
the entity has no recorded owner, provided claim permissions allow the interaction.
Attachments drop as items when a scarecrow is picked up or dies; they are not packed
inside the returned scarecrow item.

### Radius outline

The overlay shows ground outlines for loaded scarecrows and straw dummies within
64 blocks horizontally and vertically of the player. Each outline follows the
block grid at the entity's feet. It is a horizontal guide to the protection radius,
not a visualization of the entire sphere or every animal's AI behavior.

If you move or remove a scarecrow while the outline is visible, toggle the overlay
off and on to refresh it.

## Equipment

| Attachment point | Accepted items |
| --- | --- |
| Left and right arms, full-size scarecrow only | Scythes, hoes, bug nets and snow shovels |
| Left and right arms with optional mods | Art of Growing hayforks and the Leaf Litter rake |
| Neck, both scarecrow variants | Small and large upright vanilla lanterns |
| Foot, both scarecrow variants | Fired vanilla watering cans |

Each point holds one item. The interaction hints show the items allowed at the
selected point.

The mod preserves the original attachment categories of tools and other equipment.
For example, hoes and scythes remain compatible with vanilla stands. The scarecrow
uses its own item-code whitelist to decide which of those tools it accepts.

## Configuration

On first startup, the mod creates `ModConfig/scarecrow.json` in the Vintage Story
data directory. For a dedicated server, edit the file in the server's data directory.

```json
{
  "DebugOutput": false,
  "ScaredAnimalPrefixes": [
    "hare",
    "raccoon"
  ]
}
```

| Setting | Purpose |
| --- | --- |
| `DebugOutput` | Writes spawn-interception details to the server log when enabled. |
| `ScaredAnimalPrefixes` | Animal-code prefixes blocked by natural-spawn protection. Matching is case-insensitive and uses the code path, without the mod domain. For example, `hare` matches `hare-adult` and `hare-baby`. |

Restart the game or server after editing the file. An empty prefix list disables
the configured natural-spawn blocking while leaving the animal flee patches active.

If the JSON cannot be read, the mod logs the error and uses its defaults without
overwriting the invalid file. Correct the file and restart.

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
| Equus, Equus Ferus and Monoceros | Animal behavior patches |
| FotSA family | Animal behavior patches for Bovinae, Caninae, Capreolinae, Casuariidae, Cervinae, Dinornithidae, Elephantidae, Machairodontinae, Meiolaniidae, Moschidae, Pantherinae, Rhinocerotidae, Thylacinidae, Viverridae and Vombatidae |

Compatibility depends on the installed versions and the asset definitions supplied
by those mods. Include the relevant mod versions when reporting a compatibility issue.

## Building from source

Install the **.NET 10 SDK** and a matching local **Vintage Story 1.22.0** installation.
The project references the game's own assemblies; they are not included in this
repository.

Set `VINTAGE_STORY` to your game installation directory. For example, in PowerShell:

```powershell
$env:VINTAGE_STORY = "C:\Path\To\Vintagestory"
dotnet build scarecrow/scarecrow.csproj -c Release
```

The build output is written to `scarecrow/bin/Release/Mods/mod/`, including the mod
assembly, assets and metadata. The project does not copy the referenced game
assemblies into the mod output.

The repository also includes a Cake packaging script, `build.ps1`, which validates
the asset JSON, publishes the mod and creates a ZIP in `Releases`. Its packaging
step clears the existing contents of that directory before creating the output.

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
