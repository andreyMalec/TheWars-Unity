using System;
using UnityEngine;

public sealed class SpawnSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        SpawnTeam(fr, s.SpawnQueue, Team.Left);
        SpawnTeam(fr, s.SpawnQueue, Team.Right);
    }

    private void SpawnTeam(Frame fr, SpawnQueue queue, Team team) {
        if (!fr.TryFindBaseByTeam(team, out var baseState)) return;
        var baseConfig = fr.FindConfig<BaseConfig>(baseState.ConfigId);

        foreach (EntityType unitType in Enum.GetValues(typeof(EntityType))) {
            SpawnUnitType(fr, queue, baseConfig, baseState, unitType);
        }
    }

    private void SpawnUnitType(
        Frame fr, SpawnQueue queue,
        BaseConfig baseConfig, BaseState baseState, EntityType entityType
    ) {
        var team = baseState.Team;
        while (queue.Count(team, entityType) > 0) {
            var request = queue.Peek(team, entityType);

            if (IsSpawnAreaOccupied(fr, baseState, baseConfig)) {
                break;
            }

            var config = fr.FindConfig<UnitConfig>(request.UnitConfigId);
            if (baseState.Resources < config.cost) {
                break;
            }

            baseState.Resources -= config.cost;
            queue.Dequeue(team, entityType);

            var state = new UnitState {
                Id = fr.GenerateEntityId(),
                Team = request.Team,
                ConfigId = request.UnitConfigId,
                Position = baseState.Position,
                Direction = team == Team.Left ? UnitDirection.Right : UnitDirection.Left,
                Health = config.maxHealth,
                MaxHealth = config.maxHealth,
                TargetEntityId = 0,
                IsAlive = true,
                Attack = new AttackState() {
                    RecoveryTick = fr.Tick + 1,
                }
            };

            fr.AddUnit(state);
            Debug.Log(
                $"[SpawnSystem] Spawned unit (Team {team}, Config {state.ConfigId}). Remaining resources: {baseState.Resources}");
        }
    }

    private static bool IsSpawnAreaOccupied(Frame frame, BaseState baseState, BaseConfig baseConfig) {
        foreach (var pair in frame.Units) {
            var unit = pair.Value;
            if (BaseBoundsUtility.ContainsPoint(baseState, baseConfig, unit.Position)) {
                return true;
            }
        }

        return false;
    }
}