public sealed class BuildSystem : ISystem {
    public void Run(Simulation simulation) {
        while (simulation.BuildRequests.Count > 0) {
            var request = simulation.BuildRequests.Dequeue();
            if (!simulation.Frame.World.TryFindBaseByTeam(request.Team, out var baseState)) {
                continue;
            }

            var config = simulation.ConfigDatabase.GetTurretConfig(request.TurretConfigId);
            if (baseState.Resources < config.Cost) {
                continue;
            }

            baseState.Resources -= config.Cost;

            var state = new TurretState {
                Id = simulation.Frame.World.GenerateEntityId(),
                Team = request.Team,
                ConfigId = request.TurretConfigId,
                Position = request.Position,
                TargetEntityId = 0,
                Cooldown = 0f
            };

            simulation.Frame.World.AddTurret(state);
        }
    }
}