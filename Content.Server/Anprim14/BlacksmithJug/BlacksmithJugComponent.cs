using Content.Shared.Containers.ItemSlots;

namespace Content.Server.Anprim14.BlacksmithJug;

[RegisterComponent]
public sealed class BlacksmithJugComponent : Component
{
    public const string JugOreSlotId = "Jug-ore";
    public const string JugIngotSlotId = "Jug-ingot";

    [DataField("oreSlot")] 
    public ItemSlot OreSlot = new();

    [DataField("ingotSlot")]
    public ItemSlot IngotSlot = new();
}