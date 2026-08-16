using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class Simulation {
    public const int TickRate = 60;

    private readonly List<ISystem> _systems = new();
    private readonly CommandQueue _commandQueue = new();

    public World World { get; }
    public Frame Frame { get; }
    public EventBus Events { get; }
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
        Events = new EventBus();
        World = configDatabase.World;

        _systems.Add(new EconomySystem());
        _systems.Add(new BuildTurretSystem());
        _systems.Add(new DestroyTurretSystem());
        _systems.Add(new SpawnSystem());
        _systems.Add(new BaseUpgradeSystem());
        _systems.Add(new BuySlotSystem());
        _systems.Add(new MovementSystem());
        _systems.Add(new TargetSystem());
        _systems.Add(new UnitWeaponSystem());
        _systems.Add(new TurretWeaponSystem());
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

    public void Tick() {
        _commandQueue.ExecuteAll(this);

        for (var i = 0; i < _systems.Count; i++) {
            try {
                _systems[i].Run(this, Frame);
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }

        Frame.Tick++;
    }
}