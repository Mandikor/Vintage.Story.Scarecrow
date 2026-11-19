#nullable disable

namespace Scarecrow
{
    /// <summary>Selects the full-size entity; placement and equipment persistence are shared with the little variant.</summary>
    public class ItemScareCrow : ItemScarecrowBase
    {
        protected override string EntityCode => "scarecrow:scarecrow";

        protected override string PlaceHelpLangCode => "scarecrow:heldhelp-place-sc";
    }
}
