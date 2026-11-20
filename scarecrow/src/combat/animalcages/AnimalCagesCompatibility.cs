using System;
using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;

namespace Scarecrow
{
    /// <summary>Marks cage releases before the world spawn event, without changing natural spawns.</summary>
    internal sealed class AnimalCagesCompatibility : IDisposable
    {
        private const string PatchId = "scarecrow.compatibility.animalcages.release";
        private readonly Harmony harmony;
        private readonly MethodInfo target;

        private AnimalCagesCompatibility(MethodInfo target)
        {
            this.target = target;
            harmony = new Harmony(PatchId);
            harmony.Patch(target, postfix: new HarmonyMethod(typeof(AnimalCagesCompatibility), nameof(MarkReleasedAnimal)));
        }

        public static IDisposable TryInstall(ICoreServerAPI api)
        {
            // Resolve the optional assembly only after checking availability. No Animal Cages
            // types or hooks are needed on clients or when the mod is absent.
            if (!api.ModLoader.IsModEnabled("animalcages")) return null;
            Type cage = AccessTools.TypeByName("Animalcages.BlockEntityAnimalCage");
            MethodInfo getter = cage == null ? null : AccessTools.DeclaredMethod(cage, "getCapturedEntity", Type.EmptyTypes);
            if (getter == null || getter.ReturnType != typeof(Entity) || !typeof(BlockEntity).IsAssignableFrom(cage))
            {
                api.Logger.Warning("[Scarecrow] Animal Cages release hook not found; compatibility patch was not installed.");
                return null;
            }

            try
            {
                return new AnimalCagesCompatibility(getter);
            }
            catch (Exception exception)
            {
                new Harmony(PatchId).Unpatch(getter, HarmonyPatchType.Postfix, PatchId);
                api.Logger.Warning("[Scarecrow] Could not install Animal Cages release hook: {0}", exception.Message);
                return null;
            }
        }

        // Animal Cages 5.0.1 calls this getter from OnBlockBroken immediately before
        // SpawnEntity, for both cage sizes. Serialized wild animals retain their old
        // entityspawner origin; replace it on the restored copy before our spawn filter runs.
        private static void MarkReleasedAnimal(BlockEntity __instance, Entity __result)
        {
            if (__instance.Api?.Side != EnumAppSide.Server || __result == null) return;
            __result.Attributes?.SetString("origin", "animalcages-release");
        }

        public void Dispose() => harmony.Unpatch(target, HarmonyPatchType.Postfix, PatchId);
    }
}
