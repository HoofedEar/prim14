using System.Threading;
using Content.Shared.Storage;
using Content.Shared.Whitelist;

namespace Content.Server.Prim14.UseWith;

[RegisterComponent]
[Friend(typeof(UseWithSystem))]
public sealed class UseWithComponent : Component
{
    [ViewVariables]
    [DataField("result")]
    public string? Result;

    [ViewVariables]
    [DataField("spawnCount")]
    public int SpawnCount = 1;

    [ViewVariables]
    [DataField("whitelist")]
    public EntityWhitelist? UseWithWhitelist;

    [ViewVariables]
    [DataField("inHand")]
    public bool UseInHand;

    public CancellationTokenSource? CancelToken;
}
