#nullable disable

namespace Scarecrow
{
    public class ItemLittleScareCrow : ItemScarecrowBase
    {
        protected override bool HasArmSlots => false;

        protected override string EntityCode => "scarecrow:little-scarecrow";

        protected override string PlaceHelpLangCode => "scarecrow:heldhelp-place-lsc";
    }
}
