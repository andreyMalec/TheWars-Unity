using System.Collections.Generic;
using UnityEngine;

public sealed class MovementSystem : ISystem {
    private const float StopRange = 0.1f;

    public void Run(Simulation s, Frame fr) {
        var dt = s.TickDeltaTime;
        var sortedIds = new List<int>(fr.Units.Keys);
        sortedIds.Sort();

        var startPositions = new Dictionary<int, Vector2>(sortedIds.Count);
        for (var i = 0; i < sortedIds.Count; i++) {
            var id = sortedIds[i];
            startPositions[id] = fr.Units[id].Position;
        }

        var resolvedPositions = new Dictionary<int, Vector2>(sortedIds.Count);

        for (var i = 0; i < sortedIds.Count; i++) {
            var id = sortedIds[i];
            var state = fr.Units[id];
            var config = s.ConfigDatabase.GetUnitConfig(state.ConfigId);

            var targetPosition = fr.GetEnemyBasePosition(state);
            var desired = CalculateDesiredPosition(state.Position, targetPosition, config.Speed, dt, StopRange);

            if (CanMoveTo(fr, state.Position, desired, id, state.Size, sortedIds, startPositions, resolvedPositions)) {
                state.Position = desired;
            }

            resolvedPositions[id] = state.Position;
        }
    }

    private static bool CanMoveTo(
        Frame frame,
        Vector2 currentPosition,
        Vector2 desiredPosition,
        int unitId,
        float unitSize,
        List<int> sortedIds,
        Dictionary<int, Vector2> startPositions,
        Dictionary<int, Vector2> resolvedPositions
    ) {
        var movementDirection = desiredPosition - currentPosition;
        var hasMovement = movementDirection.sqrMagnitude > 0.000001f;
        var radius = unitSize * 0.5f;

        for (var i = 0; i < sortedIds.Count; i++) {
            var otherId = sortedIds[i];
            if (otherId == unitId) {
                continue;
            }

            var otherPosition = resolvedPositions.ContainsKey(otherId)
                ? resolvedPositions[otherId]
                : startPositions[otherId];

            if (hasMovement) {
                var toOther = otherPosition - currentPosition;
                if (Vector2.Dot(movementDirection, toOther) <= 0f) {
                    continue;
                }
            }

            var otherRadius = frame.Units[otherId].Size * 0.5f;
            var minDistance = radius + otherRadius;
            if ((desiredPosition - otherPosition).sqrMagnitude < minDistance * minDistance) {
                return false;
            }
        }

        return true;
    }

    private static Vector2 CalculateDesiredPosition(
        Vector2 origin, Vector2 destination, float speed, float dt, float stopRange
    ) {
        var delta = destination - origin;
        var distance = delta.magnitude;

        if (distance <= stopRange) {
            return origin;
        }

        var step = speed * dt;
        if (step >= distance - stopRange) {
            return destination - (delta / distance) * stopRange;
        }

        return origin + (delta / distance) * step;
    }
}