using UnityEngine;

public sealed class SpawnSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        SpawnTeam(fr, s.SpawnQueue, Team.Left);
        SpawnTeam(fr, s.SpawnQueue, Team.Right);
    }

    private void SpawnTeam(Frame fr, SpawnQueue queue, Team team) {
        if (!fr.TryFindBaseByTeam(team, out var baseState)) return;
        var baseConfig = fr.FindConfig<BaseConfig>(baseState.ConfigId);

        while (queue.Count(team) > 0) {
            var request = queue.Peek(team);

            if (IsSpawnAreaOccupied(fr, baseState, baseConfig)) {
                break;
            }

            var config = fr.FindConfig<UnitConfig>(request.UnitConfigId);
            if (baseState.Resources < config.cost) {
                break;
            }

            baseState.Resources -= config.cost;
            queue.Dequeue(team);

            var state = new UnitState {
                Id = fr.GenerateEntityId(),
                Team = request.Team,
                ConfigId = request.UnitConfigId,
                Position = request.Position,
                Direction = UnitDirection.Right,
                Health = config.maxHealth,
                MaxHealth = config.maxHealth,
                TargetEntityId = 0,
                Cooldown = 0f
            };

            var targetPosition = fr.GetEnemyBasePosition(state);
            state.Direction =
                UnitColliderUtility.ResolveDirection(state.Direction, targetPosition.x - state.Position.x);

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