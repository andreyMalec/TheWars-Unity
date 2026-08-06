using UnityEngine;

public sealed class EconomySystem : ISystem {
    public void Run(Simulation simulation) {
        foreach (var pair in simulation.Frame.World.Bases) {
            var state = pair.Value;
            var config = simulation.ConfigDatabase.GetBaseConfig(state.ConfigId);
            state.Resources += config.IncomePerTick;
            // Debug.Log(
            //     $"[EconomySystem] Base (Team {state.Team}) generated {config.IncomePerTick} resources. Total: {state.Resources}");
        }
    }
}