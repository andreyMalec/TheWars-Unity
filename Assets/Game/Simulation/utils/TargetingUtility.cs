using UnityEngine;

public static class TargetingUtility {
    public static bool FindNearestEnemy(Frame frame, int team, Vector2 origin, out Damageable damageable) {
        var bestDistanceSqr = float.MaxValue;
        Damageable best = null;

        foreach (var pair in frame.Units) {
            var unit = pair.Value;
            if (unit.Team == team || unit.Health <= 0) {
                continue;
            }

            var distanceSqr = (unit.Position - origin).sqrMagnitude;
            if (distanceSqr < bestDistanceSqr) {
                bestDistanceSqr = distanceSqr;
                best = unit;
            }
        }

        foreach (var pair in frame.Bases) {
            var baseState = pair.Value;
            if (baseState.Team == team || baseState.Health <= 0) {
                continue;
            }

            var distanceSqr = (baseState.Position - origin).sqrMagnitude;
            if (distanceSqr < bestDistanceSqr) {
                bestDistanceSqr = distanceSqr;
                best = baseState;
            }
        }

        damageable = best;
        return best != null;
    }

    public static bool FindEnemyInDirection(
        Frame frame, int team, Vector2 origin, Vector2 direction, out Damageable damageable
    ) {
        var bestDistanceSqr = float.MaxValue;
        Damageable best = null;
 
        foreach (var pair in frame.Units) {
            var unit = pair.Value;
            if (unit.Team == team || unit.Health <= 0) {
                continue;
            }

            var distanceSqr = (unit.Position - origin).sqrMagnitude;
            if (distanceSqr < bestDistanceSqr) {
                var toOther = unit.Position - origin;
                if (Vector2.Dot(direction, toOther) <= 0f) {
                    continue;
                }
                
                bestDistanceSqr = distanceSqr;
                best = unit;
            }
        }

        damageable = best;
        return best != null;
    }
}