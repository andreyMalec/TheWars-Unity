using System.Collections.Generic;
using UnityEngine;

public sealed class MovementSystem : ISystem {
    private const float StopRange = 0.1f;

    public void Run(Simulation s, Frame fr) {
        var dt = fr.DeltaTime;
        var sortedIds = new List<int>(fr.Units.Keys);
        sortedIds.Sort();

        var unitRadii = new Dictionary<int, float>(sortedIds.Count);
        var unitColliders = new Dictionary<int, Vector2[]>(sortedIds.Count);
        var unitDirections = new Dictionary<int, UnitDirection>(sortedIds.Count);
        for (var i = 0; i < sortedIds.Count; i++) {
            var id = sortedIds[i];
            var state = fr.Units[id];
            var config = fr.FindConfig<UnitConfig>(state.ConfigId);
            unitRadii[id] = UnitColliderUtility.GetRadius(config);
            unitColliders[id] = config.collider;
            unitDirections[id] = state.Direction;
        }

        var startPositions = new Dictionary<int, Vector2>(sortedIds.Count);
        for (var i = 0; i < sortedIds.Count; i++) {
            var id = sortedIds[i];
            startPositions[id] = fr.Units[id].Position;
        }

        var resolvedPositions = new Dictionary<int, Vector2>(sortedIds.Count);

        for (var i = 0; i < sortedIds.Count; i++) {
            var id = sortedIds[i];
            var state = fr.Units[id];
            var config = fr.FindConfig<UnitConfig>(state.ConfigId);

            var targetPosition = fr.GetEnemyBasePosition(state);
            var desired = CalculateDesiredPosition(state.Position, targetPosition, config.speed, dt, StopRange);
            var direction = UnitColliderUtility.ResolveDirection(state.Direction, desired.x - state.Position.x);
            state.Direction = direction;
            unitDirections[id] = direction;

            if (CanMoveTo(fr, state.Team, state.Position, desired, id, unitRadii[id], unitColliders[id], direction, sortedIds, unitRadii, unitColliders, unitDirections, startPositions,
                    resolvedPositions)) {
                state.Position = desired;
            }

            resolvedPositions[id] = state.Position;
        }
    }

    private static bool CanMoveTo(
        Frame frame,
        Team unitTeam,
        Vector2 currentPosition,
        Vector2 desiredPosition,
        int unitId,
        float unitRadius,
        Vector2[] unitCollider,
        UnitDirection unitDirection,
        List<int> sortedIds,
        Dictionary<int, float> unitRadii,
        Dictionary<int, Vector2[]> unitColliders,
        Dictionary<int, UnitDirection> unitDirections,
        Dictionary<int, Vector2> startPositions,
        Dictionary<int, Vector2> resolvedPositions
    ) {
        if (IsBlockedByEnemyBaseBounds(frame, unitTeam, currentPosition, desiredPosition)) {
            return false;
        }

        var movementDirection = desiredPosition - currentPosition;
        var hasMovement = movementDirection.sqrMagnitude > 0.000001f;
        var radius = unitRadius;

        for (var i = 0; i < sortedIds.Count; i++) {
            var otherId = sortedIds[i];
            if (otherId == unitId) {
                continue;
            }

            var otherPosition = resolvedPositions.TryGetValue(otherId, out var position)
                ? position
                : startPositions[otherId];

            if (hasMovement) {
                var toOther = otherPosition - currentPosition;
                if (Vector2.Dot(movementDirection, toOther) <= 0f) {
                    continue;
                }
            }

            var otherRadius = unitRadii[otherId];
            var minDistance = radius + otherRadius;
            if ((desiredPosition - otherPosition).sqrMagnitude >= minDistance * minDistance) {
                continue;
            }

            var otherCollider = unitColliders[otherId];
            var otherDirection = unitDirections[otherId];
            if (UnitColliderUtility.PolygonsOverlap(
                    unitCollider,
                    desiredPosition,
                    UnitColliderUtility.IsMirrored(unitDirection),
                    otherCollider,
                    otherPosition,
                    UnitColliderUtility.IsMirrored(otherDirection)
                )) {
                return false;
            }
        }

        return true;
    }

    private static bool IsBlockedByEnemyBaseBounds(
        Frame frame,
        Team unitTeam,
        Vector2 currentPosition,
        Vector2 desiredPosition
    ) {
        foreach (var pair in frame.Bases) {
            var baseState = pair.Value;
            if (baseState.Team == unitTeam) {
                continue;
            }

            var baseConfig = frame.FindConfig<BaseConfig>(baseState.ConfigId);
            if (BaseBoundsUtility.ContainsPoint(baseState, baseConfig, currentPosition)) {
                continue;
            }

            if (BaseBoundsUtility.SegmentIntersects(baseState, baseConfig, currentPosition, desiredPosition)) {
                return true;
            }
        }

        return false;
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