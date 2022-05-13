using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Anprim14.TimedCooker;

[Serializable, Prototype("timedCookerRecipe")]
public sealed class TimedCookerRecipePrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("input", customTypeSerializer:typeof(PrototypeIdSerializer<EntityPrototype>))]
    private string _input = string.Empty;
    
    [DataField("result")]
    private List<string> _result = new();
    
    [DataField("completeTime")]
    private TimeSpan _completeTime = TimeSpan.FromSeconds(5);

    /// <summary>
    ///     The prototype name of the necessary entity when the recipe is produced.
    /// </summary>
    [ViewVariables]
    public string Input => _input;
    
    /// <summary>
    ///     The prototype name of the resulting entity when the recipe is printed.
    /// </summary>
    [ViewVariables]
    public List<string> Result => _result;
    
    /// <summary>
    ///     How many milliseconds it'll take for the lathe to finish this recipe.
    ///     Might lower depending on the lathe's upgrade level.
    /// </summary>
    [ViewVariables]
    public TimeSpan CompleteTime => _completeTime;
    
}