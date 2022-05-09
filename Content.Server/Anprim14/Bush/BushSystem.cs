using Content.Shared.Anprim14;
using Content.Shared.Interaction;
using JetBrains.Annotations;

namespace Content.Server.Anprim14.Bush;

public sealed class BushSystem : EntitySystem
{ 
    public override void Initialize()
    {
        base.Initialize();
        
        SubscribeLocalEvent<BushComponent, InteractHandEvent>(OnInteractHand);
        SubscribeLocalEvent<BushComponent, BushReadyEvent>(OnBushReady);
    }

    public void OnInteractHand(EntityUid uid, BushComponent component, InteractHandEvent args)
    {
        if (!component.Ready) return;
        UpdateAppearance(uid, false, true);
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
    
    private void UpdateAppearance(EntityUid uid, bool isReady, bool isEmpty)
    {
        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;

        appearance.SetData(BushVisuals.Ready, isReady);
        appearance.SetData(BushVisuals.Empty, isEmpty);
    }
    
    private void OnBushReady(EntityUid uid, BushComponent component, BushReadyEvent args)
    {
        component.Ready = true;
        UpdateAppearance(uid, true, false);
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