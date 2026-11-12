using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

#nullable disable

namespace Scarecrow
{
    /// <summary>
    /// Client side overlay that shows the area a scarecrow keeps animals away from.
    /// <para>
    /// The spawn check uses the 3d distance, so the protected area is a sphere. Drawing the whole
    /// sphere is noisy and expensive, and animals spawn on the ground anyway, so this outlines the
    /// ring of blocks at the scarecrow's feet instead.
    /// </para>
    /// <para>
    /// The blocks are handed to the vanilla block highlight system, which drops every face that has
    /// a neighbour in the list. The result is a closed outline along the block grid instead of a
    /// smooth circle. Everything runs on the client: the radius comes from the entity attributes the
    /// client already receives, so the overlay needs no server round trip.
    /// </para>
    /// </summary>
    public class ScarecrowRadiusOverlay : ModSystem
    {
        private const string HotkeyCode = "scarecrow:showradius";
        private const int HighlightSlotId = 4173;
        private const int ScanRange = 64;
        private const int DefaultScareRadius = 8;

        private ICoreClientAPI capi;
        private bool visible;

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Client;
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            capi = api;

            api.Input.RegisterHotKey(HotkeyCode, Lang.Get("scarecrow:hotkey-showradius"), GlKeys.H,
                HotkeyType.HelpAndOverlays, ctrlPressed: true);
            api.Input.SetHotKeyHandler(HotkeyCode, OnHotkey);
        }

        public override void Dispose()
        {
            if (visible && capi?.World?.Player != null) Hide();
            base.Dispose();
        }

        private bool OnHotkey(KeyCombination combination)
        {
            Toggle();
            return true;
        }

        private void Toggle()
        {
            if (visible)
            {
                Hide();
                return;
            }

            HashSet<BlockPos> ring = CollectRingBlocks();
            if (ring.Count == 0)
            {
                capi.ShowChatMessage(Lang.Get("scarecrow:radius-none"));
                return;
            }

            capi.World.HighlightBlocks(capi.World.Player, HighlightSlotId,
                new List<BlockPos>(ring),
                new List<int> { ColorUtil.ToRgba(96, 255, 200, 80) },
                EnumHighlightBlocksMode.Absolute, EnumHighlightShape.Arbitrary);

            visible = true;
        }

        private void Hide()
        {
            capi.World.HighlightBlocks(capi.World.Player, HighlightSlotId,
                new List<BlockPos>(), new List<int>(),
                EnumHighlightBlocksMode.Absolute, EnumHighlightShape.Arbitrary);

            visible = false;
        }

        /// <summary>
        /// Perimeter blocks of the protected ground area of every scarecrow around the player.
        /// </summary>
        private HashSet<BlockPos> CollectRingBlocks()
        {
            HashSet<BlockPos> ring = new();

            Entity[] scarecrows = capi.World.GetEntitiesAround(capi.World.Player.Entity.Pos.XYZ,
                ScanRange, ScanRange, entity => entity is EntityScarecrow && entity.Alive);

            foreach (Entity entity in scarecrows)
            {
                int radius = entity.Properties.Attributes?["scareRadius"]?.AsInt(DefaultScareRadius)
                    ?? DefaultScareRadius;

                AddRing(ring, entity.Pos.X, entity.Pos.Y, entity.Pos.Z, radius);
            }

            return ring;
        }

        /// <summary>
        /// Adds the perimeter blocks of the horizontal footprint around the given position. A block
        /// belongs to the footprint when its centre is within the radius, which is how the spawn
        /// check measures distance in the horizontal plane.
        /// </summary>
        private static void AddRing(HashSet<BlockPos> ring, double posX, double posY, double posZ, int radius)
        {
            // The entity stands on top of the ground block, so its y is usually exactly on a block
            // boundary. The epsilon keeps the ring one block above the ground instead of inside it
            // when floating point rounding lands just below the boundary.
            int y = (int)Math.Floor(posY + 0.01);

            double radiusSquared = radius * radius;
            int minX = (int)Math.Floor(posX - radius);
            int maxX = (int)Math.Floor(posX + radius);
            int minZ = (int)Math.Floor(posZ - radius);
            int maxZ = (int)Math.Floor(posZ + radius);

            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    if (!IsInside(x, z, posX, posZ, radiusSquared)) continue;

                    // Only the perimeter: a block with all four neighbours inside is not on the ring.
                    if (IsInside(x + 1, z, posX, posZ, radiusSquared)
                        && IsInside(x - 1, z, posX, posZ, radiusSquared)
                        && IsInside(x, z + 1, posX, posZ, radiusSquared)
                        && IsInside(x, z - 1, posX, posZ, radiusSquared)) continue;

                    ring.Add(new BlockPos(x, y, z));
                }
            }
        }

        /// <summary>
        /// True when the centre of the block at the given coordinates lies within the radius around
        /// the position, measured in the horizontal plane.
        /// </summary>
        private static bool IsInside(int x, int z, double posX, double posZ, double radiusSquared)
        {
            double dx = x + 0.5 - posX;
            double dz = z + 0.5 - posZ;

            return dx * dx + dz * dz <= radiusSquared;
        }
    }
}
