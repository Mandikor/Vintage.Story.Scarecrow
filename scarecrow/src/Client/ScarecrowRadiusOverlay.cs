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
    /// sphere is noisy and expensive, so this drapes a horizontal radius circle over nearby ground
    /// and water surfaces. It is an orientation guide, not the exact sphere/terrain intersection.
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
        private const int RefreshIntervalMs = 500;
        internal const int RingColorRgb = 0xFFC850;
        private static readonly int RingColor = ColorUtil.ToRgba(96,
            RingColorRgb >> 16, (RingColorRgb >> 8) & 255, RingColorRgb & 255);

        private ICoreClientAPI capi;
        private bool visible;
        private long refreshListenerId;
        private HashSet<BlockPos> highlightedBlocks = new();

        // Client-only lifecycle; the hotkey controls both highlighting and periodic updates.
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
            if (visible) Hide();
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

            visible = true;
            Refresh(0);
            refreshListenerId = capi.Event.RegisterGameTickListener(Refresh, RefreshIntervalMs);
            if (highlightedBlocks.Count == 0)
            {
                capi.ShowChatMessage(Lang.Get("scarecrow:radius-none"));
            }
        }

        // Refresh only while visible and replace highlights only when the block set changes.
        private void Refresh(float deltaTime)
        {
            if (!visible || capi?.World?.Player?.Entity == null) return;

            HashSet<BlockPos> ring = CollectRingBlocks();
            if (highlightedBlocks.SetEquals(ring)) return;

            capi.World.HighlightBlocks(capi.World.Player, HighlightSlotId,
                new List<BlockPos>(ring),
                new List<int> { RingColor },
                EnumHighlightBlocksMode.Absolute, EnumHighlightShape.Arbitrary);

            highlightedBlocks = ring;
        }

        private void Hide()
        {
            visible = false;
            if (refreshListenerId != 0)
            {
                capi.Event.UnregisterGameTickListener(refreshListenerId);
                refreshListenerId = 0;
            }
            if (capi?.World?.Player != null)
            {
                capi.World.HighlightBlocks(capi.World.Player, HighlightSlotId,
                    new List<BlockPos>(), new List<int>(),
                    EnumHighlightBlocksMode.Absolute, EnumHighlightShape.Arbitrary);
            }
            highlightedBlocks.Clear();
        }

        /// <summary>
        /// Perimeter blocks of the protected ground area of every scarecrow around the player.
        /// </summary>
        // The display uses each nearby entity's effective radius, including equipment bonuses.
        private HashSet<BlockPos> CollectRingBlocks()
        {
            HashSet<BlockPos> ring = new();

            Entity[] scarecrows = capi.World.GetEntitiesAround(capi.World.Player.Entity.Pos.XYZ,
                ScanRange, ScanRange, entity => entity is EntityScarecrow && entity.Alive);

            foreach (EntityScarecrow entity in scarecrows)
            {
                if (entity.Pos.Dimension != capi.World.Player.Entity.Pos.Dimension) continue;
                ScarecrowTerrainOutline.Add(ring, capi.World.BlockAccessor,
                    entity.Pos.X, entity.Pos.Y, entity.Pos.Z, entity.ScareRadius, entity.Pos.Dimension);
            }

            return ring;
        }

    }
}
