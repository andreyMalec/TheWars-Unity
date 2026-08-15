using UnityEngine;

public sealed class DamageSystem : ISystem {
    public void Run(Simulation s, Frame fr) {
        foreach (var pair in fr.Projectiles) {
            var projectile = pair.Value;
            if (!fr.TryGetUnit(projectile.TargetEntityId, out var unit, out var unitConfig)) {
                continue;
            }

            var targetRadius = UnitColliderUtility.GetRadius(unitConfig);
            var targetPosition = unit.Position;
            var config = fr.FindConfig<ProjectileConfig>(projectile.ConfigId);
            var hitDistance = config.radius + targetRadius;
            var toTarget = targetPosition - projectile.Position;
            var hit = (toTarget).sqrMagnitude <= hitDistance * hitDistance;
            if (!hit) {
                continue;
            }

            s.DamageRequests.Enqueue(new DamageRequest {
                SourceEntityId = projectile.SourceEntityId,
                TargetEntityId = projectile.TargetEntityId,
                Amount = projectile.Damage
            });

            UnitColliderUtility.RayPolygonIntersection(
                projectile.Position,
                toTarget,
                UnitColliderUtility.IsMirrored(unit.Direction),
                targetPosition, unitConfig.collider, out var hitPoint);
            s.Events.Publish(new UnitEvent.DamageTaken(projectile.TargetEntityId,
                hitPoint //+ toTarget.normalized * distance
                ));
            s.ProjectileRemovalRequests.Enqueue(projectile.Id);
        }

        while (s.DamageRequests.Count > 0) {
            var request = s.DamageRequests.Dequeue();

            if (fr.TryFindUnit(request.TargetEntityId, out var unit)) {
                unit.Health -= request.Amount;
                Debug.Log(
                    $"[DamageSystem] Unit (Team {unit.Team}, Config {unit.ConfigId}) took {request.Amount} damage. Remaining health: {unit.Health}");
                continue;
            }

            if (fr.TryFindBase(request.TargetEntityId, out var baseState)) {
                baseState.Health -= request.Amount;
            }
        }
    }
}