using Robust.Shared.Serialization;

namespace Content.Shared.Prim14;

/// <summary>
/// Stores bools for if the machine is on
/// and if it's currently running.
/// Used for the visualizer
/// </summary>
[Serializable, NetSerializable]
public enum BushVisuals
{
    Ready,
    Empty
}