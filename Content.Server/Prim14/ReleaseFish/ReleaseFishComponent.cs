using Robust.Shared.Containers;

namespace Content.Server.Prim14.ReleaseFish;

[RegisterComponent]
[Friend(typeof(ReleaseFishSystem))]
public sealed class ReleaseFishComponent : Component
{
    /// <summary>
    ///     Container of entities inside this body of water.
    /// </summary>
    [ViewVariables] 
    public Container Container = default!;
}