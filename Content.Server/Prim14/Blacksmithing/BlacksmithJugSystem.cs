using Content.Server.Prim14.Blacksmithing.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;

namespace Content.Server.Prim14.Blacksmithing;

public sealed class BlacksmithJugSystem : EntitySystem
{
    [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BlacksmithJugComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<BlacksmithJugComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<BlacksmithJugComponent, ExaminedEvent>(OnExamined);
    }

    private void OnComponentInit(EntityUid uid, BlacksmithJugComponent jug, ComponentInit args)
    {
        _itemSlotsSystem.AddItemSlot(uid, BlacksmithJugComponent.JugMaterialSlotId, jug.MaterialSlot);
    }

    private void OnComponentRemove(EntityUid uid, BlacksmithJugComponent jug, ComponentRemove args)
    {
        _itemSlotsSystem.RemoveItemSlot(uid, jug.MaterialSlot);
    }

    private void OnExamined(EntityUid uid, BlacksmithJugComponent component, ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        if (component.MaterialSlot.Item == null) return;
        var name = Name(component.MaterialSlot.Item.Value);
        args.Message.AddText("\nThis contains " + name +".");
    }
}