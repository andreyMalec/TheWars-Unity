using System;
using System.Collections.Generic;

public sealed class Simulation {
    public const int TickRate = 60;

    private readonly List<ISystem> _systems = new();
    private readonly CommandQueue _commandQueue = new();
    private readonly Dictionary<Type, List<object>> _listeners = new();

    public Frame Frame { get; }
    public SpawnQueue SpawnQueue { get; }
    public readonly Queue<BuildTurretRequest> BuildTurretRequests = new();
    public readonly Queue<DestroyTurretRequest> DestroyTurretRequests = new();
    public readonly Queue<BuySlotRequest> BuySlotRequests = new();
    public readonly Queue<UpgradeBaseRequest> UpgradeRequests = new();
    public readonly Queue<SpecialWeaponRequest> SpecialWeaponRequests = new();
    public readonly Queue<DamageRequest> DamageRequests = new();
    public readonly Queue<int> ProjectileRemovalRequests = new();

    public Simulation(ConfigDatabase configDatabase, int tickRate) {
        Frame = new Frame(1f / tickRate, configDatabase);
        SpawnQueue = new SpawnQueue();

        _systems.Add(new EconomySystem());
        _systems.Add(new BuildTurretSystem());
        _systems.Add(new DestroyTurretSystem());
        _systems.Add(new SpawnSystem());
        _systems.Add(new BaseUpgradeSystem());
        _systems.Add(new BuySlotSystem());
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
        _commandQueue.Enqueue(command);
    }

    public void Subscribe<T>(IEventListener<T> listener) where T : IEvent {
        if (!_listeners.TryGetValue(typeof(T), out var list)) {
            list = new List<object>();
            _listeners.Add(typeof(T), list);
        }

        list.Add(listener);
    }

    public void Publish<T>(T e) where T : IEvent {
        if (!_listeners.TryGetValue(typeof(T), out var list))
            return;

        foreach (var listener in list) {
            ((IEventListener<T>)listener).OnEvent(e);
        }
    }

    public void Tick() {
        _commandQueue.ExecuteAll(this);

        for (var i = 0; i < _systems.Count; i++) {
            _systems[i].Run(this, Frame);
        }

        Frame.Tick++;
    }
}