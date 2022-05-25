using Content.Shared.Interaction;

namespace Content.Server.Prim14.Blocker;

public sealed class BlockerSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BlockerComponent, InteractHandEvent>(OnInteractHand);
        SubscribeLocalEvent<BlockerComponent, InteractUsingEvent>(OnInteractUsing);
    }

    public void OnInteractHand(EntityUid uid, BlockerComponent component, InteractHandEvent args)
    { }

    public void OnInteractUsing(EntityUid uid, BlockerComponent component, InteractUsingEvent args)
    { }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        foreach (var component in EntityQuery<BlockerComponent>())
        {
            if (component.Accumulator < component.Timer)
            {
                component.Accumulator += frameTime;
                continue;
            }

            QueueDel(component.Owner);
        }
    }
}
