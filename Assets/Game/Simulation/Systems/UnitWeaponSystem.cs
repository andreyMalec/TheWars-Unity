using UnityEngine;

public sealed class UnitWeaponSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        foreach (var pair in fr.Units) {
            var unit = pair.Value;
            if (!unit.IsAlive) continue;
            var config = fr.FindConfig<UnitConfig>(unit.ConfigId);

            // 1. Выполнить уже начатую атаку
            if (unit.Attack.ExecuteTick > 0 && fr.Tick >= unit.Attack.ExecuteTick) {
                ExecuteAttack(s, fr, unit, config);
                unit.Attack.ExecuteTick = 0;
                AttackTicks[] attacks;
                switch (unit.Attack.AttackType) {
                    case AttackType.WalkingRanged:
                        attacks = config.attackTicks.walkingRanged;
                        break;
                    case AttackType.StandingRanged:
                        attacks = config.attackTicks.standingRanged;
                        break;
                    default:
                        attacks = config.attackTicks.standingMelee;
                        break;
                }

                unit.Attack.AttackIndex++;
                var attack = attacks[unit.Attack.AttackIndex];
                if (attack.type == AttackTickType.Recovery) {
                    unit.Attack.RecoveryTick = fr.Tick + attack.value;
                    unit.Attack.AttackIndex = 0;
                }

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

            unit.LastTargetPosition = targetPosition;

            var attackTicks = config.attackTicks.standingMelee;
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
                        attackTicks = config.attackTicks.walkingRanged;
                    } else {
                        attackType = AttackType.StandingRanged;
                        attackTicks = config.attackTicks.standingRanged;
                    }
                }
            }

            // Начинаем атаку
            unit.Attack.ExecuteTick = fr.Tick + attackTicks[unit.Attack.AttackIndex].value;
            unit.Attack.AttackType = attackType;
            s.Events.Publish(new UnitEvent.AttackStarted(unit.Id, attackType));
        }
    }

    private void ExecuteAttack(
        Simulation s,
        Frame fr,
        UnitState unit,
        UnitConfig config
    ) {
        fr.TryGetEntityPosition(unit.TargetEntityId, out var targetPosition);
        if (targetPosition == Vector2.zero)
            targetPosition = unit.LastTargetPosition;
        if (config.attackType == UnitAttackType.Ranged) {
            var projectilePosition = UnitColliderUtility.ToWorldPoint(
                config.projectilePosition,
                unit.Position,
                UnitColliderUtility.IsMirrored(unit.Direction));

            var projConfig = fr.FindConfig<ProjectileConfig>(config.projectileId);
            var direction = (targetPosition - projectilePosition).normalized;
            var velocity = Vector2.zero;

            if (projConfig.type == ProjectileType.Ballistic) {
                var targetVelocity = BallisticsUtility.ResolveTargetVelocity(fr, unit.TargetEntityId);
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
                Team = unit.Team,
                SourceEntityId = unit.Id,
                TargetEntityId = unit.TargetEntityId,
                Damage = config.damage,
                Position = projectilePosition,
                Direction = direction,
                Velocity = velocity,
                Speed = projConfig.speed,
                Type = projConfig.type,
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