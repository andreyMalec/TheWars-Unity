public sealed class AISystem : ISystem {
    private UnitConfig _melee;
    private UnitConfig _ranged;

    public void Init(Simulation simulation) {
        _melee = simulation.Frame.FindConfig<UnitConfig>(0);
        _ranged = simulation.Frame.FindConfig<UnitConfig>(1);
    }

    public void Run(Simulation s, Frame fr) {
        if (fr.TryFindBaseByTeam(1, out var baseState1)) {
            if (baseState1.Resources >= _melee.Cost) {
                s.EnqueueCommand(new SpawnUnitCommand(1, _melee.Id, baseState1.Position));
            }
        }

        if (fr.TryFindBaseByTeam(2, out var baseState2)) {
            if (baseState2.Resources >= _ranged.Cost) {
                s.EnqueueCommand(new SpawnUnitCommand(2, _ranged.Id, baseState2.Position));
            }
        }
    }
}