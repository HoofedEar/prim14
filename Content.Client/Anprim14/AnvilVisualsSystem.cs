using Robust.Client.GameObjects;
using Content.Shared.Anprim14;


namespace Content.Client.Anprim14;

public sealed class AnvilVisualsSystem : VisualizerSystem<AnvilVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, AnvilVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (!TryComp(uid, out SpriteComponent? sprite) ||
            !args.Component.TryGetData(AnvilState.Ready, out bool isOn)) return;
        sprite.LayerSetVisible(AnvilVisualLayers.Ready, isOn);
    }
}

public enum AnvilVisualLayers : byte
{
    Ready
}
