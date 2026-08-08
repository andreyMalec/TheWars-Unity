using System;
using System.Collections.Generic;

public sealed class SpawnQueue {
    private readonly Dictionary<UnitType, Queue<SpawnUnitRequest>> _pendingLeft = new();
    private readonly Dictionary<UnitType, Queue<SpawnUnitRequest>> _pendingRight = new();

    public SpawnQueue() {
        foreach (UnitType unitType in Enum.GetValues(typeof(UnitType))) {
            _pendingLeft[unitType] = new Queue<SpawnUnitRequest>();
            _pendingRight[unitType] = new Queue<SpawnUnitRequest>();
        }
    }

    public void Enqueue(SpawnUnitRequest request) {
        if (request.Team == Team.Left) {
            _pendingLeft[request.UnitType].Enqueue(request);
        } else if (request.Team == Team.Right) {
            _pendingRight[request.UnitType].Enqueue(request);
        }
    }

    public int Count(Team team, UnitType unitType) {
        if (team == Team.Left) {
            return _pendingLeft[unitType].Count;
        } else {
            return _pendingRight[unitType].Count;
        }
    }

    public SpawnUnitRequest Peek(Team team, UnitType unitType) {
        if (team == Team.Left) {
            return _pendingLeft[unitType].Peek();
        } else {
            return _pendingRight[unitType].Peek();
        }
    }

    public SpawnUnitRequest Dequeue(Team team, UnitType unitType) {
        if (team == Team.Left) {
            return _pendingLeft[unitType].Dequeue();
        } else {
            return _pendingRight[unitType].Dequeue();
        }
    }
}