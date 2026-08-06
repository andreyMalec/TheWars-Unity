public sealed class TargetSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        foreach (var pair in fr.Units) {
            var unit = pair.Value;
            unit.TargetEntityId = TargetingUtility.FindNearestEnemy(fr, unit.Team, unit.Position);
        }

        foreach (var pair in fr.Turrets) {
            var turret = pair.Value;
            turret.TargetEntityId = TargetingUtility.FindNearestEnemy(fr, turret.Team, turret.Position);
        }
    }
}