using Content.Shared.Prim14;
using Robust.Client.Graphics;

namespace Content.Client.Prim14;

public sealed class DayNightCycleSystem : SharedDayNightCycleSystem
{
    [Dependency] private readonly ILightManager _lightManager = IoCManager.Resolve<ILightManager>();
    
    protected override void OnDayNightCycleMessage(DayNightCycleMessage message, EntitySessionEventArgs eventArgs)
    {
        base.OnDayNightCycleMessage(message, eventArgs);

        // ReSharper disable once SuspiciousTypeConversion.Global
        SetAmbientLight(message.State);
    }

    private void SetAmbientLight(DayNightCycleStates state)
    {
        _lightManager.AmbientLightColor = state switch
        {
            DayNightCycleStates.Midnight => Color.FromHex("#030303"),
            DayNightCycleStates.Dawn => Color.FromHex("#1c1c1c"),
            DayNightCycleStates.Morning => Color.FromHex("#373737"),
            DayNightCycleStates.Noon => Color.FromHex("#a0a0a0"),
            DayNightCycleStates.Afternoon => Color.FromHex("#1c1c1c"),
            DayNightCycleStates.Dusk => Color.FromHex("#1c1c1c"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    /*
     * Potential dusk/night colors: "#1c1c1c" 
     */

}