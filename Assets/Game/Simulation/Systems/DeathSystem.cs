using System.Collections.Generic;
using UnityEngine;

public sealed class DeathSystem : ISystem {
    private readonly List<int> _removeBuffer = new List<int>();
    private const int CorpseTicks = 2 * Simulation.TickRate;

    public void Run(Simulation s, Frame fr) {
        while (s.ProjectileRemovalRequests.Count > 0) {
            fr.RemoveProjectile(s.ProjectileRemovalRequests.Dequeue());
        }

        _removeBuffer.Clear();
        foreach (var (id, unit) in fr.Units) {
            if (unit.Health <= 0 && unit.IsAlive) {
                unit.IsAlive = false;
                unit.DeathTick = fr.Tick;
                s.Events.Publish(new UnitEvent.DeathStarted(unit.Id));
            } else if (!unit.IsAlive && unit.DeathTick + CorpseTicks <= fr.Tick) {
                _removeBuffer.Add(id);
            }
        }

        for (var i = 0; i < _removeBuffer.Count; i++) {
            fr.RemoveUnit(_removeBuffer[i]);
            Debug.Log(
                $"[DeathSystem] Unit (ID {_removeBuffer[i]}) has been removed from the world due to zero health.");
        }

        foreach (var pair in fr.Bases) {
            if (pair.Value.Health <= 0) {
                // TODO win/lose
            }
        }
    }
}