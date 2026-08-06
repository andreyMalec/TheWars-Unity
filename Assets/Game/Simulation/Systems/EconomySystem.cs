using UnityEngine;

public sealed class EconomySystem : ISystem {
    private float _secondTimer;

    public void Run(Simulation s, Frame fr) {
        var dt = s.TickDeltaTime;
        _secondTimer += dt;
        if (_secondTimer < 1f) return;
        _secondTimer -= 1f;

        foreach (var pair in fr.Bases) {
            var state = pair.Value;
            var config = s.ConfigDatabase.GetBaseConfig(state.ConfigId);
            state.Resources += config.IncomePerSecond;
            // Debug.Log(
            //     $"[EconomySystem] Base (Team {state.Team}) generated {config.IncomePerTick} resources. Total: {state.Resources}");
        }
    }
}