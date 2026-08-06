public sealed class BuildSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        while (s.BuildRequests.Count > 0) {
            var request = s.BuildRequests.Dequeue();
            if (!fr.TryFindBaseByTeam(request.Team, out var baseState)) {
                continue;
            }

            var config = s.ConfigDatabase.GetTurretConfig(request.TurretConfigId);
            if (baseState.Resources < config.Cost) {
                continue;
            }

            baseState.Resources -= config.Cost;

            var state = new TurretState {
                Id = fr.GenerateEntityId(),
                Team = request.Team,
                ConfigId = request.TurretConfigId,
                Position = request.Position,
                TargetEntityId = 0,
                Cooldown = 0f
            };

            fr.AddTurret(state);
        }
    }
}