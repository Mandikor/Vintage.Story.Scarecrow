using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace Scarecrow
{
    /// <summary>Registers shared types, loads settings and delegates optional-mod startup hooks.</summary>
    public class Core : ModSystem
    {
        public ScarecrowConfig Config { get; private set; }
        public ScarecrowClientConfig ClientConfig { get; private set; }
        private System.IDisposable serverCompatibility;
        private System.IDisposable transformEditor;
        private System.IDisposable handbook;
        private System.IDisposable equipmentTooltip;

        // Register stable JSON-facing names on both sides before entities or inventories load.
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            LoadConfig(api);

            api.RegisterItemClass("ItemLittleScareCrow", typeof(ItemLittleScareCrow));
            api.RegisterItemClass("ItemScareCrow", typeof(ItemScareCrow));

            api.RegisterEntityBehaviorClass("scarecrowattachments", typeof(ScarecrowAttachments));

            api.RegisterEntity("EntityScarecrow", typeof(EntityScarecrow));
        }

        private void LoadConfig(ICoreAPI api)
        {
            Config = ScarecrowConfigLoader.Load(api);
        }

        // Collectible metadata is available after patches, so categories can be inspected safely.
        public override void AssetsFinalize(ICoreAPI api)
        {
            api.World.Logger.Event("########## started '{0}' mod ##########", Mod.Info.Name);

            AttachableItems.Apply(api);
            ModCompatibility.FinalizeEntities(api);
            // On clients this runs after server types arrive, before LevelFinalize builds handbook pages.
            if (api is ICoreClientAPI client)
                ScarecrowEquipmentInfo.Install(client, ClientConfig ??= ScarecrowConfigLoader.LoadClient(client));
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            ClientConfig = ScarecrowConfigLoader.LoadClient(api);
            if (ClientConfig.ShowEquipmentTooltips) equipmentTooltip = new ScarecrowEquipmentTooltipTail();
            ModCompatibility.RegisterClientRenderers(api);
            transformEditor = new ScarecrowTransformEditor(api);
            handbook = new ScarecrowHandbook(api);
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);
            RegisterCommands(api);
            serverCompatibility = ModCompatibility.StartServer(api);
        }

        private void RegisterCommands(ICoreServerAPI api)
        {
            _ = new ScarecrowAccessCommands(api);
            _ = new ScarecrowDiagnostics(api);
            api.ChatCommands.GetOrCreate("scarecrow")
                .BeginSubCommand("reload")
                .WithDescription(Lang.Get("scarecrow:reload-help"))
                .RequiresPrivilege(Privilege.controlserver)
                .HandleWith(args => ReloadConfiguration(api, args))
                .EndSubCommand();
        }

        private TextCommandResult ReloadConfiguration(ICoreServerAPI api, TextCommandCallingArgs args)
        {
            string language = (args.Caller?.Player as IServerPlayer)?.LanguageCode
                ?? Lang.DefaultLocale;
            if (!ScarecrowConfigLoader.TryReload(api, out ScarecrowConfig settings))
                return TextCommandResult.Error(Lang.GetL(language, "scarecrow:reload-failed"));

            Config = settings;
            int count = 0;
            foreach (var entity in api.World.LoadedEntities.Values)
            {
                if (entity is not EntityScarecrow crow) continue;
                crow.ApplyConfiguration(settings);
                count++;
            }
            api.Logger.Notification("[Scarecrow] Reloaded configuration; updated {0} loaded scarecrows.", count);
            return TextCommandResult.Success(Lang.GetL(language, "scarecrow:reload-success", count));
        }

        public override void Dispose()
        {
            serverCompatibility?.Dispose();
            serverCompatibility = null;
            transformEditor?.Dispose();
            transformEditor = null;
            handbook?.Dispose();
            handbook = null;
            equipmentTooltip?.Dispose();
            equipmentTooltip = null;
            base.Dispose();
        }
    }
}
