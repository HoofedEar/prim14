using Content.Shared.Storage;
using Content.Shared.Whitelist;

namespace Content.Server.Anprim14;

[RegisterComponent]
[Friend(typeof(UseWithSystem))]
public sealed class UseWithComponent : Component
{
    [DataField("results")]
    public List<EntitySpawnEntry> Results = new();

    [DataField("spawnCount")] 
    public int SpawnCount;
    
    [ViewVariables]
    [DataField("whitelist")] 
    public EntityWhitelist? UseWithWhitelist;
}
