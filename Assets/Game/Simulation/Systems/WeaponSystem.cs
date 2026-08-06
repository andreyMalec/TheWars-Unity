using UnityEngine;

public sealed class WeaponSystem : ISystem {
    public void Run(Simulation simulation) {
        var world = simulation.Frame.World;
        var dt = simulation.TickDeltaTime;

        foreach (var pair in world.Units) {
            var unit = pair.Value;
            var config = simulation.ConfigDatabase.GetUnitConfig(unit.ConfigId);

            if (unit.Cooldown > 0f) {
                unit.Cooldown -= dt;
            }

            if (unit.TargetEntityId == 0) {
                continue;
            }

            if (!world.TryGetEntityPositionAndTeam(unit.TargetEntityId, out var targetPosition, out _)) {
                continue;
            }

            var inRange = (targetPosition - unit.Position).sqrMagnitude <= config.AttackRange * config.AttackRange;
            if (!inRange || unit.Cooldown > 0f) {
                continue;
            }

            SpawnProjectile(simulation, unit.Team, unit.Id, unit.TargetEntityId, unit.Position, config.Damage,
                config.ProjectileSpeed);
            unit.Cooldown = config.AttackInterval > 0f ? config.AttackInterval : 1f;
        }

        foreach (var pair in world.Turrets) {
            var turret = pair.Value;
            var config = simulation.ConfigDatabase.GetTurretConfig(turret.ConfigId);

            if (turret.Cooldown > 0f) {
                turret.Cooldown -= dt;
            }

            if (turret.TargetEntityId == 0) {
                continue;
            }

            if (!world.TryGetEntityPositionAndTeam(turret.TargetEntityId, out var targetPosition, out _)) {
                continue;
            }

            var inRange = (targetPosition - turret.Position).sqrMagnitude <= config.AttackRange * config.AttackRange;
            if (!inRange || turret.Cooldown > 0f) {
                continue;
            }

            SpawnProjectile(simulation, turret.Team, turret.Id, turret.TargetEntityId, turret.Position, config.Damage,
                config.ProjectileSpeed);
            turret.Cooldown = config.AttackInterval > 0f ? config.AttackInterval : 1f;
        }
    }

    private static void SpawnProjectile(
        Simulation simulation,
        int team,
        int sourceEntityId,
        int targetEntityId,
        Vector2 position,
        int damage,
        float speed
    ) {
        var world = simulation.Frame.World;

        var state = new ProjectileState {
            Id = world.GenerateEntityId(),
            Team = team,
            SourceEntityId = sourceEntityId,
            TargetEntityId = targetEntityId,
            Damage = damage,
            Position = position,
            Direction = Vector2.right,
            Speed = speed > 0f ? speed : 8f,
            Lifetime = 5f
        };

        world.AddProjectile(state);
    }
}