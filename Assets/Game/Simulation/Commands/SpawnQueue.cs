using System;
using System.Collections.Generic;

public sealed class SpawnQueue {
    private readonly Dictionary<EntityType, Queue<SpawnUnitRequest>> _pendingLeft = new();
    private readonly Dictionary<EntityType, Queue<SpawnUnitRequest>> _pendingRight = new();

    public SpawnQueue() {
        foreach (EntityType unitType in Enum.GetValues(typeof(EntityType))) {
            _pendingLeft[unitType] = new Queue<SpawnUnitRequest>();
            _pendingRight[unitType] = new Queue<SpawnUnitRequest>();
        }
    }

    public void Enqueue(SpawnUnitRequest request) {
        if (request.Team == Team.Left) {
            _pendingLeft[request.EntityType].Enqueue(request);
        } else if (request.Team == Team.Right) {
            _pendingRight[request.EntityType].Enqueue(request);
        }
    }

    public int Count(Team team, EntityType entityType) {
        if (team == Team.Left) {
            return _pendingLeft[entityType].Count;
        } else {
            return _pendingRight[entityType].Count;
        }
    }

    public SpawnUnitRequest Peek(Team team, EntityType entityType) {
        if (team == Team.Left) {
            return _pendingLeft[entityType].Peek();
        } else {
            return _pendingRight[entityType].Peek();
        }
    }

    public SpawnUnitRequest Dequeue(Team team, EntityType entityType) {
        if (team == Team.Left) {
            return _pendingLeft[entityType].Dequeue();
        } else {
            return _pendingRight[entityType].Dequeue();
        }
    }
}