public sealed class BuildSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        while (s.BuildRequests.Count > 0) {
            var request = s.BuildRequests.Dequeue();
            if (!fr.TryFindBaseByTeam(request.Team, out var baseState)) {
                continue;
            }

            var config = fr.FindConfig<TurretConfig>(request.TurretConfigId);
            if (baseState.Resources < config.cost) continue;
            var slot = baseState.Slots[(int)request.Slot];
            if (!slot.IsActive || slot.HasTurret) continue;

            var baseConfig = fr.FindConfig<BaseConfig>(baseState.ConfigId);
            baseState.Resources -= config.cost;

            var state = new TurretState {
                Id = fr.GenerateEntityId(),
                Team = request.Team,
                ConfigId = request.TurretConfigId,
                Position = baseConfig.slotPositions[(int)request.Slot] + baseState.Position,
                Slot = request.Slot,
                TargetEntityId = 0,
                Cooldown = 0f
            };
            slot.HasTurret = true;
            slot.TurretId = state.Id;
            slot.TurretConfigId = state.ConfigId;

            fr.AddTurret(state);
        }
    }
}