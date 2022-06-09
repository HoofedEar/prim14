using System.Threading;
using Content.Shared.Storage;
using Content.Shared.Whitelist;

namespace Content.Server.Prim14.UseWith;

[RegisterComponent]
[Friend(typeof(UseWithSystem))]
public sealed class UseWithComponent : Component
{
    [ViewVariables]
    [DataField("results")]
    public List<EntitySpawnEntry> Results = new();

    [ViewVariables]
    [DataField("spawnCount")]
    public int SpawnCount;

    [ViewVariables]
    [DataField("whitelist")]
    public EntityWhitelist? UseWithWhitelist;

    [ViewVariables]
    [DataField("inHand")]
    public bool UseInHand;

    public CancellationTokenSource? CancelToken;
}

public sealed class UseWithEvent : EntityEventArgs
{
    public readonly EntityUid User;
    public readonly UseWithComponent UseWith;

    public UseWithEvent(EntityUid user, UseWithComponent usewith)
    {
        User = user;
        UseWith = usewith;
    }
}
