using System.Threading;
using Content.Shared.Tools;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Tools.Components
{
    [RegisterComponent]
    public sealed class DiggingComponent : Component
    {
        [ViewVariables]
        [DataField("toolComponentNeeded")]
        public bool ToolComponentNeeded = true;

        [ViewVariables]
        [DataField("qualityNeeded", customTypeSerializer:typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
        public string QualityNeeded = "Digging";

        [ViewVariables]
        [DataField("delay")]
        public float Delay = 1f;

        /// <summary>
        /// Used for do_afters.
        /// </summary>
        public CancellationTokenSource? CancelToken = null;
    }
}
