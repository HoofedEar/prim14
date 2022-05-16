using System.Linq;
using Content.Shared.Interaction;
using Content.Shared.Sound;
using Content.Shared.Throwing;
using Robust.Server.Containers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.Player;

namespace Content.Server.Anprim14.ReleaseFish;

public sealed class ReleaseFishSystem : EntitySystem
{
    [Dependency] private readonly ContainerSystem _containerSystem = default!;
    private readonly SoundSpecifier _waterSplash = new SoundPathSpecifier("/Audio/Anprim14/Effects/watersplash.ogg");
    
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ReleaseFishComponent, ComponentInit>(HandleReleaseFishInit);
        SubscribeLocalEvent<ReleaseFishComponent, ThrowHitByEvent>(HandleThrowCollide);
        SubscribeLocalEvent<ReleaseFishComponent, InteractUsingEvent>(OnInteractUsing);
        
    }
    
    private void HandleReleaseFishInit(EntityUid uid, ReleaseFishComponent component, ComponentInit args)
    {
        component.Container = _containerSystem.EnsureContainer<Container>(component.Owner, "ocean_water", out _);
    }

    private void HandleThrowCollide(EntityUid uid, ReleaseFishComponent component, ThrowHitByEvent args)
    {
        if (!component.Container.CanInsert(args.Thrown) ||
            !component.Container.Insert(args.Thrown))
            return;
        
        SoundSystem.Play(Filter.Pvs(args.Thrown), _waterSplash.GetSound(), args.Thrown);
        //QueueDel(args.Thrown);
    }

    private void OnInteractUsing(EntityUid uid, ReleaseFishComponent component, InteractUsingEvent args)
    {
        if (!component.Container.ContainedEntities.Any()) return;
        // check if you're using a net
        TryEjectContents(component);
    }

    /// <summary>
    /// Remove all entities currently in the body of water.
    /// </summary>
    private void TryEjectContents(ReleaseFishComponent component)
    {
        var contained = component.Container.ContainedEntities.ToArray();
        foreach (var entity in contained)
        {
            Remove(component, entity);
        }
    }

    private void Remove(ReleaseFishComponent component, EntityUid entity)
    {
        component.Container.Remove(entity);
    }
}