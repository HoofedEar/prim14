namespace Content.Client.Anprim14;

[RegisterComponent]
public sealed class AnvilVisualsComponent : Component
{
    [DataField("stateReady")]
    public string? StateReady;

    [DataField("stateEmpty")]
    public string? StateEmpty;
}
