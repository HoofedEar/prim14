using Content.Shared.Storage;
using Content.Shared.Whitelist;

namespace Content.Server.Anprim14.Gathering.Components;

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
