public sealed class DestroyTurretSystem : ISystem {
    private const float RefundPercentage = 0.5f;

    public void Run(Simulation s, Frame fr) {
        while (s.DestroyTurretRequests.Count > 0) {
            var request = s.DestroyTurretRequests.Dequeue();
            if (!fr.TryFindBaseByTeam(request.Team, out var baseState)) {
                continue;
            }

            TurretState turret = null;
            foreach (var (_, t) in fr.Turrets) {
                if (t.Team != request.Team) continue;
                if (t.Slot != request.Slot) continue;
                turret = t;
                break;
            }

            if (turret == null) continue;
            var config = fr.FindConfig<TurretConfig>(turret.ConfigId);
            var slot = baseState.Slots[(int)request.Slot];
            if (!slot.HasTurret) continue;

            baseState.Resources += (int)(config.cost * RefundPercentage);

            slot.HasTurret = false;
            slot.TurretId = -1;
            slot.TurretConfigId = default;

            fr.RemoveTurret(turret.Id);
        }
    }
}