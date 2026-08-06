public sealed class BaseUpgradeSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        while (s.UpgradeRequests.Count > 0) {
            var request = s.UpgradeRequests.Dequeue();

            if (fr.TryFindBase(request.BaseEntityId, out var baseState)) {
                var config = s.ConfigDatabase.GetBaseConfig(baseState.ConfigId);
                if (baseState.Resources < config.UpgradeCost) {
                    continue;
                }

                baseState.Resources -= config.UpgradeCost;
                baseState.Level += 1;
                baseState.Health += config.HealthPerUpgrade;
            }
        }
    }
}