#nullable disable

namespace Scarecrow
{
    /// <summary>Selects the little entity and omits arm slots from its stored-equipment tooltip.</summary>
    public class ItemLittleScareCrow : ItemScarecrowBase
    {
        protected override bool HasArmSlots => false;

        protected override string EntityCode => "scarecrow:little-scarecrow";

        protected override string PlaceHelpLangCode => "scarecrow:heldhelp-place-lsc";
    }
}
