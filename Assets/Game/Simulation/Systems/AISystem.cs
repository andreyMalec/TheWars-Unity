public sealed class AISystem : ISystem {
    private UnitConfig _melee;
    private UnitConfig _ranged;

    public void Init(Simulation simulation) {
        _melee = simulation.Frame.FindConfig<UnitConfig>(Epoch.StoneAge, EntityType.Type1);
        _ranged = simulation.Frame.FindConfig<UnitConfig>(Epoch.StoneAge, EntityType.Type2);
    }

    public void Run(Simulation s, Frame fr) {
        // if (fr.TryFindBaseByTeam(Team.Left, out var baseState1)) {
        //     if (baseState1.Resources >= _melee.cost) {
        //         s.EnqueueCommand(new SpawnUnitCommand(Team.Left, _melee));
        //     }
        // }

        if (fr.TryFindBaseByTeam(Team.Right, out var baseState2)) {
            if (baseState2.Resources >= _ranged.cost) {
                s.EnqueueCommand(new SpawnUnitCommand(Team.Right, _ranged));
            }
        }
    }
}