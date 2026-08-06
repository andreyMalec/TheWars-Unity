using UnityEngine;

public sealed class SimulationRunner : MonoBehaviour
{
    private Simulation _simulation;
    private TickManager _tickManager;

    public void Initialize(Simulation simulation, TickManager tickManager)
    {
        _simulation = simulation;
        _tickManager = tickManager;
    }

    private void FixedUpdate()
    {
        _tickManager.Advance(Time.fixedDeltaTime, _simulation.Tick);
    }
}

