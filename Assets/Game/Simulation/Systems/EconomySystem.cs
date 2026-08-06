using UnityEngine;

public sealed class EconomySystem : ISystem {
    private float _secondTimer;

    public void Run(Simulation simulation) {
        var dt = simulation.TickDeltaTime;
        _secondTimer += dt;
        if (_secondTimer < 1f) return;
        _secondTimer -= 1f;

        foreach (var pair in simulation.Frame.World.Bases) {
            var state = pair.Value;
            var config = simulation.ConfigDatabase.GetBaseConfig(state.ConfigId);
            state.Resources += config.IncomePerSecond;
            // Debug.Log(
            //     $"[EconomySystem] Base (Team {state.Team}) generated {config.IncomePerTick} resources. Total: {state.Resources}");
        }
    }
}