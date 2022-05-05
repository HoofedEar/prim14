using Content.Shared.Sound;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Player;

namespace Content.Server.Anprim14.ReleaseFish;

public sealed class ReleaseFishSystem : EntitySystem
{
    public readonly SoundSpecifier WaterSplash = new SoundPathSpecifier("/Audio/Anprim14/Effects/watersplash.ogg");
    public override void Initialize()
    {
        base.Initialize();
        
        SubscribeLocalEvent<ReleaseFishComponent, ThrowHitByEvent>(HandleThrowCollide);
    }

    private void HandleThrowCollide(EntityUid uid, ReleaseFishComponent component, ThrowHitByEvent args)
    {
        SoundSystem.Play(Filter.Pvs(args.Thrown), WaterSplash.GetSound(), args.Thrown);
        QueueDel(args.Thrown);
    }
}