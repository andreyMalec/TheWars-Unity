using System.Collections.Generic;
using UnityEngine;

public sealed class MovementSystem : ISystem {
    private const float StopRange = 0.1f;
    
    public void Run(Simulation simulation) {
        var world = simulation.Frame.World;
        var dt = simulation.TickDeltaTime;
        var sortedIds = new List<int>(world.Units.Keys);
        sortedIds.Sort();

        var startPositions = new Dictionary<int, Vector2>(sortedIds.Count);
        for (var i = 0; i < sortedIds.Count; i++) {
            var id = sortedIds[i];
            startPositions[id] = world.Units[id].Position;
        }

        var resolvedPositions = new Dictionary<int, Vector2>(sortedIds.Count);

        for (var i = 0; i < sortedIds.Count; i++) {
            var id = sortedIds[i];
            var state = world.Units[id];
            var config = simulation.ConfigDatabase.GetUnitConfig(state.ConfigId);
            var desired = state.Position;

            if (state.TargetEntityId != 0 &&
                world.TryGetEntityPositionAndTeam(state.TargetEntityId, out var targetPosition,
                    out _)) {
                desired = CalculateDesiredPosition(state.Position, targetPosition, config.Speed, dt, StopRange);
            } else if (state.HasDestination) {
                desired = CalculateDesiredPosition(state.Position, state.Destination, config.Speed, dt, 0f);
            }

            if (CanMoveTo(world, state.Position, desired, id, state.Size, sortedIds, startPositions, resolvedPositions)) {
                state.Position = desired;
            }

            resolvedPositions[id] = state.Position;
        }
    }

    private static bool CanMoveTo(
        World world,
        Vector2 currentPosition,
        Vector2 desiredPosition,
        int unitId,
        float unitSize,
        List<int> sortedIds,
        Dictionary<int, Vector2> startPositions,
        Dictionary<int, Vector2> resolvedPositions) {
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

            var otherRadius = world.Units[otherId].Size * 0.5f;
            var minDistance = radius + otherRadius;
            if ((desiredPosition - otherPosition).sqrMagnitude < minDistance * minDistance) {
                return false;
            }
        }

        return true;
    }

    private static Vector2 CalculateDesiredPosition(Vector2 origin, Vector2 destination, float speed, float dt, float stopRange) {
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