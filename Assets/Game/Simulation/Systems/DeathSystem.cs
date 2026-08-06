using System.Collections.Generic;
using UnityEngine;

public sealed class DeathSystem : ISystem {
    private readonly List<int> _removeBuffer = new List<int>();

    public void Run(Simulation simulation) {
        var world = simulation.Frame.World;

        while (simulation.ProjectileRemovalRequests.Count > 0) {
            world.RemoveProjectile(simulation.ProjectileRemovalRequests.Dequeue());
        }

        _removeBuffer.Clear();
        foreach (var pair in world.Units) {
            if (pair.Value.Health <= 0) {
                _removeBuffer.Add(pair.Key);
            }
        }

        for (var i = 0; i < _removeBuffer.Count; i++) {
            world.RemoveUnit(_removeBuffer[i]);
            Debug.Log(
                $"[DeathSystem] Unit (ID {_removeBuffer[i]}) has been removed from the world due to zero health.");
        }

        foreach (var pair in world.Bases) {
            if (pair.Value.Health <= 0) {
               // TODO win/lose
            }
        }
    }
}