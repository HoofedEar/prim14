using Content.Shared.Containers.ItemSlots;
using Content.Shared.Sound;

namespace Content.Server.Anprim14.Blacksmithing.Components;

[RegisterComponent, Friend(typeof(AnvilSystem))]
public sealed class AnvilComponent : Component
{
    public const string AnvilMoldSlotId = "Anvil-mold";
    
    [ViewVariables]
    [DataField("moldSlot", required: true)] 
    public ItemSlot MoldSlot = new();
    
    /// <summary>
    /// The sound played when hammering the anvil
    /// </summary>
    [ViewVariables]
    [DataField("interactSound")]
    public SoundSpecifier? InteractSound;
    
    public List<string> Results = new()
    {
        "PickaxeHeadCrude"
    };
}