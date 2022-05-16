using Content.Shared.Anprim14;
using Content.Shared.Interaction;
using JetBrains.Annotations;
using Robust.Shared.Random;

namespace Content.Server.Anprim14.Bush;

public sealed class BushSystem : EntitySystem
{ 
    [Dependency] private readonly IRobustRandom _random = null!;
    public override void Initialize()
    {
        base.Initialize();
        
        SubscribeLocalEvent<BushComponent, InteractHandEvent>(OnInteractHand);
        SubscribeLocalEvent<BushComponent, BushReadyEvent>(OnBushReady);
    }

    public void OnInteractHand(EntityUid uid, BushComponent component, InteractHandEvent args)
    {
        if (!component.Ready) return;
        var playerPos = Transform(args.Target).MapPosition;
        EntityManager.SpawnEntity(component.Loot, playerPos);
        UpdateAppearance(uid, true, false);
        component.Ready = false;
    }

    public override void Update(float frameTime)
    {
        foreach (var component in EntityQuery<BushComponent>())
        {
            if (component.Ready) continue;
            if (component.Accumulator < component.RespawnTime)
            {
                component.Accumulator += frameTime;
                continue;
            }
            component.Accumulator = 0;
            var ev = new BushReadyEvent(component);
            RaiseLocalEvent(component.Owner, ev, false);
        }
    }
    
    private void UpdateAppearance(EntityUid uid, bool isEmpty, bool isReady)
    {
        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;

        appearance.SetData(BushVisuals.Empty, isEmpty);
        appearance.SetData(BushVisuals.Ready, isReady);
    }
    
    private void OnBushReady(EntityUid uid, BushComponent component, BushReadyEvent args)
    {
        component.Ready = true;
        UpdateAppearance(uid, false, true);
    }
    
    private sealed class BushReadyEvent : EntityEventArgs
    {
        private BushComponent Bush {[UsedImplicitly] get;}
        public BushReadyEvent(BushComponent bush)
        {
            Bush = bush;
        }
    }
}