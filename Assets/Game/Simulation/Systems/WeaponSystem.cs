using UnityEngine;

public sealed class WeaponSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        var dt = fr.DeltaTime;

        foreach (var pair in fr.Units) {
            var unit = pair.Value;
            var config = fr.FindConfig<UnitConfig>(unit.ConfigId);

            if (unit.Cooldown > 0f) {
                unit.Cooldown -= dt;
            }

            if (unit.TargetEntityId == 0) {
                continue;
            }

            if (!fr.TryGetEntityPositionAndTeam(unit.TargetEntityId, out var targetPosition, out _)) {
                continue;
            }

            var inRange = (targetPosition - unit.Position).sqrMagnitude <= config.attackRange * config.attackRange;
            if (!inRange || unit.Cooldown > 0f) {
                continue;
            }

            if (config.attackType == UnitAttackType.Ranged) {
                var direction = (targetPosition - unit.Position).normalized;
                var projectilePosition = UnitColliderUtility.ToWorldPoint(
                    config.projectilePosition,
                    unit.Position,
                    UnitColliderUtility.IsMirrored(unit.Direction));
                var projConfig = fr.FindConfig<ProjectileConfig>(config.projectileId);
                var state = new ProjectileState {
                    Id = fr.GenerateEntityId(),
                    ConfigId = projConfig.id,
                    Team = unit.Team,
                    SourceEntityId = unit.Id,
                    TargetEntityId = unit.TargetEntityId,
                    Damage = config.damage,
                    Position = projectilePosition,
                    Direction = direction,
                    Speed = projConfig.speed,
                    Lifetime = 5f
                };

                fr.AddProjectile(state);
            } else {
                s.DamageRequests.Enqueue(new DamageRequest {
                    SourceEntityId = unit.Id,
                    TargetEntityId = unit.TargetEntityId,
                    Amount = config.damage
                });
            }

            unit.Cooldown = config.attackInterval > 0f ? config.attackInterval : 1f;
        }

        foreach (var pair in fr.Turrets) {
            // var turret = pair.Value; TODO
            // var config = fr.FindConfig<TurretConfig>(turret.ConfigId);
            //
            // if (turret.Cooldown > 0f) {
            //     turret.Cooldown -= dt;
            // }
            //
            // if (turret.TargetEntityId == 0) {
            //     continue;
            // }
            //
            // if (!fr.TryGetEntityPositionAndTeam(turret.TargetEntityId, out var targetPosition, out _)) {
            //     continue;
            // }
            //
            // var inRange = (targetPosition - turret.Position).sqrMagnitude <= config.AttackRange * config.AttackRange;
            // if (!inRange || turret.Cooldown > 0f) {
            //     continue;
            // }
            //
            // var direction = (targetPosition - turret.Position).normalized;
            // SpawnProjectile(fr, turret.Team, turret.Id, turret.TargetEntityId, turret.Position, direction,
            //     config.Damage,
            //     config.ProjectileSpeed);
            // turret.Cooldown = config.AttackInterval > 0f ? config.AttackInterval : 1f;
        }
    }

    private static void SpawnProjectile(
        Frame fr,
        Team team,
        int sourceEntityId,
        int targetEntityId,
        Vector2 position,
        Vector2 direction,
        int damage,
        float speed
    ) {
        var state = new ProjectileState {
            Id = fr.GenerateEntityId(),
            Team = team,
            SourceEntityId = sourceEntityId,
            TargetEntityId = targetEntityId,
            Damage = damage,
            Position = position,
            Direction = direction,
            Speed = speed,
            Lifetime = 5f
        };

        fr.AddProjectile(state);
    }
}