using UnityEngine;

public sealed class ProjectileSystem : ISystem {
    private const float Epsilon = 0.0001f;

    public void Run(Simulation s, Frame fr) {
        var dt = fr.DeltaTime;

        foreach (var pair in fr.Projectiles) {
            var projectile = pair.Value;

            projectile.Lifetime -= dt;
            if (projectile.Lifetime <= 0f) {
                s.ProjectileRemovalRequests.Enqueue(projectile.Id);
                continue;
            }
            if (projectile.Position.y <= s.World.ground) {
                s.ProjectileRemovalRequests.Enqueue(projectile.Id);
                continue;
            }

            switch (projectile.Type) {
                case ProjectileType.Linear: MoveLinear(fr, projectile, dt); break;
                case ProjectileType.Homing: MoveHoming(fr, projectile, dt); break;
                case ProjectileType.Ballistic: MoveBallistic(fr, projectile, dt); break;
            }
        }
    }

    private void MoveLinear(Frame fr, ProjectileState projectile, float dt) {
        var hasTarget = fr.IsAlive(projectile.TargetEntityId);

        if (!hasTarget) {
            if (fr.FindEnemyInDirection(projectile.Team, projectile.Position, projectile.Direction, out var enemy)) {
                projectile.TargetEntityId = enemy.Id;
            }
        }

        projectile.Position += projectile.Direction * (projectile.Speed * dt);
    }

    private void MoveBallistic(Frame fr, ProjectileState projectile, float dt) {
        var config = fr.FindConfig<ProjectileConfig>(projectile.ConfigId);
        if (projectile.Velocity == Vector2.zero && projectile.Direction.sqrMagnitude > Epsilon) {
            projectile.Velocity = projectile.Direction * projectile.Speed;
        }

        projectile.Velocity += Vector2.down * (config.gravity * dt);
        projectile.Position += projectile.Velocity * dt;
        if (projectile.Velocity.sqrMagnitude > Epsilon) {
            projectile.Direction = projectile.Velocity.normalized;
            projectile.Speed = projectile.Velocity.magnitude;
        }
    }


    private void MoveHoming(Frame fr, ProjectileState projectile, float dt) {
        var dir = projectile.Direction;
        var hasTarget = fr.TryGetEntityPositionAndTeam(projectile.TargetEntityId, out var targetPosition, out _);

        if (!hasTarget) {
            if (fr.FindNearestEnemy(projectile.Team, projectile.Position, out var enemy)) {
                projectile.TargetEntityId = enemy.Id;
                targetPosition = enemy.Position;

                var delta = targetPosition - projectile.Position;
                if (delta.sqrMagnitude > 0.0001f) {
                    dir = delta.normalized;
                }
            }
        }

        var desiredPosition = projectile.Position + dir * (projectile.Speed * dt);
        var movementDirection = desiredPosition - projectile.Position;
        var toOther = targetPosition - projectile.Position;
        if (Vector2.Dot(movementDirection, toOther) <= 0f) {
            projectile.Position = desiredPosition;
        } else {
            projectile.Direction = dir;
            projectile.Position = desiredPosition;
        }
    }
}