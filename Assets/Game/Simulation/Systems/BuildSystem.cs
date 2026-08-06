public sealed class BuildSystem : ISystem
{
    public void Run(Simulation simulation)
    {
        while (simulation.BuildRequests.Count > 0)
        {
            var request = simulation.BuildRequests.Dequeue();
            var config = simulation.ConfigDatabase.GetTurretConfig(request.TurretConfigId);

            var state = new TurretState
            {
                Id = simulation.Frame.World.GenerateEntityId(),
                Team = request.Team,
                ConfigId = request.TurretConfigId,
                Position = request.Position,
                Health = config.MaxHealth,
                Cooldown = 0f
            };

            simulation.Frame.World.AddTurret(state);
        }
    }
}

