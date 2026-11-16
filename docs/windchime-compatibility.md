# Wind Chimes compatibility

Optional support covers **Wind Chimes by Ape 1.5.3 and 1.5.4**. Reference checks
use 1.5.3 assets and source from the
[upstream repository](https://github.com/Monkeybrain73/windchimes) and the supplied
local 1.5.4 package. The newer package includes a bone model absent from that
upstream version.

The full-size scarecrow accepts one wind chime in either arm slot, instead of a
tool or lantern. Attach it with **Shift + right-click** and remove it with an empty
hand using the same control. Its original equipment slots remain unchanged.

| Model | Material variants |
| --- | --- |
| Bamboo | Brown, green |
| Wood | Wood |
| Bone, version 1.5.4 | Bone |
| Brass | Brass |
| Copper | Copper |
| Crystal | Milky quartz, rose quartz, smoky quartz, amethyst, olivine |

All eleven variants in 1.5.4 reuse the original shapes and textures; the ten variants
in 1.5.3 remain supported. Arm mounts compensate
for the arms' built-in rotation so wind chimes hang vertically in the resting
pose. Separate texture prefixes allow different wind chimes or a wind chime and
a More Lanterns lantern to be equipped together. The Wind Chimes integration
does not require More Lanterns. Disabled dreamcatchers and unrelated items are
not added to the attachment whitelist.

## Sound

Sound processing is enabled once during attachment-behavior initialization, only
on clients with Wind Chimes enabled. Without the mod, the audio path skips periodic
inventory scans, configuration reads and sound loading entirely.

The original mod plays sound for placed blocks and the player's held wind chime.
Scarecrow adds client-side spatial playback for attached wind chimes, reusing each
variant's original `heldIdleSound` recording. Copper uses the same recording as
brass, matching the upstream assets. The bone wind chime uses the wood recording,
matching the held-sound definition supplied in 1.5.4. Recordings play once instead
of continuously looping. Horizontal wind speed is sampled at the attached sound's
height; readings at or below 0.05 remain silent. Between 0.05 and 0.75, randomized
pauses shorten smoothly from 8–20 seconds to 0.5–3 seconds. These pauses follow the
recording's end and adapt to the current wind, so an existing pause shortens when
wind rises. Each arm receives its own randomized pause.

Each arm's sound is handled separately. Changing the material replaces its sound;
removing a chime stops it. Sounds follow the scarecrow and are stopped and disposed
on death, despawn, leaving hearing range or changing dimension. They use the game's
ambient sound channel and its normal spatial attenuation.

The integration reads `WindChimeHeldVolume` and `WindChimeMaxDistance` from
`ModConfig/windchimes-client.json` when its sound player is created. Defaults are
0.2 base volume and 16 blocks. Attached playback raises the base volume by 50%
(0.2 becomes 0.3), capped at 1; a zero volume remains silent. Range is limited to
1–128 blocks. Reload
the world after changing these settings. Scarecrow does not write the other mod's
configuration file. It also reads `WindChimeIndoorVolume` (default 0.1) and
`DisableIndoorMuffle`. Unless muffling is disabled, either the scarecrow being
under cover or the listener being under cover applies the indoor multiplier once.
Cover uses the game's rain-height map at the sound height and listener position;
it is an approximation, not a full room or wind-obstruction simulation. Moving
under or out of cover updates an active sound's volume without reloading it.

Calm weather stops active recordings and clears pending pauses. Weather is only
queried for nearby, living scarecrows with an attached chime in the listener's
dimension. Position and cover queries preserve dimensions and negative block
coordinates. The existing quarter-second sound-update interval is retained.

An attached wind chime adds **six blocks** to natural-spawn protection, increasing
the full-size scarecrow's radius from 16 to 22 blocks. Only the strongest equipment
bonus applies: multiple wind chimes or wind chimes with lanterns still give +6.
Removing the last wind chime uses the strongest remaining lantern bonus, or
restores the base radius if no bonus equipment remains. The enabled radius outline
updates automatically. Animal flee and attack distances are unchanged. The little
scarecrow has no arm equipment slots and is unchanged.

## Assets and validation

Three JSON files under `assets/scarecrow/compatibility/windchimes/patches` add the
arm whitelists, attachment metadata, model mounts and the optional renderer.
Original world geometry,
block behaviors, textures, sounds and material variants are preserved. Models and
recordings are referenced from Wind Chimes, without copying them into Scarecrow
or adding a hard assembly dependency.

Wind chimes are drawn separately using the game's block tesselator and block
shader, preserving the original ceiling models' wind flags and moving regions.
The entity shader otherwise changes wind mode 9 while handling frost, producing
different motion. The optional renderer avoids that reinterpretation without
changing game shaders or original Wind Chimes models. Corrected arm attachment
points retain upright alignment, scale and animation poses. Meshes are cached and
released on item changes, shape reloads and renderer disposal; the shadow pass
uses the same block meshes.

Renderer selection is patched on the server, where entity types are registered,
and passed to clients with the entity properties. Models are excluded from the
ordinary entity mesh only when the separate renderer instance is actually active.

Automated checks cover both versions, with ten and eleven variants respectively,
on both arms, excluded attachment points,
upright model transforms, six-block spawn boundaries, removal, non-stacking and
coexistence with More Lanterns
in either patch order. Separate audio checks cover both arms, configuration,
material changes, removal, movement, range, dimensions, mute and lifecycle cleanup.
Environmental checks cover calm and invalid wind, one-shot completion, pause bounds,
changing wind during a pause, source/listener cover, indoor settings, live volume
updates, dimension-aware queries, negative coordinates and cleanup of pending
pauses. In-game testing confirmed the earlier rendering, sound and six-block radius
bonus, as well as the new environmental playback and adjusted volume.
