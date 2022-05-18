using Content.Shared.Containers.ItemSlots;

namespace Content.Server.Anprim14.Blacksmithing.Components;

[RegisterComponent, Friend(typeof(BlacksmithJugSystem))]
public sealed class BlacksmithJugComponent : Component
{
    public const string JugMaterialSlotId = "Jug-material";

    [DataField("materialSlot", required: true)] 
    public ItemSlot MaterialSlot = new();
}