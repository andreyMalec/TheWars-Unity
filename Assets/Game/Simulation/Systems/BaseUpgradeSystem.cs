public sealed class BaseUpgradeSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        while (s.UpgradeRequests.Count > 0) {
            var request = s.UpgradeRequests.Dequeue();

            if (fr.TryFindBaseByTeam(request.Team, out var baseState)) {
                var config = fr.FindConfig<BaseConfig>(baseState.ConfigId);
                if (baseState.Resources < config.upgradeCost) {
                    continue;
                }

                baseState.Resources -= config.upgradeCost;
                baseState.Epoch += 1;
                baseState.Health += config.healthPerUpgrade;
            }
        }
    }
}