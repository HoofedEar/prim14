using Content.Shared.Prim14;
using Robust.Client.GameObjects;

namespace Content.Client.Prim14;

public sealed class BushVisualsSystem : VisualizerSystem<BushVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, BushVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (!TryComp(uid, out SpriteComponent? sprite) ||
            !args.Component.TryGetData(BushVisuals.Ready, out bool isOn) ||
            !args.Component.TryGetData(BushVisuals.Empty, out bool isRunning)) return;
        var state = isRunning ? component.StateReady : component.StateEmpty;
        sprite.LayerSetVisible(BushVisualLayers.Ready, isOn);
        sprite.LayerSetState(BushVisualLayers.Empty, state);
    }
}

public enum BushVisualLayers : byte
{
    Ready,
    Empty
}