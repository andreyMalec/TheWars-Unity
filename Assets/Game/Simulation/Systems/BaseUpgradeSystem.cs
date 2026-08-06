public sealed class BaseUpgradeSystem : ISystem
{
    public void Run(Simulation simulation)
    {
        while (simulation.UpgradeRequests.Count > 0)
        {
            var request = simulation.UpgradeRequests.Dequeue();

            if (simulation.Frame.World.TryFindBase(request.BaseEntityId, out var baseState))
            {
                var config = simulation.ConfigDatabase.GetBaseConfig(baseState.ConfigId);
                baseState.Level += 1;
                baseState.Health += config.HealthPerUpgrade;
            }
        }
    }
}

