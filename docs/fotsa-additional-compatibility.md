# Additional FotSA integrations

This `dev` batch adds the remaining supplied FotSA mods relevant to a land/garden
scarecrow. It uses the same native player-task approach as the existing creature
migration, not a new size/diet rule. The author completed the in-game test successfully.

| Manifest ID | Supplied version | Scope | Configured types | Age/sex variants |
| --- | --- | --- | ---: | ---: |
| `felinae` | 0.2.16 | Caracal and three lynx types, adults and young | 4 | 16 |
| `manidae` | 1.0.24 | Pangolins, adults and young | 9 | 36 |
| `spheniscidae` | 1.0.20 | Penguins, adults and mobile chicks; land and swimming AI | 20 | 80 |

## Native reactions

Six JSON files under `compatibility/<modId>/patches` add 19 task copies targeting
only `scarecrow`, `little-scarecrow` and `strawdummy`. The original player-tag
selectors are replaced in the copies; original tasks remain untouched. Native
priorities, ranges, speeds, animations, variant maps, emotion conditions and
swimming gates are preserved. No global `player` tag is applied to scarecrows.

The templates use player tags rather than direct player codes. In these files
the player-facing reactions are `fleeentity` tasks gated by the native
`fleeondamage` state. The patches retain this condition; they do not force fear,
attacks or flight merely because a scarecrow is present. Existing emotion-state
activation and original player/prey behavior remain the responsibility of FotSA.

Adult Manidae has an authored stationary defense task: animation `Sleep`, speed
zero. The added task preserves it for all 18 adult sex/type configurations rather
than converting it into walking/running. Native slower walking and swimming
responses remain available under their original conditions.

Penguin swimming tasks remain swimming-only and ground tasks remain ground-only.
The native ranges can be much larger than Scarecrow's natural-spawn radius;
they are not replaced with fixed 16/12/8-block detection ranges.

Added tasks disable `retaliateAttacks` and any damage-triggered instant-flight
setting, preventing an arbitrary attacker from bypassing their scarecrow target
lists. Original player-facing damage responses, breeding, sleeping, egg and
other behavior settings are unchanged. No C# adapter, language strings or new
natural-spawn blocking prefixes are required.

## Deliberately excluded

- Spheniscidae eggs do not receive movement tasks. The supplied mobile chicks do.
- No new tame/owned definitions or owner-command behavior are added.
- `iniidae` 0.1.8 and `sirenia` 1.0.33 are fully aquatic in the supplied assets.
  They are reviewed but deferred for this land/garden use case. Neither receives
  patches or artificial land reactions. Iniidae's previously noted malformed
  item JSON is an upstream input issue and remains untouched.

## Validation and game test

Checks read the current unpacked manifests and original entity files, apply every
shipped operation and use the actual game variant resolver. They cover six
definitions, 19 task copies, 132 variants and 414 enabled task configurations,
including 132 swimming and 18 stationary-defense configurations. They verify
unchanged original tasks and per-variant action/state settings, target isolation,
attacker safety and the exclusion of eggs and the two aquatic mods. Source files
are read-only. Build and automated checks pass; the author also completed the
in-game test successfully.

Suggested in-game samples: adult/young caracal or lynx, adult/young pangolin,
and adult/chick penguin on land and in water. Compare behavior against actual
players and all three scarecrow types, including the native injury/fear states.
Do not expect unconditional fleeing when the original FotSA state is inactive.

The local tests do not simulate navigation or every installed mod combination.
