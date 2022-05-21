using Content.Server.Anprim14.TimedCooker;

namespace Content.Server.Anprim14.Blacksmithing.Components;

[RegisterComponent]
[Friend(typeof(TimedCookerSystem), typeof(KilnSystem))]
public sealed class KilnComponent : TimedCookerComponent
{
    /// <summary>
    /// This limits how many jugs it can process at once.
    /// </summary>
    [ViewVariables]
    public int KilnMax = 1;

    /// <summary>
    /// This is how we track how much wood it current is holding.
    /// </summary>
    [ViewVariables]
    public int KilnWoodStorage = 0;

    /// <summary>
    /// List of materials and their corresponding
    /// </summary>
    [ViewVariables]
    [DataField("results", required: true)]
    public Dictionary<string, string> Results = new();


    //public float ElapsedTime;
    //public int TimeThreshold = 5;
}
