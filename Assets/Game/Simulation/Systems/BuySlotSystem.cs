public sealed class BuySlotSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        while (s.BuySlotRequests.Count > 0) {
            var request = s.BuySlotRequests.Dequeue();

            if (fr.TryFindBaseByTeam(request.Team, out var baseState)) {
                var config = fr.FindConfig<BaseConfig>(baseState.ConfigId);
                int i = 0;
                Slot? slot = null;
                for ( i = 0; i < baseState.Slots.Length; i++) {
                    if (!baseState.Slots[i].IsActive) {
                        slot  = baseState.Slots[i];
                        break;
                    }
                }
                if (slot == null) continue;
                var cost = config.slotCost[(TurretSlot)i];
                if (baseState.Resources < cost) {
                    continue;
                }

                baseState.Resources -= cost;
                baseState.Slots[i].IsActive = true;
            }
        }
    }
}