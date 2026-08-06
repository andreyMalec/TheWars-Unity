using UnityEngine;

public sealed class WeaponSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        var dt = s.TickDeltaTime;

        foreach (var pair in fr.Units) {
            var unit = pair.Value;
            var config = s.ConfigDatabase.GetUnitConfig(unit.ConfigId);

            if (unit.Cooldown > 0f) {
                unit.Cooldown -= dt;
            }

            if (unit.TargetEntityId == 0) {
                continue;
            }

            if (!fr.TryGetEntityPositionAndTeam(unit.TargetEntityId, out var targetPosition, out _)) {
                continue;
            }

            var inRange = (targetPosition - unit.Position).sqrMagnitude <= config.AttackRange * config.AttackRange;
            if (!inRange || unit.Cooldown > 0f) {
                continue;
            }

            if (config.type == UnitAttackType.Ranged) {
                var direction = (targetPosition - unit.Position).normalized;
                SpawnProjectile(fr, unit.Team, unit.Id, unit.TargetEntityId, unit.Position, direction, config.Damage,
                    config.ProjectileSpeed);
            } else {
                s.DamageRequests.Enqueue(new DamageRequest {
                    SourceEntityId = unit.Id,
                    TargetEntityId = unit.TargetEntityId,
                    Amount = config.Damage
                });
            }

            unit.Cooldown = config.AttackInterval > 0f ? config.AttackInterval : 1f;
        }

        foreach (var pair in fr.Turrets) {
            var turret = pair.Value;
            var config = s.ConfigDatabase.GetTurretConfig(turret.ConfigId);

            if (turret.Cooldown > 0f) {
                turret.Cooldown -= dt;
            }

            if (turret.TargetEntityId == 0) {
                continue;
            }

            if (!fr.TryGetEntityPositionAndTeam(turret.TargetEntityId, out var targetPosition, out _)) {
                continue;
            }

            var inRange = (targetPosition - turret.Position).sqrMagnitude <= config.AttackRange * config.AttackRange;
            if (!inRange || turret.Cooldown > 0f) {
                continue;
            }

            var direction = (targetPosition - turret.Position).normalized;
            SpawnProjectile(fr, turret.Team, turret.Id, turret.TargetEntityId, turret.Position, direction,
                config.Damage,
                config.ProjectileSpeed);
            turret.Cooldown = config.AttackInterval > 0f ? config.AttackInterval : 1f;
        }
    }

    private static void SpawnProjectile(
        Frame fr,
        int team,
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