using UnityEngine;

public sealed class EconomySystem : ISystem {
    private float _secondTimer;

    public void Run(Simulation s, Frame fr) {
        var dt = fr.DeltaTime;
        _secondTimer += dt;
        if (_secondTimer < 1f) return;
        _secondTimer -= 1f;

        foreach (var pair in fr.Bases) {
            var state = pair.Value;
            var config = fr.FindConfig<BaseConfig>(state.ConfigId);
            state.Resources += config.incomePerSecond;
            // Debug.Log(
            //     $"[EconomySystem] Base (Team {state.Team}) generated {config.IncomePerTick} resources. Total: {state.Resources}");
        }
    }
}