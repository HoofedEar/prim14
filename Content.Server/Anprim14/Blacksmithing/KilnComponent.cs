using Content.Server.Anprim14.TimedCooker;

namespace Content.Server.Anprim14.Blacksmithing;

[RegisterComponent]
[Friend(typeof(TimedCookerSystem), typeof(KilnSystem))]
public sealed class KilnComponent : TimedCookerComponent
{
    public int? KilnMax = 1;
}