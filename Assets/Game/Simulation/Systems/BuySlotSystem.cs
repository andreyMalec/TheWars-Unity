public sealed class BuySlotSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        while (s.BuySlotRequests.Count > 0) {
            var request = s.BuySlotRequests.Dequeue();

            if (fr.TryFindBaseByTeam(request.Team, out var baseState)) {
                var config = fr.FindConfig<BaseConfig>(baseState.ConfigId);
                var cost = baseState.NextSlotCost(config, out var index);
                if (cost == 0) continue;
                if (baseState.Resources < cost) {
                    continue;
                }

                baseState.Resources -= cost;
                baseState.Slots[index].IsActive = true;
            }
        }
    }
}