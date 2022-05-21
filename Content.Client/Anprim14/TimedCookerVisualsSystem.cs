using Content.Shared.Anprim14;
using Robust.Client.GameObjects;

namespace Content.Client.Anprim14;

public sealed class TimedCookerVisualsSystem : VisualizerSystem<TimedCookerVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, TimedCookerVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (!TryComp(uid, out SpriteComponent? sprite) ||
            !args.Component.TryGetData(TimedCookerState.Fired, out bool isFired)) return;
        sprite.LayerSetVisible(TimedCookerVisualLayers.Fired, isFired);
    }
}

public enum TimedCookerVisualLayers : byte
{
    Fired
}