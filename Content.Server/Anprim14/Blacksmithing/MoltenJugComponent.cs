using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Anprim14.Blacksmithing;

[RegisterComponent, Friend(typeof(MoltenJugSystem))]
public sealed class MoltenJugComponent : Component
{
    [ViewVariables]
    [DataField("empty", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string? EmptyPrototype { get; set; }
    
}