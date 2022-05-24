using Content.Shared.Prim14;
using Robust.Client.GameObjects;

namespace Content.Client.Prim14;

public sealed class KilnVisualsSystem : VisualizerSystem<KilnVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, KilnVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (!TryComp(uid, out SpriteComponent? sprite) ||
            !args.Component.TryGetData(KilnState.Fired, out bool isFired)) return;
        sprite.LayerSetVisible(KilnVisualLayers.Fired, isFired);
    }
}

public enum KilnVisualLayers : byte
{
    Fired
}
