using System.Collections.Generic;
using UnityEngine;

public sealed class Frame {
    private readonly EntityIdGenerator _entityIdGenerator = new EntityIdGenerator();

    public readonly Dictionary<int, BaseState> Bases = new Dictionary<int, BaseState>();
    public readonly Dictionary<int, UnitState> Units = new Dictionary<int, UnitState>();
    public readonly Dictionary<int, TurretState> Turrets = new Dictionary<int, TurretState>();
    public readonly Dictionary<int, ProjectileState> Projectiles = new Dictionary<int, ProjectileState>();

    public int Tick = 0;

    public int GenerateEntityId() {
        return _entityIdGenerator.Next();
    }

    public void AddBase(BaseState state) {
        Bases[state.Id] = state;
    }

    public bool RemoveBase(int id) {
        return Bases.Remove(id);
    }

    public bool TryFindBase(int id, out BaseState state) {
        return Bases.TryGetValue(id, out state);
    }

    public bool TryFindBaseByTeam(int team, out BaseState state) {
        foreach (var pair in Bases) {
            if (pair.Value.Team == team) {
                state = pair.Value;
                return true;
            }
        }

        state = null;
        return false;
    }

    public void AddUnit(UnitState state) {
        Units[state.Id] = state;
    }

    public bool RemoveUnit(int id) {
        return Units.Remove(id);
    }

    public bool TryFindUnit(int id, out UnitState state) {
        return Units.TryGetValue(id, out state);
    }

    public void AddTurret(TurretState state) {
        Turrets[state.Id] = state;
    }

    public bool RemoveTurret(int id) {
        return Turrets.Remove(id);
    }

    public bool TryFindTurret(int id, out TurretState state) {
        return Turrets.TryGetValue(id, out state);
    }

    public void AddProjectile(ProjectileState state) {
        Projectiles[state.Id] = state;
    }

    public bool RemoveProjectile(int id) {
        return Projectiles.Remove(id);
    }

    public bool TryFindProjectile(int id, out ProjectileState state) {
        return Projectiles.TryGetValue(id, out state);
    }

    public bool TryGetEntityPositionAndTeam(int entityId, out Vector2 position, out int team) {
        if (Units.TryGetValue(entityId, out var unit)) {
            position = unit.Position;
            team = unit.Team;
            return true;
        }

        if (Turrets.TryGetValue(entityId, out var turret)) {
            position = turret.Position;
            team = turret.Team;
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

    public bool TryGetEntityPositionTeamAndRadius(int entityId, out Vector2 position, out int team, out float radius) {
        if (Units.TryGetValue(entityId, out var unit)) {
            position = unit.Position;
            team = unit.Team;
            radius = unit.Size * 0.5f;
            return true;
        }

        if (Turrets.TryGetValue(entityId, out var turret)) {
            position = turret.Position;
            team = turret.Team;
            radius = 0.5f;
            return true;
        }

        if (Bases.TryGetValue(entityId, out var baseState)) {
            position = baseState.Position;
            team = baseState.Team;
            radius = 1f;
            return true;
        }

        position = Vector2.zero;
        team = 0;
        radius = 0f;
        return false;
    }
}