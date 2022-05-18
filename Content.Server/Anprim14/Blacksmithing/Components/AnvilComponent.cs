using Content.Shared.Containers.ItemSlots;

namespace Content.Server.Anprim14.Blacksmithing.Components;

[RegisterComponent, Friend(typeof(AnvilSystem))]
public sealed class AnvilComponent : Component
{
    public const string AnvilMoldSlotId = "Anvil-mold";
    
    [DataField("moldSlot", required: true)] 
    public ItemSlot MoldSlot = new();
}