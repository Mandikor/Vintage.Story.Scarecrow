using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace Scarecrow
{
    /// <summary>Fills the title-only guide asset with localized text and native recipe links before display.</summary>
    internal sealed class ScarecrowHandbook : IDisposable
    {
        private readonly ICoreClientAPI api;
        private readonly ModSystemSurvivalHandbook handbook;
        public ScarecrowHandbook(ICoreClientAPI api)
        {
            this.api = api;
            handbook = api.ModLoader.GetModSystem<ModSystemSurvivalHandbook>(false);
            if (handbook != null) handbook.OnInitCustomPages += PrepareGuide;
        }

        private void PrepareGuide(List<GuiHandbookPage> pages)
        {
            var page = pages.OfType<GuiHandbookTextPage>().FirstOrDefault(p => p.PageCode == ScarecrowEquipmentInfo.GuidePage);
            if (page == null) return;
            page.Text = CreateText(api);
            page.Init(api);
        }

        internal static string CreateText(ICoreClientAPI api)
        {
            string Link(string code)
            {
                var item = api.World.GetItem(new AssetLocation(code));
                if (item == null) return code;
                var stack = new ItemStack(item);
                string label = stack.GetName().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
                return "<a href=\"handbook://" + GuiHandbookItemStackPage.PageCodeForStack(stack) + "\">" + label + "</a>";
            }
            return Lang.Get("scarecrow:guide-text", Link("scarecrow:scarecrow"),
                Link("scarecrow:little-scarecrow"), Link("game:strawdummy"))
                + "\n\n" + Lang.Get("scarecrow:guide-radius-defaults")
                + "\n\n" + Lang.Get("scarecrow:access-guide")
                + "\n/scarecrow access add &lt;player&gt; equipment"
                + "\n/scarecrow access add &lt;player&gt; full"
                + "\n/scarecrow access remove &lt;player&gt;"
                + "\n/scarecrow access list";
        }

        public void Dispose()
        {
            if (handbook != null) handbook.OnInitCustomPages -= PrepareGuide;
        }
    }
}
