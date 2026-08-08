public sealed class SpecialWeaponCommand : ICommand {
    private readonly Team _team;

    public SpecialWeaponCommand(Team team) {
        _team = team;
    }

    public void Execute(Simulation simulation) {
        simulation.SpecialWeaponRequests.Enqueue(new SpecialWeaponRequest {
            Team = _team
        });
    }
}