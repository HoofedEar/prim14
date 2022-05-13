using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Tag;

namespace Content.Server.Anprim14.Blacksmithing;

public sealed class BlacksmithJugSystem : EntitySystem
{
    [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
    [Dependency] private readonly TagSystem _tagSystem = default!;

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
        
        if (component.MaterialSlot.Item != null && _tagSystem.HasTag(component.MaterialSlot.Item.Value, "Ingot"))
            args.Message.AddText("\nThis contains an ingot.");
        if (component.MaterialSlot.Item != null && _tagSystem.HasTag(component.MaterialSlot.Item.Value, "Ore"))
            args.Message.AddText("\nThis contains ore.");
    }
}