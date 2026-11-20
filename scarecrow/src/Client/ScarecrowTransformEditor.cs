using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.Client.NoObf;

namespace Scarecrow
{
    /// <summary>Registers native .tfedit targets and refreshes equipped client previews on edits.</summary>
    internal sealed class ScarecrowTransformEditor : IDisposable
    {
        private readonly ICoreClientAPI api;
        private readonly List<TransformConfig> ownedEntries = new();
        private bool disposed;

        public ScarecrowTransformEditor(ICoreClientAPI api)
        {
            this.api = api;
            Add(ScarecrowArmTransform.LeftAttribute, "scarecrow:transform-leftarm");
            Add(ScarecrowArmTransform.RightAttribute, "scarecrow:transform-rightarm");
            api.Event.RegisterEventBusListener(OnTransformChanged, filterByEventName: "onsettransform");
        }

        private void Add(string attribute, string title)
        {
            if (GuiDialogTransformEditor.extraTransforms.Any(entry => entry.AttributeName == attribute)) return;
            var entry = new TransformConfig { AttributeName = attribute, Title = Lang.Get(title) };
            GuiDialogTransformEditor.extraTransforms.Add(entry);
            ownedEntries.Add(entry);
        }

        private void OnTransformChanged(string name, ref EnumHandling handling, IAttribute data)
        {
            if (disposed || data is not TreeAttribute tree || tree.GetBool("preventDefault")) return;
            string target = tree.GetString("target");
            if (target != ScarecrowArmTransform.LeftAttribute && target != ScarecrowArmTransform.RightAttribute) return;
            CollectibleObject collectible = api.World.Player?.InventoryManager.ActiveHotbarSlot?.Itemstack?.Collectible;
            if (collectible == null) return;

            // Match native editor behavior: this is a local development preview. Exported JSON
            // must be saved to assets; do not synchronize arbitrary client edits to the server.
            collectible.Attributes ??= new JsonObject(new JObject());
            collectible.Attributes.Token[target] = JToken.FromObject(ModelTransform.CreateFromTreeAttribute(tree));
            tree.SetBool("preventDefault", true);
            foreach (var entity in api.World.LoadedEntities.Values)
            {
                var attachments = entity.GetBehavior<ScarecrowAttachments>();
                if (attachments?.Inventory?.Any(slot => ReferenceEquals(slot.Itemstack?.Collectible, collectible)) == true)
                    entity.MarkShapeModified();
            }
        }

        public void Dispose()
        {
            disposed = true;
            foreach (TransformConfig entry in ownedEntries) GuiDialogTransformEditor.extraTransforms.Remove(entry);
            ownedEntries.Clear();
            // Event-bus listeners belong to this client API's lifetime. Guard queued events
            // after disposal; the client event manager releases its listeners on shutdown.
        }
    }
}
