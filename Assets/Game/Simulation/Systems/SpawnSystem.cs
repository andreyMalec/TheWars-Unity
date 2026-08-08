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

        foreach (UnitType unitType in Enum.GetValues(typeof(UnitType))) {
            SpawnUnitType(fr, queue, baseConfig, baseState, unitType);
        }
    }

    private void SpawnUnitType(
        Frame fr, SpawnQueue queue,
        BaseConfig baseConfig, BaseState baseState, UnitType unitType
    ) {
        var team = baseState.Team;
        while (queue.Count(team, unitType) > 0) {
            var request = queue.Peek(team, unitType);

            if (IsSpawnAreaOccupied(fr, baseState, baseConfig)) {
                break;
            }

            var config = fr.FindConfig<UnitConfig>(request.UnitConfigId);
            if (baseState.Resources < config.cost) {
                break;
            }

            baseState.Resources -= config.cost;
            queue.Dequeue(team, unitType);

            var state = new UnitState {
                Id = fr.GenerateEntityId(),
                Team = request.Team,
                ConfigId = request.UnitConfigId,
                Position = baseState.Position,
                Direction = team == Team.Left ? UnitDirection.Right : UnitDirection.Left,
                Health = config.maxHealth,
                MaxHealth = config.maxHealth,
                TargetEntityId = 0,
                Cooldown = 0f
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