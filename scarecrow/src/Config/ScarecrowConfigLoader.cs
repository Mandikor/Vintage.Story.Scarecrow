using System;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Scarecrow
{
    /// <summary>Migrates raw JSON defaults while retaining user values and backing up files before writes.</summary>
    internal static class ScarecrowConfigLoader
    {
        private const string FileName = "scarecrow.json";

        public static ScarecrowConfig Load(ICoreAPI api)
        {
            ScarecrowConfig config;
            JObject original = null;
            JObject updated;
            bool save;
            try
            {
                // The non-generic API rejects non-object JSON and distinguishes a missing file.
                JsonObject loaded = api.LoadModConfig(FileName);
                if (loaded == null)
                {
                    config = new ScarecrowConfig();
                    updated = JObject.FromObject(config);
                    save = true;
                }
                else
                {
                    original = loaded.Token as JObject
                        ?? throw new InvalidOperationException("The config must contain a JSON object.");
                    JProperty versionProperty = original.Property("ConfigVersion", StringComparison.OrdinalIgnoreCase);
                    int version = 1;
                    if (versionProperty != null)
                    {
                        if (versionProperty.Value.Type != JTokenType.Integer
                            || !int.TryParse(versionProperty.Value.ToString(), out version) || version < 1)
                            throw new InvalidOperationException("ConfigVersion must be a positive integer.");
                    }
                    if (version > ScarecrowConfig.CurrentConfigVersion)
                    {
                        config = original.ToObject<ScarecrowConfig>();
                        api.Logger.Warning("[Scarecrow] ConfigVersion {0} is newer than supported version {1}; loading known settings without rewriting the file.",
                            version, ScarecrowConfig.CurrentConfigVersion);
                        return config;
                    }

                    updated = (JObject)original.DeepClone();
                    save = versionProperty == null || version != ScarecrowConfig.CurrentConfigVersion;
                    JObject defaults = JObject.FromObject(new ScarecrowConfig());
                    foreach (JProperty property in defaults.Properties())
                    {
                        if (property.Name == nameof(ScarecrowConfig.ConfigVersion)) continue;
                        // Match the serializer's case-insensitive property lookup. Explicit values,
                        // including false, empty arrays and null, are never replaced with defaults.
                        if (updated.Property(property.Name, StringComparison.OrdinalIgnoreCase) != null) continue;
                        updated.Add(property.Name, property.Value.DeepClone());
                        save = true;
                    }
                    JProperty updatedVersion = updated.Property("ConfigVersion", StringComparison.OrdinalIgnoreCase);
                    if (updatedVersion == null)
                        updated.AddFirst(new JProperty("ConfigVersion", ScarecrowConfig.CurrentConfigVersion));
                    else updatedVersion.Value = ScarecrowConfig.CurrentConfigVersion;

                    // Validate before touching disk. Unknown properties stay in the raw document.
                    config = updated.ToObject<ScarecrowConfig>();
                }
            }
            catch (Exception exception)
            {
                api.Logger.Error("[Scarecrow] Could not read ModConfig/scarecrow.json, using defaults without modifying the file. Fix the file and restart. Error: {0}", exception.Message);
                return new ScarecrowConfig();
            }

            if (save)
            {
                try
                {
                    // Back up the original values before a migration, not merely the typed subset.
                    // A backup failure aborts the write; loaded settings remain active regardless.
                    if (original != null) api.StoreModConfig(new JsonObject(original), FileName + ".bak");
                    api.StoreModConfig(new JsonObject(updated), FileName);
                    api.Logger.Notification(original == null
                        ? "[Scarecrow] Wrote default config to ModConfig/scarecrow.json"
                        : "[Scarecrow] Updated ModConfig/scarecrow.json to ConfigVersion {0}; existing values were preserved and backed up to scarecrow.json.bak.",
                        ScarecrowConfig.CurrentConfigVersion);
                }
                catch (Exception exception)
                {
                    api.Logger.Error("[Scarecrow] Could not save ModConfig/scarecrow.json; continuing with the loaded settings. Error: {0}", exception.Message);
                }
            }
            return config;
        }
    }
}
