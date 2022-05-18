using Robust.Client.GameObjects;
using Content.Shared.Anprim14;


namespace Content.Client.Anprim14;

public sealed class AnvilVisualsSystem : VisualizerSystem<AnvilVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, AnvilVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (!TryComp(uid, out SpriteComponent? sprite) ||
            !args.Component.TryGetData(AnvilState.Ready, out bool isOn) ||
            !args.Component.TryGetData(AnvilState.Empty, out bool isRunning)) return;
        var state = isRunning ? component.StateReady : component.StateEmpty;
        sprite.LayerSetVisible(BushVisualLayers.Ready, isOn);
        sprite.LayerSetState(BushVisualLayers.Empty, state);
    }
}
