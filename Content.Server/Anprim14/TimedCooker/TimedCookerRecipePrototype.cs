using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Anprim14.TimedCooker;

[Serializable, Prototype("timedCookerRecipe")]
public sealed class TimedCookerRecipePrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("name")]
    private string _name = string.Empty;
    
    [DataField("input", customTypeSerializer:typeof(PrototypeIdSerializer<EntityPrototype>))]
    private string _input = string.Empty;
    
    [DataField("result", customTypeSerializer:typeof(PrototypeIdSerializer<EntityPrototype>))]
    private string _result = string.Empty;
    
    [DataField("completetime")]
    private TimeSpan _completeTime = TimeSpan.FromSeconds(5);
    
    /// <summary>
    ///     Name displayed in the GUI. ?
    /// </summary>
    [ViewVariables]
    public string Name
    {
        get
        {
            if (_name.Trim().Length != 0) return _name;
            var protoMan = IoCManager.Resolve<IPrototypeManager>();
            protoMan.TryIndex(_result, out EntityPrototype? prototype);
            if (prototype?.Name != null)
                _name = prototype.Name;
            return _name;
        }
    }
    
    /// <summary>
    ///     The prototype name of the necessary entity when the recipe is produced.
    /// </summary>
    [ViewVariables]
    public string Input => _input;
    
    /// <summary>
    ///     The prototype name of the resulting entity when the recipe is printed.
    /// </summary>
    [ViewVariables]
    public string Result => _result;
    
    /// <summary>
    ///     How many milliseconds it'll take for the lathe to finish this recipe.
    ///     Might lower depending on the lathe's upgrade level.
    /// </summary>
    [ViewVariables]
    public TimeSpan CompleteTime => _completeTime;
    
}