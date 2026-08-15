using System;
using System.Collections.Generic;
using System.Linq;

public sealed class SpawnQueue {
    private readonly List<SpawnUnitRequest> _pendingLeft = new();
    private readonly List<SpawnUnitRequest> _pendingRight = new();

    public SpawnQueue() {
    }

    public void Enqueue(SpawnUnitRequest request) {
        if (request.Team == Team.Left) {
            _pendingLeft.Add(request);
        } else if (request.Team == Team.Right) {
            _pendingRight.Add(request);
        }
    }

    public int Count(Team team) {
        if (team == Team.Left) {
            return _pendingLeft.Count;
        } else {
            return _pendingRight.Count;
        }
    }

    public int Count(Team team, EntityType entityType) {
        if (team == Team.Left) {
            return _pendingLeft.Count(request => request.EntityType == entityType);
        } else {
            return _pendingRight.Count(request => request.EntityType == entityType);
        }
    }

    public SpawnUnitRequest Peek(Team team) {
        if (team == Team.Left) {
            return _pendingLeft.First();
        } else {
            return _pendingRight.First();
        }
    }

    public SpawnUnitRequest Dequeue(Team team) {
        if (team == Team.Left) {
            var request = _pendingLeft.First();
            _pendingLeft.RemoveAt(0);
            return request;
        } else {
            var request = _pendingRight.First();
            _pendingRight.RemoveAt(0);
            return request;
        }
    }
}