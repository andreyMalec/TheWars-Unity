public sealed class ProjectileSystem : ISystem {
    public void Run(Simulation simulation) {
        var world = simulation.Frame.World;
        var dt = simulation.TickDeltaTime;

        foreach (var pair in world.Projectiles) {
            var projectile = pair.Value;
            var hasTarget = world.TryGetEntityPositionAndTeam(projectile.TargetEntityId, out var targetPosition, out _);

            if (!hasTarget) {
                projectile.TargetEntityId = TargetingUtility.FindNearestEnemy(world, projectile.Team, projectile.Position);
                hasTarget = world.TryGetEntityPositionAndTeam(projectile.TargetEntityId, out targetPosition, out _);
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
                simulation.ProjectileRemovalRequests.Enqueue(projectile.Id);
            }
        }
    }
}