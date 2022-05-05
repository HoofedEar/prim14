using Content.Shared.Storage;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Anprim14.Gathering.Components;

[RegisterComponent]
[Friend(typeof(GatherableSystem))]
public sealed class GatherableComponent : Component
{
    /// <summary>
    /// Bool if we are using an ordered loot table
    /// </summary>
    [DataField("useOrderedLoot")] 
    public bool UseOrderedLoot;
    
    
    /// <summary>
    /// Whitelist for specifying the kind of tools can be used on a resource
    /// Supports multiple tags.
    /// </summary>
    [ViewVariables]
    [DataField("whitelist", required: true)] 
    public EntityWhitelist? ToolWhitelist;

    /// <summary>
    /// Loot table for the resource
    /// </summary>
    [DataField("loot")] 
    public List<EntitySpawnEntry> Loot = new();

    /// <summary>
    /// If this is defined, loot will be determined by the order of tags in the white list.
    /// For example:
    /// whitelist:
    /// tags:
    /// - FishingRod - gives fish
    /// - Hatchet - gives log
    /// loot:
    /// - id: Fish
    /// - id: Log
    /// </summary>
    /// , customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))
    [DataField("orderedLoot")] 
    public Dictionary<string, List<EntitySpawnEntry>> OrderedLoot = new();

    public float BaseMineTime = 1.0f;
}
