using UnityEngine;

public sealed class DamageSystem : ISystem {

    public void Run(Simulation s, Frame fr) {
        foreach (var pair in fr.Projectiles) {
            var projectile = pair.Value;
            if (!fr.TryGetEntityPositionTeamAndRadius(projectile.TargetEntityId, out var targetPosition, out _,
                    out var targetRadius)) {
                continue;
            }

            var config = fr.FindConfig<ProjectileConfig>(projectile.ConfigId);
            var hitDistance = config.radius + targetRadius;
            var hit = (targetPosition - projectile.Position).sqrMagnitude <= hitDistance * hitDistance;
            if (!hit) {
                continue;
            }

            s.DamageRequests.Enqueue(new DamageRequest {
                SourceEntityId = projectile.SourceEntityId,
                TargetEntityId = projectile.TargetEntityId,
                Amount = projectile.Damage
            });

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