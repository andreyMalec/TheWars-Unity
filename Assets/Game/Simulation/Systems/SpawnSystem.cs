using UnityEngine;

public sealed class SpawnSystem : ISystem {
    public void Run(Simulation simulation) {
        while (simulation.SpawnRequests.Count > 0) {
            var request = simulation.SpawnRequests.Peek();
            if (!simulation.Frame.World.TryFindBaseByTeam(request.Team, out var baseState)) {
                break;
            }

            var config = simulation.ConfigDatabase.GetUnitConfig(request.UnitConfigId);
            if (baseState.Resources < config.Cost) {
                break;
            }

            baseState.Resources -= config.Cost;
            simulation.SpawnRequests.Dequeue();

            var state = new UnitState {
                Id = simulation.Frame.World.GenerateEntityId(),
                Team = request.Team,
                ConfigId = request.UnitConfigId,
                Position = request.Position,
                Size = config.Size,
                Destination = request.Destination,
                HasDestination = true,
                Health = config.MaxHealth,
                TargetEntityId = 0,
                Cooldown = 0f
            };

            simulation.Frame.World.AddUnit(state);
            Debug.Log($"[SpawnSystem] Spawned unit (Team {state.Team}, Config {state.ConfigId}) at position {state.Position}. Remaining resources: {baseState.Resources}");
        }
    }
}