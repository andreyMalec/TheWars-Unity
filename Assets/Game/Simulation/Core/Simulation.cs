using System.Collections.Generic;

public sealed class Simulation {
    private readonly List<ISystem> _systems = new List<ISystem>();

    public Frame Frame { get; }
    public CommandQueue CommandQueue { get; }
    public SpawnQueue SpawnQueue { get; }
    public readonly Queue<BuildTurretRequest> BuildRequests = new Queue<BuildTurretRequest>();
    public readonly Queue<UpgradeBaseRequest> UpgradeRequests = new Queue<UpgradeBaseRequest>();
    public readonly Queue<DamageRequest> DamageRequests = new Queue<DamageRequest>();
    public readonly Queue<int> ProjectileRemovalRequests = new Queue<int>();

    public Simulation(ConfigDatabase configDatabase, int tickRate) {
        Frame = new Frame(1f / tickRate, configDatabase);
        CommandQueue = new CommandQueue();
        SpawnQueue = new SpawnQueue();

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
            _systems[i].Run(this, Frame);
        }

        Frame.Tick++;
    }
}