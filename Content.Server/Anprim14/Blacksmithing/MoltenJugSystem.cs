using Content.Server.Chemistry.Components;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;

namespace Content.Server.Anprim14.Blacksmithing;

public sealed class MoltenJugSystem : EntitySystem
{
    [Dependency] private readonly SharedHandsSystem _handsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MoltenJugComponent, AfterInteractEvent>(OnAfterInteract);
    }

    private void OnAfterInteract(EntityUid uid, MoltenJugComponent component, AfterInteractEvent args)
    {
        DeleteAndSpawnTrash(component, args.User);
    }

    private void DeleteAndSpawnTrash(MoltenJugComponent component, EntityUid? user = null)
    {
        if (!TryComp<SolutionContainerManagerComponent>(user, out var solCont))
            return;

        foreach (var (_, solution) in solCont.Solutions)
        {
            if (solution.CurrentVolume != 0)
                return;
        }

        //We're empty. Become trash.
        var position = EntityManager.GetComponent<TransformComponent>(component.Owner).Coordinates;
        var finisher = EntityManager.SpawnEntity(component.EmptyPrototype, position);

        // If the user is holding the item
        if (_handsSystem.IsHolding(user.Value, component.Owner, out var hand))
        {
            EntityManager.DeleteEntity((component).Owner);

            // Put the trash in the user's hand
            _handsSystem.TryPickup(user.Value, finisher, hand);
            return;
        }

        EntityManager.QueueDeleteEntity(component.Owner);
    }
}