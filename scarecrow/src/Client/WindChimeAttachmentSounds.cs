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
        private readonly float indoorVolume;
        private readonly bool disableIndoorMuffle;
        private readonly Dictionary<int, (AssetLocation location, ILoadedSound sound)> playing = new();
        private readonly Dictionary<int, (AssetLocation location, long finishedAt, double jitter)> waiting = new();
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
            volume = Math.Min(1, ReadSetting(settings, "WindChimeHeldVolume", 0.2f, 0, 1) * 1.5f);
            range = ReadSetting(settings, "WindChimeMaxDistance", 16, 1, 128);
            indoorVolume = ReadSetting(settings, "WindChimeIndoorVolume", 0.1f, 0, 1);
            disableIndoorMuffle = bool.TryParse(settings?["DisableIndoorMuffle"]?.ToString(), out bool disabled) && disabled;
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
                || player.Pos.Dimension != entity.Pos.Dimension || player.Pos.SquareDistanceTo(entity.Pos) > range * range
                || !inventory.Any(slot => slot.Itemstack?.ItemAttributes?["scarecrowWindChime"].AsBool(false) == true))
            {
                StopAll();
                return;
            }

            Vec3d soundPosition = entity.Pos.XYZ.Add(0, 1.2, 0);
            Vec3d wind = api.World.BlockAccessor.GetWindSpeedAt(soundPosition);
            double speed = Math.Sqrt(wind.X * wind.X + wind.Z * wind.Z);
            // Ignore near-zero wind; horizontal speed 0.75 reaches the shortest pauses.
            if (!double.IsFinite(speed) || speed <= 0.05)
            {
                StopAll();
                return;
            }
            float windStrength = (float)Math.Clamp((speed - 0.05) / 0.7, 0, 1);
            float soundVolume = volume;
            if (!disableIndoorMuffle && (IsUnderCover(entity.Pos, 1.2) || IsUnderCover(player.Pos, 0)))
                soundVolume *= indoorVolume;

            if (soundVolume <= 0)
            {
                StopAll();
                return;
            }

            long now = api.World.ElapsedMilliseconds;
            HashSet<int> active = new();
            for (int index = 0; index < inventory.Count; index++)
            {
                ItemStack stack = inventory[index].Itemstack;
                if (stack?.ItemAttributes?["scarecrowWindChime"].AsBool(false) != true) continue;
                string path = stack.ItemAttributes["heldIdleSound"].AsString();
                if (string.IsNullOrWhiteSpace(path)) continue;
                AssetLocation location = AssetLocation.Create(path, stack.Collectible.Code.Domain);
                active.Add(index);
                if (playing.TryGetValue(index, out var current) && !current.location.Equals(location))
                {
                    Stop(index);
                }
                else if (playing.TryGetValue(index, out current) && (current.sound.IsDisposed || current.sound.HasStopped))
                {
                    Stop(index);
                    waiting[index] = (location, now, api.World.Rand.NextDouble());
                }
                if (waiting.TryGetValue(index, out var pause))
                {
                    if (pause.location.Equals(location) && now - pause.finishedAt < PauseSeconds(windStrength, pause.jitter) * 1000)
                        continue;
                    waiting.Remove(index);
                }
                Vec3f position = new((float)soundPosition.X, (float)soundPosition.Y, (float)soundPosition.Z);
                if (!playing.TryGetValue(index, out current))
                {
                    ILoadedSound sound = api.World.LoadSound(new SoundParams
                    {
                        Location = location,
                        Position = position,
                        RelativePosition = false,
                        ShouldLoop = false,
                        DisposeOnFinish = false,
                        SoundType = EnumSoundType.Ambient,
                        Range = range,
                        Volume = soundVolume
                    });
                    if (sound == null) continue;
                    playing[index] = current = (location, sound);
                    sound.Start();
                }
                current.sound.SetPosition(position);
                current.sound.SetVolume(soundVolume);
            }
            foreach (int index in playing.Keys.Where(index => !active.Contains(index)).ToArray()) Stop(index);
            foreach (int index in waiting.Keys.Where(index => !active.Contains(index)).ToArray()) waiting.Remove(index);
        }

        private static double PauseSeconds(float windStrength, double jitter)
            => (8 + 12 * jitter) * (1 - windStrength) + (0.5 + 2.5 * jitter) * windStrength;

        private bool IsUnderCover(EntityPos position, double heightOffset)
        {
            double height = position.Y + heightOffset;
            BlockPos blockPos = new((int)Math.Floor(position.X), (int)Math.Floor(height),
                (int)Math.Floor(position.Z), position.Dimension);
            return api.World.BlockAccessor.GetRainMapHeightAt(blockPos) > height;
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
            waiting.Clear();
        }

        public void Dispose()
        {
            StopAll();
            disposed = true;
        }
    }
}
