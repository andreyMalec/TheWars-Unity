public sealed class UpgradeBaseCommand : ICommand
{
    private readonly int _baseEntityId;

    public UpgradeBaseCommand(int baseEntityId)
    {
        _baseEntityId = baseEntityId;
    }

    public void Execute(Simulation simulation)
    {
        simulation.UpgradeRequests.Enqueue(new UpgradeBaseRequest
        {
            BaseEntityId = _baseEntityId
        });
    }
}

