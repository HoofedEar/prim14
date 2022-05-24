using System.Threading;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server.Prim14.RenewableSpawner
{
    [RegisterComponent]
    public sealed class RenewableSpawnerComponent : Component, ISerializationHooks
    {
        [Dependency] private readonly IRobustRandom _robustRandom = default!;
        [Dependency] private readonly IEntityManager _entMan = default!;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("prototypes", customTypeSerializer: typeof(PrototypeIdListSerializer<EntityPrototype>))]
        public List<string> Prototypes { get; set; } = new();

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("chance")]
        public float Chance { get; set; } = 1.0f;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("intervalSeconds")]
        public int IntervalSeconds { get; set; } = 60;

        //private EntityUid _recentlySpawned;

        /*
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("MinimumEntitiesSpawned")]
        public int MinimumEntitiesSpawned { get; set; } = 1;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("MaximumEntitiesSpawned")]
        public int MaximumEntitiesSpawned { get; set; } = 1;
        */

        private CancellationTokenSource? _tokenSource;

        void ISerializationHooks.AfterDeserialization()
        {
            //if (MinimumEntitiesSpawned > MaximumEntitiesSpawned)
            //    throw new ArgumentException("MaximumEntitiesSpawned can't be lower than MinimumEntitiesSpawned!");
        }

        protected override void Initialize()
        {
            base.Initialize();
            SetupTimer();
        }

        protected override void Shutdown()
        {
            base.Shutdown();
            _tokenSource?.Cancel();
        }

        private void SetupTimer()
        {
            _tokenSource?.Cancel();
            _tokenSource = new CancellationTokenSource();
            Owner.SpawnRepeatingTimer(TimeSpan.FromSeconds(IntervalSeconds), OnTimerFired, _tokenSource.Token);
        }

        private void OnTimerFired()
        {
            if (!_robustRandom.Prob(Chance))
                return;

            // Check if I am touching the thing I spawned. If I am, return.
            //if (_entMan.EntityExists(_recentlySpawned))
            //    return;

            var number = 1;

            for (int i = 0; i < number; i++)
            {
                var entity = _robustRandom.Pick(Prototypes);
                IoCManager.Resolve<IEntityManager>().SpawnEntity(entity, IoCManager.Resolve<IEntityManager>().GetComponent<TransformComponent>(Owner).Coordinates);
                _entMan.DeleteEntity(Owner);
            }
        }
    }
}
