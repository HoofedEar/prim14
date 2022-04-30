using System.Threading;
using Content.Shared.Damage;
using Content.Shared.Sound;

namespace Content.Server.Gathering.Components
{
    /// <summary>
    ///     When interacting with an <see cref="GatherableComponent"/> allows it to spawn entities.
    /// </summary>
    [RegisterComponent]
    public sealed class GatheringToolComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("sound")]
        public SoundSpecifier GatheringSound { get; set; } = new SoundPathSpecifier("/Audio/Items/Mining/pickaxe.ogg");

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("timeMultiplier")]
        public float GatheringTimeMultiplier { get; set; } = 1f;

        /// <summary>
        ///     What damage should be given to objects when
        ///     gathered using a tool?
        /// </summary>
        [DataField("damage", required: true)]
        public DamageSpecifier Damage { get; set; } = default!;

        /// <summary>
        ///     How many entities can this tool gather at once?
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("maxEntities")]
        public int MaxGatheringEntities = 1;

        [ViewVariables]
        public readonly Dictionary<EntityUid, CancellationTokenSource> GatheringEntities = new();
    }
}
