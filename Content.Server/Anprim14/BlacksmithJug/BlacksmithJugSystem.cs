using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Robust.Shared.Containers;

namespace Content.Server.Anprim14.BlacksmithJug;

public sealed class BlacksmithJugSystem : EntitySystem
{
    [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BlacksmithJugComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<BlacksmithJugComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<BlacksmithJugComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<BlacksmithJugComponent, EntInsertedIntoContainerMessage>(OnItemInserted);
    }

    private void OnComponentInit(EntityUid uid, BlacksmithJugComponent jug, ComponentInit args)
    {
        _itemSlotsSystem.AddItemSlot(uid, BlacksmithJugComponent.JugOreSlotId, jug.OreSlot);
        _itemSlotsSystem.AddItemSlot(uid, BlacksmithJugComponent.JugIngotSlotId, jug.IngotSlot);
    }

    private void OnComponentRemove(EntityUid uid, BlacksmithJugComponent jug, ComponentRemove args)
    {
        _itemSlotsSystem.RemoveItemSlot(uid, jug.OreSlot);
        _itemSlotsSystem.RemoveItemSlot(uid, jug.IngotSlot);
    }

    private void OnExamined(EntityUid uid, BlacksmithJugComponent component, ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;
        
        if (component.OreSlot.HasItem)
            args.Message.AddText("\nThis contains ore.");
        if (component.IngotSlot.HasItem)
            args.Message.AddText("\nThis contains ingot.");
    }

    private void OnItemInserted(EntityUid uid, BlacksmithJugComponent jug, EntInsertedIntoContainerMessage args)
    {
        // Make sure you can't have both ore and ingot inside of it
        if (args.Container.ID == jug.OreSlot.ID && jug.IngotSlot.HasItem)
            return;
        
        if (args.Container.ID == jug.IngotSlot.ID && jug.OreSlot.HasItem)
            return;
    }
}