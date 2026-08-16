using System;
using UnityEngine;

public sealed class TurretWeaponSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        foreach (var pair in fr.Turrets) {
            var turret = pair.Value;
            var config = fr.FindConfig<TurretConfig>(turret.ConfigId);
            RotateToTarget(turret, config);

            // 1. Выполнить уже начатую атаку
            if (TryAttack(s, fr, turret, config)) continue;

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

    private bool TryAttack(Simulation s, Frame fr, TurretState turret, TurretConfig config) {
        if (turret.ExecuteTick > 0 && fr.Tick >= turret.ExecuteTick) {
            ExecuteAttack(s, fr, turret, config);
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

    private void RotateToTarget(TurretState turret, TurretConfig config) {
        if (turret.LastTargetPosition == Vector2.zero) return;
        var toTarget = turret.LastTargetPosition - turret.Position;
        var direction = toTarget.normalized;
        if (config.rotateToTarget)
            turret.Rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        else
            turret.Rotation = 0f;
    }

    private void ExecuteAttack(
        Simulation s,
        Frame fr,
        TurretState turret,
        TurretConfig config
    ) {
        fr.TryGetEntityPosition(turret.TargetEntityId, out var targetPosition);
        if (targetPosition == Vector2.zero)
            targetPosition = turret.LastTargetPosition;
        var direction = (targetPosition - turret.Position).normalized;

        var projectilePosition = UnitColliderUtility.ToWorldPoint(
            config.projectilePositions[turret.AttackIndex],
            turret.Position,
            turret.Team == Team.Right);

        var projConfig = fr.FindConfig<ProjectileConfig>(config.projectileId);

        var state = new ProjectileState {
            Id = fr.GenerateEntityId(),
            ConfigId = projConfig.id,
            Team = turret.Team,
            SourceEntityId = turret.Id,
            TargetEntityId = turret.TargetEntityId,
            Damage = config.damage,
            Position = projectilePosition,
            Direction = direction,
            Speed = projConfig.speed,
            Lifetime = 5f
        };

        fr.AddProjectile(state);
    }
}