using Content.Shared.Containers.ItemSlots;

namespace Content.Server.Anprim14.BlacksmithJug;

public sealed class BlacksmithJugSystem : EntitySystem
{
    [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        
        SubscribeLocalEvent<BlacksmithJugComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<BlacksmithJugComponent, ComponentRemove>(OnComponentRemove);
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
}