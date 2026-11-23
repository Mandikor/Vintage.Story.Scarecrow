using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Scarecrow
{
    /// <summary>Drapes a horizontal radius circle over nearby usable surfaces.</summary>
    internal static class ScarecrowTerrainOutline
    {
        internal static void Add(HashSet<BlockPos> ring, IBlockAccessor blocks,
            double posX, double posY, double posZ, int radius, int dimension)
        {
            var probe = new BlockPos(dimension);
            AddSurfaceOutline(ring, posX, posY, posZ, radius, dimension, (x, z) =>
            {
                // Prefer the scarecrow's level, then search both ways. A height map alone
                // would miss built terraces and bridges, or select a distant treetop.
                int baseY = (int)Math.Floor(posY + 0.01);
                for (int offset = 0; offset <= radius + 1; offset++)
                {
                    if (SurfaceAt(x, baseY - offset, z)) return baseY - offset;
                    if (offset > 0 && SurfaceAt(x, baseY + offset, z)) return baseY + offset;
                }
                return null;
            });

            bool SurfaceAt(int x, int y, int z)
            {
                probe.Set(x, y - 1, z);
                if (!blocks.IsValidPos(probe) || blocks.GetChunkAtBlockPos(probe) == null) return false;
                Block ground = blocks.GetBlock(probe, BlockLayersAccess.MostSolid);
                Block fluid = blocks.GetBlock(probe, BlockLayersAccess.Fluid);
                bool solidTop = ground != null && ground.Id != 0 && ground.SideSolid[BlockFacing.UP.Index];
                bool waterTop = fluid != null && fluid.IsLiquid();
                if (!solidTop && !waterTop) return false;
                probe.Set(x, y, z);
                if (!blocks.IsValidPos(probe) || blocks.GetChunkAtBlockPos(probe) == null) return false;
                Block above = blocks.GetBlock(probe);
                // Check the fluid layer separately: reeds can hide water in the default layer.
                if (above == null || above.IsLiquid()
                    || blocks.GetBlock(probe, BlockLayersAccess.Fluid)?.IsLiquid() == true) return false;
                // Plants without collision are fine; walls and solid blocks are not surfaces.
                var boxes = above.GetCollisionBoxes(blocks, probe);
                return boxes == null || boxes.Length == 0;
            }
        }

        // Determine the circle before sampling heights: terrain must not move the
        // perimeter inward or create extra borders around holes inside the circle.
        // This is a radius guide; actual spawn protection still uses 3D distance.
        internal static void AddSurfaceOutline(HashSet<BlockPos> ring,
            double posX, double posY, double posZ, int radius, int dimension,
            System.Func<int, int, int?> surfaceHeight)
        {
            double radiusSquared = (double)radius * radius;
            for (int x = (int)Math.Floor(posX - radius); x <= (int)Math.Floor(posX + radius); x++)
            {
                for (int z = (int)Math.Floor(posZ - radius); z <= (int)Math.Floor(posZ + radius); z++)
                {
                    if (!Inside(x, z)) continue;
                    if (Inside(x - 1, z) && Inside(x + 1, z)
                        && Inside(x, z - 1) && Inside(x, z + 1)) continue;
                    int? y = surfaceHeight(x, z);
                    if (!y.HasValue) continue;
                    ring.Add(new BlockPos(x, y.Value, z, dimension));
                }
            }

            bool Inside(int x, int z)
            {
                double dx = x + 0.5 - posX, dz = z + 0.5 - posZ;
                return dx * dx + dz * dz <= radiusSquared;
            }
        }
    }
}
