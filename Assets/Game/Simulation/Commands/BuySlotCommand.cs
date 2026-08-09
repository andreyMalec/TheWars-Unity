
public sealed class BuySlotCommand : ICommand {
    private readonly Team _team;

    public BuySlotCommand(Team team) {
        _team = team;
    }

    public void Execute(Simulation simulation) {
        simulation.BuySlotRequests.Enqueue(new BuySlotRequest {
            Team = _team,
        });
    }
}