using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace Scarecrow
{
    /// <summary>
    /// Gives a collectible its own attachment category for the scarecrow slots, independent of the
    /// shared vanilla "attachableToEntity/categoryCode" attribute.
    /// <para>
    /// IAttachableToEntity.FromCollectible prefers a collectible behavior over the attribute, so this
    /// behavior wins even when another mod rewrites the attribute after our patches. The category code
    /// is part of the behavior properties and therefore also reaches the client through the item type
    /// packet. Shapes, textures and element lists are still read from the attribute, so the material
    /// variants stay data driven in the json patches.
    /// </para>
    /// </summary>
    public class ScarecrowAttachableBehavior : CollectibleBehavior, IAttachableToEntity
    {
        private string categoryCode;
        private IAttachableToEntity attributeAttachable;
        private bool attributeAttachableResolved;

        public ScarecrowAttachableBehavior(CollectibleObject collObj) : base(collObj)
        {
        }

        public override void Initialize(JsonObject properties)
        {
            base.Initialize(properties);
            categoryCode = properties["categoryCode"].AsString();
        }

        public string GetCategoryCode(ItemStack stack)
        {
            if (!string.IsNullOrEmpty(categoryCode)) return categoryCode;

            return AttributeAttachable?.GetCategoryCode(stack);
        }

        public bool IsAttachable(Entity toEntity, ItemStack itemStack) => true;

        public CompositeShape GetAttachedShape(ItemStack stack, string slotCode)
        {
            CompositeShape shape = AttributeAttachable?.GetAttachedShape(stack, slotCode);
            if (shape != null) return shape;

            return stack.Class == EnumItemClass.Block ? stack.Block?.Shape : stack.Item?.Shape;
        }

        public string[] GetDisableElements(ItemStack stack) => AttributeAttachable?.GetDisableElements(stack);

        public string[] GetKeepElements(ItemStack stack) => AttributeAttachable?.GetKeepElements(stack);

        public string GetTexturePrefixCode(ItemStack stack) => AttributeAttachable?.GetTexturePrefixCode(stack);

        public int RequiresBehindSlots
        {
            get => AttributeAttachable?.RequiresBehindSlots ?? 0;
            // The engine only reads this value, it is never assigned at runtime.
            set { }
        }

        public void CollectTextures(ItemStack stack, Shape gearShape, string texturePrefixCode, Dictionary<string, CompositeTexture> intoDict)
        {
            IAttachableToEntity.CollectTexturesFromCollectible(stack, texturePrefixCode, gearShape, intoDict);
        }

        /// <summary>
        /// The attribute based attachable of the collectible, used for shapes and textures.
        /// Resolved lazily: on the client the behavior is created from the item type packet before
        /// the item attributes are assigned.
        /// </summary>
        private IAttachableToEntity AttributeAttachable
        {
            get
            {
                if (attributeAttachableResolved) return attributeAttachable;

                attributeAttachable = collObj?.Attributes?["attachableToEntity"]
                    ?.AsObject<AttributeAttachableToEntity>(null, collObj.Code?.Domain);
                attributeAttachableResolved = attributeAttachable != null;

                return attributeAttachable;
            }
        }
    }
}
