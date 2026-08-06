using System.Collections.Generic;

public sealed class Simulation {
    private readonly List<ISystem> _systems = new List<ISystem>();

    public Frame Frame { get; }
    public ConfigDatabase ConfigDatabase { get; }
    public CommandQueue CommandQueue { get; }
    public float TickDeltaTime { get; }

    public readonly Queue<SpawnUnitRequest> SpawnRequests = new Queue<SpawnUnitRequest>();
    public readonly Queue<BuildTurretRequest> BuildRequests = new Queue<BuildTurretRequest>();
    public readonly Queue<UpgradeBaseRequest> UpgradeRequests = new Queue<UpgradeBaseRequest>();
    public readonly Queue<DamageRequest> DamageRequests = new Queue<DamageRequest>();
    public readonly Queue<int> ProjectileRemovalRequests = new Queue<int>();

    public Simulation(ConfigDatabase configDatabase, int tickRate) {
        Frame = new Frame();
        ConfigDatabase = configDatabase;
        CommandQueue = new CommandQueue();
        TickDeltaTime = 1f / tickRate;

        _systems.Add(new EconomySystem());
        _systems.Add(new BuildSystem());
        _systems.Add(new SpawnSystem());
        _systems.Add(new BaseUpgradeSystem());
        _systems.Add(new MovementSystem());
        _systems.Add(new TargetSystem());
        _systems.Add(new WeaponSystem());
        _systems.Add(new ProjectileSystem());
        _systems.Add(new DamageSystem());
        _systems.Add(new DeathSystem());
        _systems.Add(new AISystem());

        for (var i = 0; i < _systems.Count; i++) {
            _systems[i].Init(this);
        }
    }

    public void EnqueueCommand(ICommand command) {
        CommandQueue.Enqueue(command);
    }

    public void Tick() {
        CommandQueue.ExecuteAll(this);

        for (var i = 0; i < _systems.Count; i++) {
            _systems[i].Run(this);
        }

        Frame.Tick++;
    }
}