using System.Linq;
using Content.Server.Anprim14.Blacksmithing.Components;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Shared.Anprim14;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Tag;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Server.Anprim14.Blacksmithing;

public sealed class AnvilSystem : EntitySystem
{
    [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AnvilComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<AnvilComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<AnvilComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<AnvilComponent, EntInsertedIntoContainerMessage>(OnContainerModified);
        SubscribeLocalEvent<AnvilComponent, EntRemovedFromContainerMessage>(OnContainerModified);
        SubscribeLocalEvent<AnvilComponent, InteractUsingEvent>(OnInteractUsing);
    }

    private void OnInteractUsing(EntityUid uid, AnvilComponent anvil, InteractUsingEvent args)
    {
        if (!TryComp(args.Used, out TagComponent? tagComp))
        {
            return;
        }

        if (!tagComp.Tags.Contains("AnvilHammer"))
        {
            return;
        }

        var anvilPos = Transform(args.Target).MapPosition;
        if (anvil.MoldSlot.Item == null) return;
        if (TryPrototype(anvil.MoldSlot.Item.Value, out var mold))
        {
            foreach (var (oldMold, result) in anvil.Results)
            {
                if (mold?.ID != oldMold) break;
                EntityManager.SpawnEntity(oldMold, anvilPos);
                EntityManager.SpawnEntity(result, anvilPos);
            }
        }
        QueueDel(anvil.MoldSlot.Item.Value);
    }

    private void OnContainerModified(EntityUid uid, AnvilComponent anvil, ContainerModifiedMessage args)
    {
        if (!anvil.Initialized) return;

        if (args.Container.ID != anvil.MoldSlot.ID)
            return;

        var hasMold = args.Container.ContainedEntities.Any();

        if (hasMold == false)
        {
            UpdateAppearance(uid, hasMold);
            return;
        }
        
        if (!TryComp(anvil.MoldSlot.Item, out SolutionContainerManagerComponent? solutionComp))
        {
            return;
        }
        
        foreach (var (name, solution) in solutionComp.Solutions)
        {
            if (name != "metal")
                return;

            if (solution.CurrentVolume != 20) break;
            UpdateAppearance(uid, hasMold);
            break;
        }
    }

    private void OnComponentInit(EntityUid uid, AnvilComponent anvil, ComponentInit args)
    {
        _itemSlotsSystem.AddItemSlot(uid, AnvilComponent.AnvilMoldSlotId, anvil.MoldSlot);
        UpdateAppearance(uid, false);
    }

    private void OnComponentRemove(EntityUid uid, AnvilComponent anvil, ComponentRemove args)
    {
        _itemSlotsSystem.RemoveItemSlot(uid, anvil.MoldSlot);
    }

    private void OnExamined(EntityUid uid, AnvilComponent component, ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        if (!TryComp(component.MoldSlot.Item, out SolutionContainerManagerComponent? solutionComp))
        {
            return;
        }

        if (solutionComp.Solutions.Count == 0)
        {
            return;
        }

        foreach (var (name, solution) in solutionComp.Solutions)
        {
            if (name != "metal")
                return;

            if (solution.CurrentVolume != 20)
            {
                args.Message.AddText("\nIt has an empty mold.");
                break;
            }

            args.Message.AddText("\nIt has a mold ready to smith.");
            break;
        }
    }
    private void UpdateAppearance(EntityUid uid, bool isReady)
    {
        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;
        
        appearance.SetData(AnvilState.Ready, isReady);
    }
}
