using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;

namespace Content.Server.Anprim14.Blacksmithing;

public sealed class MoltenJugSystem : EntitySystem
{
    [Dependency] private readonly SharedHandsSystem _handsSystem = default!;
    [Dependency] private readonly SolutionContainerSystem _solutionContainerSystem = default!;


    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MoltenJugComponent, AfterInteractEvent>(OnAfterInteract);
    }

    private void OnAfterInteract(EntityUid uid, MoltenJugComponent component, AfterInteractEvent args)
    {
        DeleteAndSpawnJug(component.Owner, args.User, "b");
    }

    private void DeleteAndSpawnJug(EntityUid owner, EntityUid user, string emptyprototype)
    {
        //We're empty. Become trash.
        var position = EntityManager.GetComponent<TransformComponent>(owner).Coordinates;
        var finisher = EntityManager.SpawnEntity(emptyprototype, position);

        // If the user is holding the item
        if (_handsSystem.IsHolding(user, owner, out var hand))
        {
            EntityManager.DeleteEntity(owner);

            // Put the trash in the user's hand
            _handsSystem.TryPickup(user, finisher, hand);
            return;
        }

        EntityManager.QueueDeleteEntity(owner);
    }

}
