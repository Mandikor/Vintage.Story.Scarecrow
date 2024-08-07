namespace scarecrow.unused.src.HighlightSkif
{
    public class ClientMod : ModSystem
    {

        private ICoreClientAPI capi;
        private List<LandClaim> claims;
        private bool show = false;
        private long timerForUpdateClaimRadar;
        private long countShowClaim = 0;

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            capi = api;

            api.Input.RegisterHotKey("showClaims", Lang.Get("Show claims"), GlKeys.R, HotkeyType.GUIOrOtherControls, ctrlPressed: true);
            api.Input.SetHotKeyHandler("showClaims", OnOffClaimRadar);

            claims = new List<LandClaim>();

        }

        private bool OnOffClaimRadar(KeyCombination keyCombination)
        {
            if (!show)
            {
                show = true;
                timerForUpdateClaimRadar = capi.World.RegisterGameTickListener(new Action<float>(UpdateClaimRadar), 1000, 0);
                GenerateListShowClaim();
                AddClaimsToHighlight();
            }
            else
            {
                show = false;
                capi.World.UnregisterGameTickListener(timerForUpdateClaimRadar);
                ClearHighlightBlocks();


            }
            return true;
        }

        private void ClearHighlightBlocks()
        {
            for (int i = 0; i < countShowClaim; i++)
            {
                capi.World.HighlightBlocks(capi.World.Player, i + 100, new List<BlockPos>(), EnumHighlightBlocksMode.Absolute, EnumHighlightShape.Cube);
            }
        }

        private void UpdateClaimRadar(float dt)
        {
            GenerateListShowClaim();
            ClearHighlightBlocks();
            AddClaimsToHighlight();
        }

        private void GenerateListShowClaim()
        {
            claims.Clear();
            List<LandClaim> allClaims = capi.World.Claims.All.ToList();
            foreach (LandClaim claim in allClaims)
            {
                foreach (Cuboidi area in claim.Areas)
                {
                    if (DistanceXZTo(capi.World.Player.Entity.Pos, area.Center.ToBlockPos()) < ClientSettings.ViewDistance ||
                        DistanceXZTo(capi.World.Player.Entity.Pos, area.Start.ToBlockPos()) < ClientSettings.ViewDistance ||
                        DistanceXZTo(capi.World.Player.Entity.Pos, area.End.ToBlockPos()) < ClientSettings.ViewDistance)
                    {
                        claims.Add(claim);
                        break;
                    }
                }
            }
        }

        private static double DistanceXZTo(EntityPos player, BlockPos cube)
        {

            return Math.Sqrt(Math.Pow(player.X - cube.X, 2) + Math.Pow(player.Z - cube.Z, 2));
        }

        private void AddClaimsToHighlight()
        {
            int i = 0;
            List<BlockPos> list = new();
            foreach (LandClaim claim in claims)
            {
                List<int> colors = new() { OwnerNameToColor(claim.LastKnownOwnerName) };
                foreach (Cuboidi area in claim.Areas)
                {
                    list.Clear();
                    list.Add(new BlockPos(area.MinX, area.MinY, area.MinZ));
                    list.Add(new BlockPos(area.MaxX, area.MaxY, area.MaxZ));
                    capi.World.HighlightBlocks(capi.World.Player, i + 100, list, colors, EnumHighlightBlocksMode.Absolute, EnumHighlightShape.Cube);
                    i++;
                }

            }
            countShowClaim = i;

        }

        public static int OwnerNameToColor(string input)
        {
            const uint firstPrime = 328514948;
            const uint secondPrime = 221669321;
            const uint thirdPrime = 301287251;

            const uint FNVOffsetBasis = 2166136261;

            uint hashR = FNVOffsetBasis;
            uint hashG = FNVOffsetBasis;
            uint hashB = FNVOffsetBasis;
            foreach (byte c in input.Select(v => (byte)v))
            {
                hashR *= firstPrime;
                hashR ^= c;
                hashG *= secondPrime;
                hashG ^= c;
                hashB *= thirdPrime;
                hashB ^= c;
            }

            return ColorUtil.ToRgba(150, (int)(hashR % 256), (int)(hashG % 256), (int)(hashB % 256));
        }

    }

}
