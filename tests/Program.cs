using Newtonsoft.Json.Linq;
using Scarecrow;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;
using System.Reflection;

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
    Console.WriteLine("PASS: " + message);
}

static ItemStack Tool(string code, string category = "toolholding", string shapeSlot = "sc-leftarm")
{
    JObject attachment = new()
    {
        ["attachedShapeBySlotCode"] = new JObject { [shapeSlot] = new JObject { ["base"] = "scarecrow:test" } }
    };
    if (category != null) attachment["categoryCode"] = category;
    return new ItemStack(new Item
    {
        Code = new AssetLocation(code),
        Attributes = new JsonObject(new JObject { ["attachableToEntity"] = attachment }),
        MaxStackSize = 1
    });
}

var inventory = new InventoryGeneric(4, "scarecrow-tests", null);
var arm = new ScarecrowAttachmentSlot(inventory, new[] { "toolholding" }, new[] { "game:hoe-*", "game:scythe-*" });
var vanilla = new ItemSlotWearable(inventory, new[] { "toolholding" });
var hoe = Tool("game:hoe-copper");
var axe = Tool("game:axe-copper");
AttachableItems.SupplyMissingCategory(hoe.Collectible);
Check(IAttachableToEntity.FromCollectible(hoe.Collectible).GetCategoryCode(hoe) == "toolholding", "Original tool category is preserved");
Check(vanilla.CanHold(new DummySlot(hoe)), "Vanilla stand accepts the hoe");
Check(arm.CanHold(new DummySlot(hoe)), "Scarecrow arm accepts an allowed hoe");
Check(arm.CanTakeFrom(new DummySlot(hoe)), "Allowed hoe can be transferred into the arm");
Check(vanilla.CanHold(new DummySlot(axe)), "Vanilla stand still accepts other tools");
Check(!arm.CanHold(new DummySlot(axe)) && !arm.CanTakeFrom(new DummySlot(axe)), "Same-category axe is rejected by both insertion paths");
Check(!arm.CanHold(new DummySlot(Tool("other:hoe-copper"))), "Whitelist respects the mod domain");
Check(!arm.Allows(null), "Empty hand is not an attachment");

var can = Tool("game:wateringcan-blue-fired", null, "sc-foot");
AttachableItems.SupplyMissingCategory(can.Collectible);
var foot = new ScarecrowAttachmentSlot(inventory, new[] { "scarecrowattachment" }, new[] { "game:wateringcan-*-fired" });
Check(foot.CanHold(new DummySlot(can)), "Previously uncategorized watering can can be attached");
Check(!arm.CanHold(new DummySlot(can)) && !foot.CanHold(new DummySlot(hoe)), "Foot and arm rules are independent");
var foreign = Tool("other:accessory", null, "other-slot");
AttachableItems.SupplyMissingCategory(foreign.Collectible);
Check(IAttachableToEntity.FromCollectible(foreign.Collectible).GetCategoryCode(foreign) == null, "Unrelated attachment metadata is untouched");
var custom = Tool("leaflitter:rake", "original-rake-category");
AttachableItems.SupplyMissingCategory(custom.Collectible);
Check(IAttachableToEntity.FromCollectible(custom.Collectible).GetCategoryCode(custom) == "original-rake-category", "Other mods retain their existing categories");

IWorldAccessor world = DispatchProxy.Create<IWorldAccessor, TestWorld>();
((TestWorld)world).Collectibles = new() { hoe.Collectible, axe.Collectible, custom.Collectible, can.Collectible };
var behavior = new ScarecrowAttachments(new EntityScarecrow());
var prepare = typeof(ScarecrowAttachments).GetMethod("PrepareConfig", BindingFlags.Instance | BindingFlags.NonPublic);
string repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../.."));
foreach (string name in new[] { "scarecrow", "little-scarecrow" })
{
    var definition = JObject.Parse(File.ReadAllText(Path.Combine(repoRoot, "scarecrow/assets/scarecrow/entities/nonliving", name + ".json")));
    var config = new JsonObject(definition["behaviorConfigs"]["scarecrowattachments"]);
    string original = config.Token.ToString();
    var prepared = (JsonObject)prepare.Invoke(behavior, new object[] { config, world });
    var slots = (JArray)prepared["wearableSlots"].Token;
    Check(config.Token.ToString() == original, name + ": original shared config is not mutated");
    foreach (var slot in slots)
    {
        var categoryCodes = slot["forCategoryCodes"].ToObject<string[]>();
        string slotCode = slot["code"].Value<string>();
        if (slotCode.Contains("arm"))
            Check(categoryCodes.Contains("toolholding") && categoryCodes.Contains("original-rake-category"), name + ": arm accepts original game and mod categories");
        if (slotCode == "sc-foot")
            Check(categoryCodes.Contains("scarecrowattachment"), name + ": foot accepts the supplied fallback category");
    }
    Check(definition["client"]["behaviors"].Any(b => b["code"].Value<string>() == "scarecrowattachments")
        && definition["server"]["behaviors"].Any(b => b["code"].Value<string>() == "scarecrowattachments"), name + ": custom slots enabled on client and server");
}

Console.WriteLine("Attachment regression checks passed.");

public class TestWorld : DispatchProxy
{
    public List<CollectibleObject> Collectibles = new();
    protected override object Invoke(MethodInfo targetMethod, object[] args)
        => targetMethod.Name == "get_Collectibles" ? Collectibles : throw new NotSupportedException(targetMethod.Name);
}
