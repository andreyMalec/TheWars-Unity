public sealed class ProjectileSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        var dt = s.TickDeltaTime;

        foreach (var pair in fr.Projectiles) {
            var projectile = pair.Value;
            var hasTarget = fr.TryGetEntityPositionAndTeam(projectile.TargetEntityId, out var targetPosition, out _);

            if (!hasTarget) {
                projectile.TargetEntityId = TargetingUtility.FindNearestEnemy(fr, projectile.Team, projectile.Position);
                hasTarget = fr.TryGetEntityPositionAndTeam(projectile.TargetEntityId, out targetPosition, out _);
            }

            if (hasTarget) {
                var delta = targetPosition - projectile.Position;
                if (delta.sqrMagnitude > 0.0001f) {
                    projectile.Direction = delta.normalized;
                }
            }

            projectile.Position += projectile.Direction * (projectile.Speed * dt);
            projectile.Lifetime -= dt;

            if (projectile.Lifetime <= 0f) {
                s.ProjectileRemovalRequests.Enqueue(projectile.Id);
            }
        }
    }
}