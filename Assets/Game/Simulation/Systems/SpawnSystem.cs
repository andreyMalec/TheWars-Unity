using System;
using Unity.Mathematics;
using UnityEngine;

public sealed class SpawnSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        SpawnTeam(fr, s.SpawnQueue, Team.Left);
        SpawnTeam(fr, s.SpawnQueue, Team.Right);
    }

    private void SpawnTeam(Frame fr, SpawnQueue queue, Team team) {
        if (!fr.TryFindBaseByTeam(team, out var baseState)) return;

        while (queue.Count(team) > 0 ) {
            var spawnUnitRequest = queue.Dequeue(team);

            var unitConfig = fr.FindConfig<UnitConfig>(spawnUnitRequest.UnitConfigId);
            if (baseState.Resources < unitConfig.cost) {
                break;
            }

            baseState.Resources -= unitConfig.cost;
            baseState.SpawnQueue.Enqueue(spawnUnitRequest);
        }

        if (baseState.SpawnQueue.Count > 0 && baseState.SpawnProgress == null) {
            var r = baseState.SpawnQueue.Dequeue();
            var unitConfig = fr.FindConfig<UnitConfig>(r.UnitConfigId);
            
            baseState.SpawnProgress = new SpawnProgress() {
                Request = r,
                Timer = unitConfig.spawnTicks,
                SpawnTicks = unitConfig.spawnTicks
            };
        }
        if (baseState.SpawnProgress == null) return;
        var request = baseState.SpawnProgress.Request;
        if (baseState.SpawnProgress.Timer > 0) {
            baseState.SpawnProgress.Timer--;
            return;
        }

        var baseConfig = fr.FindConfig<BaseConfig>(baseState.ConfigId);
        if (IsSpawnAreaOccupied(fr, baseState, baseConfig)) {
            return;
        }

        var config = fr.FindConfig<UnitConfig>(request.UnitConfigId);
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
        baseState.SpawnProgress = null;
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