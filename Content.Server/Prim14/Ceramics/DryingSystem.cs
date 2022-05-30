using Content.Server.Prim14.Ceramics;
using JetBrains.Annotations;
using Robust.Shared.Containers;

public sealed class DryingSystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _containerSystem = default!;
    public override void Initialize()
    {
        base.Initialize();
        
        SubscribeLocalEvent<DryingComponent, DryingDoneEvent>(OnDryingDone);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        foreach (var component in EntityQuery<DryingComponent>())
        {
            if (_containerSystem.IsEntityInContainer(component.Owner))
                continue;
            
            if (component.Accumulator < component.DryingTime)
            {
                component.Accumulator += frameTime;
                continue;
            }
            
            component.Accumulator = 0;
            var ev = new DryingDoneEvent(component);
            RaiseLocalEvent(component.Owner, ev, false);
        }
    }

    private void OnDryingDone(EntityUid uid, DryingComponent component, DryingDoneEvent args)
    {
        EntityManager.SpawnEntity(component.Result, Comp<TransformComponent>(component.Owner).Coordinates);
        QueueDel(uid);
    }
    
    private sealed class DryingDoneEvent : EntityEventArgs
    {
        private DryingComponent Dry {[UsedImplicitly] get;}
        public DryingDoneEvent(DryingComponent dry)
        {
            Dry = dry;
        }
    }
}