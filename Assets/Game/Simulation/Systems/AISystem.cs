public sealed class AISystem : ISystem {
    private UnitConfig _melee;
    private UnitConfig _ranged;

    public void Init(Simulation simulation) {
        _melee = simulation.ConfigDatabase.GetUnitConfig(0);
        _ranged = simulation.ConfigDatabase.GetUnitConfig(1);
    }

    public void Run(Simulation simulation) {
        if (simulation.Frame.World.TryFindBaseByTeam(1, out var baseState1)) {
            if (baseState1.Resources >= _melee.Cost) {
                simulation.EnqueueCommand(new SpawnUnitCommand(1, _melee.Id, baseState1.Position));
            }
        }

        if (simulation.Frame.World.TryFindBaseByTeam(2, out var baseState2)) {
            if (baseState2.Resources >= _ranged.Cost) {
                simulation.EnqueueCommand(new SpawnUnitCommand(2, _ranged.Id, baseState2.Position));
            }
        }
    }
}