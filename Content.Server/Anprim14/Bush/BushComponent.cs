using Content.Shared.Sound;

namespace Content.Server.Anprim14.Bush;

[RegisterComponent]
[Friend(typeof(BushSystem))]
public sealed class BushComponent : Component
{
    /// <summary>
    /// Entity that is spawned when a player picks from the bush
    /// </summary>
    [ViewVariables]
    [DataField("loot")] 
    public string? Loot;
    
    /// <summary>
    /// How many should be spawned?
    /// </summary>
    [ViewVariables]
    [DataField("quantity")] 
    public int? Quantity = 1;
    
    /// <summary>
    /// Time it takes to respawn the loot
    /// </summary>
    [ViewVariables]
    [DataField("respawnTime")]
    public float? RespawnTime = 5f;
    
    /// <summary>
    /// The sound played when interacting with the bush
    /// </summary>
    [ViewVariables]
    [DataField("interactSound")]
    public SoundSpecifier? InteractSound;
    
    public float Accumulator;
    public bool Ready = true;
}