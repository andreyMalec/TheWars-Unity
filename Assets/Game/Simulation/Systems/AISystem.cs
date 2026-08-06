public sealed class AISystem : ISystem {
    private UnitConfig _unit;

    public void Init(Simulation simulation) {
        _unit = simulation.ConfigDatabase.GetUnitConfig(0);
    }

    public void Run(Simulation simulation) {
        if (simulation.Frame.World.TryFindBaseByTeam(1, out var baseState1)) {
            if (baseState1.Resources >= _unit.Cost) {
                simulation.EnqueueCommand(new SpawnUnitCommand(1, _unit.ConfigId, baseState1.Position));
            }
        }

        if (simulation.Frame.World.TryFindBaseByTeam(2, out var baseState2)) {
            if (baseState2.Resources >= _unit.Cost * 2) {
                baseState2.Resources -= _unit.Cost;
                simulation.EnqueueCommand(new SpawnUnitCommand(2, _unit.ConfigId, baseState2.Position));
            }
        }
    }
}