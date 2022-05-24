namespace Content.Client.Prim14;

[RegisterComponent]
public sealed class BushVisualsComponent : Component
{
    [DataField("stateReady")]
    public string? StateReady;

    [DataField("stateEmpty")]
    public string? StateEmpty;
}