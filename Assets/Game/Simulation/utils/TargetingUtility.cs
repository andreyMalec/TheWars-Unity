using UnityEngine;

public static class TargetingUtility {
    public static int FindNearestEnemy(Frame frame, int team, Vector2 origin) {
        var bestDistanceSqr = float.MaxValue;
        var bestId = 0;

        foreach (var pair in frame.Units) {
            var unit = pair.Value;
            if (unit.Team == team || unit.Health <= 0) {
                continue;
            }

            var distanceSqr = (unit.Position - origin).sqrMagnitude;
            if (distanceSqr < bestDistanceSqr) {
                bestDistanceSqr = distanceSqr;
                bestId = unit.Id;
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
                bestId = baseState.Id;
            }
        }

        return bestId;
    }
}