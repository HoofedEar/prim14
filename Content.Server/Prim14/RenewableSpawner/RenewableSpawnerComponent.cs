using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server.Prim14.RenewableSpawner
{
    [RegisterComponent]
    public sealed class RenewableSpawnerComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("prototypes", customTypeSerializer: typeof(PrototypeIdListSerializer<EntityPrototype>))]
        public List<string> Prototypes { get; set; } = new();

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("chance")]
        public float Chance { get; set; } = 1.0f;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("intervalSeconds")]
        public int IntervalSeconds { get; set; } = 60;

        public float ElapsedTime;
    }
}
