using Content.Server.Anprim14.TimedCooker;

namespace Content.Server.Anprim14.Blacksmithing.Components;

[RegisterComponent]
[Friend(typeof(TimedCookerSystem), typeof(KilnSystem))]
public sealed class KilnComponent : TimedCookerComponent
{
    // This limits how many jugs it can process at once.
    public int KilnMax = 1;

    // This is how we track how much wood has been insert into it.
    [ViewVariables]
    public int KilnWoodStorage = 0;

    
    public float ElapsedTime;
    public int TimeThreshold = 10;
}