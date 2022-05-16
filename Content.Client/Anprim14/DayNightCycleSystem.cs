using Content.Shared.Administration;
using Content.Shared.Anprim14;
using Robust.Client.Graphics;

namespace Content.Client.Anprim14;

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
        switch (state)
        {
            case DayNightCycleStates.Midnight:
                _lightManager.AmbientLightColor = Color.FromHex("#030303");
                break;
            case DayNightCycleStates.Dawn:
                _lightManager.AmbientLightColor = Color.FromHex("#1c1c1c");
                break;
            case DayNightCycleStates.Morning:
                _lightManager.AmbientLightColor = Color.FromHex("#373737");
                break;
            case DayNightCycleStates.Noon:
                _lightManager.AmbientLightColor = Color.FromHex("#a0a0a0");
                break;
            case DayNightCycleStates.Afternoon:
                _lightManager.AmbientLightColor = Color.FromHex("#1c1c1c");
                break;
            case DayNightCycleStates.Dusk:
                _lightManager.AmbientLightColor = Color.FromHex("#1c1c1c");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    /*
     * Potential dusk/night colors: "#1c1c1c" 
     */

}