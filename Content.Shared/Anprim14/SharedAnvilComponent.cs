using Robust.Shared.Serialization;

namespace Content.Shared.Anprim14;

public sealed class SharedAnvilComponent
{
    [ViewVariables]
    public AnvilState State = AnvilState.Empty;
    
    [Serializable, NetSerializable]
    public enum AnvilState
    {
        Empty,
        Ready
    }
}