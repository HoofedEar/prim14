using Content.Shared.Storage;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;

namespace Content.Server.Gathering.Components;

[RegisterComponent]
[Friend(typeof(GatherableSystem))]
public sealed class GatherableComponent : Component
{
    /// <summary>
    /// Whitelist for specifying the kind of tools can be used on a resource
    /// </summary>
    [ViewVariables]
    [DataField("whitelist")] 
    public EntityWhitelist? ToolWhitelist;

    /// <summary>
    /// Loot table for the resource
    /// </summary>
    [DataField("loot")] public List<EntitySpawnEntry> Loot = new();

    public float BaseMineTime = 1.0f;
}
