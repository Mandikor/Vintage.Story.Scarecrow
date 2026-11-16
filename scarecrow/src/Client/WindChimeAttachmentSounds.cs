using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Scarecrow
{
    /// <summary>Spatial sound for attached Wind Chimes, whose block ambient sound cannot run on an entity.</summary>
    internal sealed class WindChimeAttachmentSounds : IDisposable
    {
        private readonly ICoreClientAPI api;
        private readonly Entity entity;
        private readonly float volume;
        private readonly float range;
        private readonly Dictionary<int, (AssetLocation location, ILoadedSound sound)> playing = new();
        private bool disposed;

        public WindChimeAttachmentSounds(ICoreClientAPI api, Entity entity)
        {
            this.api = api;
            this.entity = entity;
            JObject settings = null;
            try
            {
                // Read only: Wind Chimes owns this file. Match its held sound volume and hearing range.
                settings = api.LoadModConfig<JObject>("windchimes-client.json");
            }
            catch (Exception exception)
            {
                api.Logger.Warning("[Scarecrow] Could not read Wind Chimes client sound settings; using defaults: {0}", exception.Message);
            }
            volume = Math.Min(1, ReadSetting(settings, "WindChimeHeldVolume", 0.2f, 0, 1) * 1.1f);
            range = ReadSetting(settings, "WindChimeMaxDistance", 16, 1, 128);
        }

        private static float ReadSetting(JObject settings, string key, float fallback, float min, float max)
        {
            return float.TryParse(settings?[key]?.ToString(Newtonsoft.Json.Formatting.None), System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out float value) && float.IsFinite(value)
                ? Math.Clamp(value, min, max) : fallback;
        }

        public void Update(InventoryBase inventory)
        {
            if (disposed) return;
            EntityPlayer player = api.World.Player?.Entity;
            if (inventory == null || !entity.Alive || player == null || volume <= 0
                || player.Pos.Dimension != entity.Pos.Dimension || player.Pos.SquareDistanceTo(entity.Pos) > range * range)
            {
                StopAll();
                return;
            }

            HashSet<int> active = new();
            for (int index = 0; index < inventory.Count; index++)
            {
                ItemStack stack = inventory[index].Itemstack;
                if (stack?.ItemAttributes?["scarecrowWindChime"].AsBool(false) != true) continue;
                string path = stack.ItemAttributes["heldIdleSound"].AsString();
                if (string.IsNullOrWhiteSpace(path)) continue;
                AssetLocation location = AssetLocation.Create(path, stack.Collectible.Code.Domain);
                active.Add(index);
                if (playing.TryGetValue(index, out var current)
                    && (!current.location.Equals(location) || current.sound.IsDisposed || current.sound.HasStopped))
                {
                    Stop(index);
                }
                Vec3f position = new((float)entity.Pos.X, (float)entity.Pos.Y + 1.2f, (float)entity.Pos.Z);
                if (!playing.TryGetValue(index, out current))
                {
                    ILoadedSound sound = api.World.LoadSound(new SoundParams
                    {
                        Location = location,
                        Position = position,
                        RelativePosition = false,
                        ShouldLoop = true,
                        DisposeOnFinish = false,
                        SoundType = EnumSoundType.Ambient,
                        Range = range,
                        Volume = volume
                    });
                    if (sound == null) continue;
                    playing[index] = current = (location, sound);
                    sound.Start();
                }
                current.sound.SetPosition(position);
            }
            foreach (int index in playing.Keys.Where(index => !active.Contains(index)).ToArray()) Stop(index);
        }

        private void Stop(int index)
        {
            if (!playing.Remove(index, out var current)) return;
            if (current.sound.IsDisposed) return;
            current.sound.Stop();
            current.sound.Dispose();
        }

        private void StopAll()
        {
            foreach (int index in playing.Keys.ToArray()) Stop(index);
        }

        public void Dispose()
        {
            StopAll();
            disposed = true;
        }
    }
}
