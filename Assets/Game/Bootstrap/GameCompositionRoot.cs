public sealed class GameCompositionRoot
{
    public Simulation Simulation { get; }
    public TickManager TickManager { get; }

    public GameCompositionRoot(ConfigDatabase configDatabase, int tickRate)
    {
        Simulation = new Simulation(configDatabase, tickRate);
        TickManager = new TickManager(tickRate);
    }
}

