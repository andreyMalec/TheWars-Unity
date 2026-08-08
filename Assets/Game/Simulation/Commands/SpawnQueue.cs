using System;
using System.Collections.Generic;

public sealed class SpawnQueue {
    private readonly Queue<SpawnUnitRequest> _pendingLeft = new Queue<SpawnUnitRequest>();
    private readonly Queue<SpawnUnitRequest> _pendingRight = new Queue<SpawnUnitRequest>();

    public void Enqueue(SpawnUnitRequest request) {
        if (request.Team == Team.Left) {
            _pendingLeft.Enqueue(request);
        } else if (request.Team == Team.Right) {
            _pendingRight.Enqueue(request);
        }
    }

    public int Count() {
        return _pendingLeft.Count + _pendingRight.Count;
    }

    public int Count(Team team) {
        if (team == Team.Left) {
            return _pendingLeft.Count;
        } else {
            return _pendingRight.Count;
        }
    }

    public SpawnUnitRequest Peek(Team team) {
        if (team == Team.Left) {
            return _pendingLeft.Peek();
        } else {
            return _pendingRight.Peek();
        }
    }

    public SpawnUnitRequest Dequeue(Team team) {
        if (team == Team.Left) {
            return _pendingLeft.Dequeue();
        } else {
            return _pendingRight.Dequeue();
        }
    }
}