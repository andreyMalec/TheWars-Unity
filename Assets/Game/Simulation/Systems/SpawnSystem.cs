public sealed class SpawnSystem : ISystem
{
    public void Run(Simulation simulation)
    {
        while (simulation.SpawnRequests.Count > 0)
        {
            var request = simulation.SpawnRequests.Dequeue();
            var config = simulation.ConfigDatabase.GetUnitConfig(request.UnitConfigId);

            var state = new UnitState
            {
                Id = simulation.Frame.World.GenerateEntityId(),
                Team = request.Team,
                ConfigId = request.UnitConfigId,
                Position = request.Position,
                Health = config.MaxHealth,
                TargetEntityId = 0,
                Cooldown = 0f
            };

            simulation.Frame.World.AddUnit(state);
        }
    }
}

