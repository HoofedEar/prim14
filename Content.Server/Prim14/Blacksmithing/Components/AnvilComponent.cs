using Content.Shared.Containers.ItemSlots;
using Content.Shared.Sound;

namespace Content.Server.Prim14.Blacksmithing.Components;

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

    /// <summary>
    /// Tag that the anvil needs to process the inside mold
    /// </summary>
    [ViewVariables] 
    [DataField("tag", required: true)] 
    public string Tag = string.Empty;

    /// <summary>
    /// List of IDs for molds and their result
    /// </summary>
    [ViewVariables]
    [DataField("results", required: true)]
    public Dictionary<string, string> Results = new();
}