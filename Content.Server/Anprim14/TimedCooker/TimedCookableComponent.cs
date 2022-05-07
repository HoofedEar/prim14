namespace Content.Server.Anprim14.TimedCooker;

[RegisterComponent]
public sealed class TimedCookableComponent : Component
{
    /// <summary>
    /// Valid recipe when insert into TimedCookerComponent
    /// </summary>
    [ViewVariables]
    [DataField("recipe", required: true)] 
    public string? Recipe = default!;
}