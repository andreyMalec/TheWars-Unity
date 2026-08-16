using UnityEngine;

public sealed class TurretWeaponSystem : ISystem {
    private const float Epsilon = 0.0001f;

    public void Run(Simulation s, Frame fr) {
        foreach (var pair in fr.Turrets) {
            var turret = pair.Value;
            var config = fr.FindConfig<TurretConfig>(turret.ConfigId);
            RotateToTarget(fr, turret, config);

            // 1. Выполнить уже начатую атаку
            if (TryAttack(fr, turret, config)) continue;

            // 2. Если сейчас готова новая атака
            if (!CanAttack(fr, turret, config)) continue;

            // Начинаем атаку
            var attack = config.attackTicks[turret.AttackIndex];
            if (attack.type == AttackTickType.Execute) {
                turret.ExecuteTick = fr.Tick + attack.value;
            }

            if (turret.AttackIndex == 0) {
                s.Events.Publish(new TurretEvent.AttackStarted(turret.Id));
            }
        }
    }

    private bool TryAttack(Frame fr, TurretState turret, TurretConfig config) {
        if (turret.ExecuteTick > 0 && fr.Tick >= turret.ExecuteTick) {
            ExecuteAttack(fr, turret, config);
            turret.AttackIndex++;
            turret.ExecuteTick = 0;
            var attack = config.attackTicks[turret.AttackIndex];
            if (attack.type == AttackTickType.Recovery) {
                turret.RecoveryTick = fr.Tick + attack.value;
                turret.AttackIndex = 0;
            }

            return true;
        }

        return false;
    }

    private bool CanAttack(Frame fr, TurretState turret, TurretConfig config) {
        if (turret.ExecuteTick > 0) return false;
        if (turret.RecoveryTick > fr.Tick) return false;

        // Если это множественная атака, достреливаем все, что есть
        if (turret.AttackIndex > 0) return true;

        if (turret.TargetEntityId == 0) return false;
        if (!fr.TryGetEntityPosition(turret.TargetEntityId, out var targetPosition)) {
            turret.TargetEntityId = 0;
            turret.AttackIndex = 0;
            return false;
        }

        turret.LastTargetPosition = targetPosition;

        var inRange = (targetPosition - turret.Position).sqrMagnitude <= config.attackRange * config.attackRange;

        if (!inRange) {
            turret.AttackIndex = 0;
            return false;
        }

        return true;
    }

    private void RotateToTarget(Frame fr, TurretState turret, TurretConfig config) {
        if (!config.rotateToTarget) {
            turret.Rotation = 0f;
            return;
        }

        if (turret.LastTargetPosition == Vector2.zero) return;

        var toTarget = turret.LastTargetPosition - turret.Position;
        if (toTarget.sqrMagnitude <= Epsilon) return;

        var direction = toTarget.normalized;
        var projectileConfig = fr.FindConfig<ProjectileConfig>(config.projectileId);

        if (projectileConfig.type == ProjectileType.Ballistic) {
            var targetVelocity = BallisticsUtility.ResolveTargetVelocity(fr, turret.TargetEntityId);
            direction = BallisticsUtility.CalculateAimedBallisticDirection(
                turret.Position,
                turret.LastTargetPosition,
                targetVelocity,
                projectileConfig.speed,
                projectileConfig.gravity,
                projectileConfig.highArc,
                projectileConfig.autoSwitchArcRoot);
        }

        turret.Rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    private void ExecuteAttack(
        Frame fr,
        TurretState turret,
        TurretConfig config
    ) {
        fr.TryGetEntityPosition(turret.TargetEntityId, out var targetPosition);
        if (targetPosition == Vector2.zero)
            targetPosition = turret.LastTargetPosition;

        var rotated = Vector3.RotateTowards(config.projectilePositions[turret.AttackIndex], turret.Position,
            turret.Rotation * Mathf.Deg2Rad, 0f);
        var projectilePosition = UnitColliderUtility.ToWorldPoint(
            rotated,
            turret.Position,
            turret.Team == Team.Right);

        var projConfig = fr.FindConfig<ProjectileConfig>(config.projectileId);
        var direction = (targetPosition - projectilePosition).normalized;
        var velocity = Vector2.zero;

        if (projConfig.type == ProjectileType.Ballistic) {
            var targetVelocity = BallisticsUtility.ResolveTargetVelocity(fr, turret.TargetEntityId);
            direction = BallisticsUtility.CalculateAimedBallisticDirection(
                projectilePosition,
                targetPosition,
                targetVelocity,
                projConfig.speed,
                projConfig.gravity,
                projConfig.highArc,
                projConfig.autoSwitchArcRoot);
            velocity = direction * projConfig.speed;
        }

        var state = new ProjectileState {
            Id = fr.GenerateEntityId(),
            ConfigId = projConfig.id,
            Team = turret.Team,
            SourceEntityId = turret.Id,
            TargetEntityId = turret.TargetEntityId,
            Damage = config.damage,
            Position = projectilePosition,
            Direction = direction,
            Velocity = velocity,
            Speed = projConfig.speed,
            Type = projConfig.type,
        };

        fr.AddProjectile(state);
    }
}