namespace scarecrow.unused.src.HighlightDana;

public class HighlightMycelium : ModSystemHighlight
{
    public override string ThreadName => "Scarecrow:Mycelium";

    public override string Name => Lang.Get("scarecrow:HighlightMycelium");
    public override string HotkeyCode => "scarecrow:highlightmycelium";

    public static int Radius => 64;

    public static int HighlightColor => Constants.ColorsRGBA.Yellow;

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        api.Input.RegisterHotKey(HotkeyCode, Constants.ToggleName(Name), GlKeys.M, HotkeyType.HelpAndOverlays, shiftPressed: true);
        api.Input.SetHotKeyHandler(HotkeyCode, _ => ToggleRun(api));
    }

    public override void OnRunning(ICoreClientAPI capi)
    {
        List<BlockPos> positions = new();
        List<int> colors = new();
        BlockPos playerPos = capi.World.Player.Entity.Pos.AsBlockPos;

        capi.World.BlockAccessor.WalkBlocks(playerPos.AddCopy(-Radius, -Radius, -Radius), playerPos.AddCopy(Radius, Radius, Radius), (_, x, y, z) =>
        {
            BlockPos bPos = new(x, y, z);

            BlockEntityMycelium beMycelium = GetMycelium(bPos, capi);
            if (beMycelium == null)
            {
                return;
            }
            positions.Add(bPos);
            colors.Add(HighlightColor);

            int range = beMycelium.GetField<int>("growRange");

            capi.World.BlockAccessor.WalkBlocks(bPos.AddCopy(-range, -range, -range), bPos.AddCopy(range, range, range), (_, x, y, z) =>
            {
                BlockPos bPos = new(x, y, z);

                if (positions.Contains(bPos)) return;

                positions.Add(bPos);
                colors.Add(HighlightColor);
            });
        });

        capi.Event.EnqueueMainThreadTask(new Action(() => capi.World.HighlightBlocks(capi.World.Player, 5229, positions, colors)), ThreadName);
    }

    private static BlockEntityMycelium GetMycelium(BlockPos pos, ICoreAPI api) => api.World.BlockAccessor.GetBlockEntity(pos) as BlockEntityMycelium;
}