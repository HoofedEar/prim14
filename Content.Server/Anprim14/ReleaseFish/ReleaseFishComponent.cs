using Robust.Shared.Containers;

namespace Content.Server.Anprim14.ReleaseFish;

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