# Scarecrow

With the scarecrow you get an effective solution to a plague of hares and raccoons in your garden.

Just combine a straw hat, a pumpkin and a straw dummy and the scarecrow is finished.

Added the little scarecrow, for all those who can't find pumpkins.
Hares and raccoons are less afraid of her, they dare to come within 8 blocks (default).

Attachment rules are defined per slot in the entity JSON files under
`behaviorConfigs/scarecrowattachments/wearableSlots`. Each slot's `allowedCodes`
list supports domain-qualified wildcard patterns such as `game:hoe-*`.
The original collectible categories are preserved, so vanilla stands keep accepting
their tools. Existing attached items remain removable after changing a whitelist.

Run the attachment regression checks with `dotnet run --project tests/Scarecrow.Tests.csproj`.
The `VINTAGE_STORY` environment variable must point to the local game installation.
