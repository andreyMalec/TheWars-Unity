public sealed class UpgradeBaseCommand : ICommand {
    private readonly Team _team;

    public UpgradeBaseCommand(Team team) {
        _team = team;
    }

    public void Execute(Simulation simulation) {
        simulation.UpgradeRequests.Enqueue(new UpgradeBaseRequest {
            Team = _team
        });
    }
}