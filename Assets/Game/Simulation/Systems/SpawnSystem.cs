using UnityEngine;

public sealed class SpawnSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        while (s.SpawnRequests.Count > 0) {
            var request = s.SpawnRequests.Peek();
            if (!fr.TryFindBaseByTeam(request.Team, out var baseState)) {
                break;
            }

            var config = s.ConfigDatabase.GetUnitConfig(request.UnitConfigId);
            if (baseState.Resources < config.Cost) {
                break;
            }

            baseState.Resources -= config.Cost;
            s.SpawnRequests.Dequeue();

            var state = new UnitState {
                Id = fr.GenerateEntityId(),
                Team = request.Team,
                ConfigId = request.UnitConfigId,
                Position = request.Position,
                Size = config.Size,
                Health = config.MaxHealth,
                TargetEntityId = 0,
                Cooldown = 0f
            };

            fr.AddUnit(state);
            Debug.Log(
                $"[SpawnSystem] Spawned unit (Team {state.Team}, Config {state.ConfigId}) at position {state.Position}. Remaining resources: {baseState.Resources}");
        }
    }
}