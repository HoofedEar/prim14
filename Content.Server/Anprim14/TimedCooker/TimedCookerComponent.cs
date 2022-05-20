using Content.Server.Anprim14.Blacksmithing;
using Content.Shared.Sound;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;

namespace Content.Server.Anprim14.TimedCooker;

[RegisterComponent]
[Friend(typeof(TimedCookerSystem), typeof(KilnSystem))]
public class TimedCookerComponent : Component
{
    /// <summary>
    /// Container of entities inside to be processed.
    /// </summary>
    [ViewVariables] 
    public Container Container = default!;
    
    /// <summary>
    /// Whitelist for specifying the kind of stuff can be cooked/processed
    /// </summary>
    [ViewVariables]
    [DataField("whitelist", required: true)] 
    public EntityWhitelist? Whitelist;
    
    /// <summary>
    /// Maximum number of items that can be queued
    /// </summary>
    [ViewVariables]
    [DataField("max")] 
    public int? Max = 2;
    
    /// <summary>
    /// The sound that plays when finished producing the result
    /// </summary>
    [DataField("producingSound")]
    public SoundSpecifier? ProducingSound;
        
    /// <summary>
    /// The sound that plays when inserting an item
    /// </summary>
    [DataField("insertingSound")]
    public SoundSpecifier? InsertingSound;
    
    /// <summary>
    /// The recipe that is currently producing
    /// </summary>
    [ViewVariables]
    public TimedCookerRecipePrototype? ProducingRecipe;
    
    /// <summary>
    /// Production accumulator for the production time.
    /// </summary>
    [ViewVariables]
    [DataField("producingAccumulator")]
    public float ProducingAccumulator;
    
    /// <summary>
    /// The cooker's construction queue
    /// </summary>
    [ViewVariables]
    public Queue<TimedCookerRecipePrototype> Queue { get; } = new();

    /// <summary>
    /// Is the cooker currently turned on?
    /// </summary>
    public bool IsRunning;
}