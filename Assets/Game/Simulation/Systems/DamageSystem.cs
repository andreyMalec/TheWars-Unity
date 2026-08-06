using UnityEngine;

public sealed class DamageSystem : ISystem {
    private const float ProjectileRadius = 0.1f;

    public void Run(Simulation simulation) {
        var world = simulation.Frame.World;

        foreach (var pair in world.Projectiles) {
            var projectile = pair.Value;
            if (!world.TryGetEntityPositionTeamAndRadius(projectile.TargetEntityId, out var targetPosition, out _,
                    out var targetRadius)) {
                continue;
            }

            var hitDistance = ProjectileRadius + targetRadius;
            var hit = (targetPosition - projectile.Position).sqrMagnitude <= hitDistance * hitDistance;
            if (!hit) {
                continue;
            }

            simulation.DamageRequests.Enqueue(new DamageRequest {
                SourceEntityId = projectile.SourceEntityId,
                TargetEntityId = projectile.TargetEntityId,
                Amount = projectile.Damage
            });

            simulation.ProjectileRemovalRequests.Enqueue(projectile.Id);
        }

        while (simulation.DamageRequests.Count > 0) {
            var request = simulation.DamageRequests.Dequeue();

            if (world.TryFindUnit(request.TargetEntityId, out var unit)) {
                unit.Health -= request.Amount;
                Debug.Log($"[DamageSystem] Unit (Team {unit.Team}, Config {unit.ConfigId}) took {request.Amount} damage. Remaining health: {unit.Health}");
                continue;
            }

            if (world.TryFindTurret(request.TargetEntityId, out var turret)) {
                turret.Health -= request.Amount;
                continue;
            }

            if (world.TryFindBase(request.TargetEntityId, out var baseState)) {
                baseState.Health -= request.Amount;
            }
        }
    }
}