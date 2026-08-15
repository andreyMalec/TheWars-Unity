using UnityEngine;

public sealed class WeaponSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        var dt = fr.DeltaTime;

        foreach (var pair in fr.Units) {
            var unit = pair.Value;
            if (!unit.IsAlive) continue;
            var config = fr.FindConfig<UnitConfig>(unit.ConfigId);

            // 1. Выполнить уже начатую атаку
            if (unit.Attack.ExecuteTick > 0 && fr.Tick >= unit.Attack.ExecuteTick) {
                ExecuteAttack(s, fr, unit, config);
                unit.Attack.ExecuteTick = 0;
                var recovery = 0;
                switch (unit.Attack.AttackType) {
                    case AttackType.StandingMelee:
                        recovery = config.attackTicks.recoveryStandingMelee;
                        break;
                    case AttackType.WalkingRanged:
                        recovery = config.attackTicks.recoveryWalkingRanged;
                        break;
                    case AttackType.StandingRanged:
                        recovery = config.attackTicks.recoveryStandingRanged;
                        break;
                }

                unit.Attack.RecoveryTick = fr.Tick + recovery;
                continue;
            }

            // 2. Если сейчас готова новая атака
            if (unit.Attack.ExecuteTick > 0) continue;
            if (unit.Attack.RecoveryTick > fr.Tick) continue;
            if (unit.TargetEntityId == 0) continue;
            if (!fr.TryGetEntityPositionAndTeam(unit.TargetEntityId, out var targetPosition, out _)) {
                unit.TargetEntityId = 0;
                continue;
            }

            var attackTicks = config.attackTicks.executeStandingMelee;
            AttackType attackType = AttackType.StandingMelee;
            var meleeRange = config.attackRangeMelee * config.attackRangeMelee;
            var toTarget = (targetPosition - unit.Position).sqrMagnitude;
            if (config.attackType == UnitAttackType.Melee) {
                if (toTarget > meleeRange)
                    continue;
            } else {
                var rangedRange = config.attackRangeRanged * config.attackRangeRanged;
                if (toTarget > rangedRange)
                    continue;

                if (toTarget <= rangedRange && toTarget > meleeRange) {
                    if (unit.IsMoving) {
                        attackType = AttackType.WalkingRanged;
                        attackTicks = config.attackTicks.executeWalkingRanged;
                    } else {
                        attackType = AttackType.StandingRanged;
                        attackTicks = config.attackTicks.executeStandingRanged;
                    }
                }
            }

            // Начинаем атаку
            unit.Attack.ExecuteTick = fr.Tick + attackTicks;
            unit.Attack.AttackType = attackType;
            s.Events.Publish(new UnitEvent.AttackStarted(unit.Id, attackType));
        }

        foreach (var pair in fr.Turrets) {
            var turret = pair.Value;
            var config = fr.FindConfig<TurretConfig>(turret.ConfigId);

            if (turret.Cooldown > 0f) {
                turret.Cooldown -= dt;
            }

            if (turret.TargetEntityId == 0) {
                continue;
            }

            if (!fr.TryGetEntityPositionAndTeam(turret.TargetEntityId, out var targetPosition, out _)) {
                continue;
            }

            var inRange = (targetPosition - turret.Position).sqrMagnitude <= config.attackRange * config.attackRange;
            var direction = (targetPosition - turret.Position).normalized;
            if (config.rotateToTarget)
                turret.Rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            else
                turret.Rotation = 0f;
            if (!inRange || turret.Cooldown > 0f) {
                continue;
            }

            var projectilePosition = UnitColliderUtility.ToWorldPoint(
                config.projectilePosition,
                turret.Position,
                UnitColliderUtility.IsMirrored(turret.Team == Team.Left ? UnitDirection.Right : UnitDirection.Left));
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
                Lifetime = 10f
            };

            fr.AddProjectile(state);
            turret.Cooldown = config.attackInterval > 0f ? config.attackInterval : 1f;
        }
    }

    private void ExecuteAttack(
        Simulation s,
        Frame fr,
        UnitState unit,
        UnitConfig config
    ) {
        fr.TryGetEntityPositionAndTeam(unit.TargetEntityId, out var targetPosition, out _);
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
            s.Events.Publish(new UnitEvent.DamageTaken(unit.TargetEntityId, targetPosition));
        }
    }
}