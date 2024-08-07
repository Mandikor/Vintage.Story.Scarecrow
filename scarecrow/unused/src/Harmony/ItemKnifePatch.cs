namespace scarecrow.unused.src.Harmony
{
    [HarmonyPatch(typeof(EntityStrawDummy))]
    public class EntityStrawDummyPatch
    {
        [HarmonyPatch("GetInteractionHelp")]
        public static bool Prefix(EntityAgent byEntity, EntitySelection entitySel)
        {
            if (entitySel == null)
            {
                return true;
            }
            EntityBehaviorHarvestable behavior = entitySel.Entity.GetBehavior<EntityBehaviorDisassemblable>();
            return entitySel.Entity.HasBehavior("harvestable") || !byEntity.Controls.Sneak || behavior == null || !behavior.Harvestable;
        }
    }
}
