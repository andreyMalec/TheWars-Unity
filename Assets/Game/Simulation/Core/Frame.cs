using System.Collections.Generic;
using UnityEngine;

public sealed class Frame {
    private readonly EntityIdGenerator _entityIdGenerator = new EntityIdGenerator();

    public IReadOnlyDictionary<int, BaseState> Bases => _bases;
    public IReadOnlyDictionary<int, UnitState> Units => _units;
    public IReadOnlyDictionary<int, TurretState> Turrets => _turrets;
    public IReadOnlyDictionary<int, ProjectileState> Projectiles => _projectiles;

    private readonly Dictionary<int, BaseState> _bases = new();
    private readonly Dictionary<int, UnitState> _units = new();
    private readonly Dictionary<int, TurretState> _turrets = new();
    private readonly Dictionary<int, ProjectileState> _projectiles = new();

    private readonly ConfigDatabase _configDatabase;

    public int Tick = 0;
    public float DeltaTime { get; private set; }

    public Frame(float tickDeltaTime, ConfigDatabase configDatabase) {
        DeltaTime = tickDeltaTime;
        _configDatabase = configDatabase;
    }

    public T FindConfig<T>(ConfigId configId) where T : EntityConfig {
        return _configDatabase.GetConfig<T>(configId);
    }

    public T FindConfig<T>(Epoch epoch, EntityType type) where T : EntityConfig {
        return _configDatabase.GetConfig<T>(epoch, type);
    }

    public int GenerateEntityId() {
        return _entityIdGenerator.Next();
    }

    public void AddBase(BaseState state) {
        _bases[state.Id] = state;
    }

    public bool RemoveBase(int id) {
        return _bases.Remove(id);
    }

    public bool TryFindBase(int id, out BaseState state) {
        return _bases.TryGetValue(id, out state);
    }

    public bool TryFindBaseByTeam(Team team, out BaseState state) {
        foreach (var pair in _bases) {
            if (pair.Value.Team == team) {
                state = pair.Value;
                return true;
            }
        }

        state = null;
        return false;
    }

    public void AddUnit(UnitState state) {
        _units[state.Id] = state;
    }

    public bool RemoveUnit(int id) {
        return _units.Remove(id);
    }

    public bool TryFindUnit(int id, out UnitState state) {
        return _units.TryGetValue(id, out state);
    }

    public void AddTurret(TurretState state) {
        _turrets[state.Id] = state;
    }

    public bool RemoveTurret(int id) {
        return _turrets.Remove(id);
    }

    public bool TryFindTurret(int id, out TurretState state) {
        return _turrets.TryGetValue(id, out state);
    }

    public void AddProjectile(ProjectileState state) {
        _projectiles[state.Id] = state;
    }

    public bool RemoveProjectile(int id) {
        return _projectiles.Remove(id);
    }

    public bool TryFindProjectile(int id, out ProjectileState state) {
        return _projectiles.TryGetValue(id, out state);
    }

    public bool IsAlive(int entityId) {
        if (Units.TryGetValue(entityId, out var unit)) {
            return unit.IsAlive;
        }

        if (Bases.ContainsKey(entityId)) {
            return true;
        }

        return false;
    }

    public bool TryGetEntityPositionAndTeam(int entityId, out Vector2 position, out Team team) {
        if (Units.TryGetValue(entityId, out var unit)) {
            position = unit.Position;
            team = unit.Team;
            return true;
        }

        if (Bases.TryGetValue(entityId, out var baseState)) {
            position = baseState.Position;
            team = baseState.Team;
            return true;
        }

        position = Vector2.zero;
        team = 0;
        return false;
    }

    public bool TryGetUnit(int entityId, out UnitState unit, out UnitConfig config) {
        if (Units.TryGetValue(entityId, out unit)) {
            config = FindConfig<UnitConfig>(unit.ConfigId);
            return true;
        }

        config = null;
        return false;
    }

    public Vector2 GetEnemyBasePosition(int entityId) {
        if (Units.TryGetValue(entityId, out var unit)) {
            return GetEnemyBasePosition(unit);
        }

        return Vector2.zero;
    }

    public Vector2 GetEnemyBasePosition(UnitState unit) {
        foreach (var pair in _bases) {
            if (pair.Value.Team != unit.Team) {
                return pair.Value.Position;
            }
        }

        return Vector2.zero;
    }

    public bool FindEnemyInDirection(Team team, Vector2 origin, Vector2 direction, out Damageable damageable) {
        return TargetingUtility.FindEnemyInDirection(this, team, origin, direction, out damageable);
    }

    public bool FindNearestEnemy(Team team, Vector2 origin, out Damageable damageable) {
        return TargetingUtility.FindNearestEnemy(this, team, origin, out damageable);
    }

    public int FindNearestEnemyId(Team team, Vector2 origin) {
        TargetingUtility.FindNearestEnemy(this, team, origin, out var damageable);
        return damageable?.Id ?? 0;
    }

    public Team LocalPlayerTeam() {
        return Team.Left;
    }
}