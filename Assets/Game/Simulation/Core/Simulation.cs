using System.Collections.Generic;

public sealed class Simulation
{
    private readonly List<ISystem> _systems = new List<ISystem>();

    public Frame Frame { get; }
    public ConfigDatabase ConfigDatabase { get; }
    public CommandQueue CommandQueue { get; }

    public readonly Queue<SpawnUnitRequest> SpawnRequests = new Queue<SpawnUnitRequest>();
    public readonly Queue<BuildTurretRequest> BuildRequests = new Queue<BuildTurretRequest>();
    public readonly Queue<UpgradeBaseRequest> UpgradeRequests = new Queue<UpgradeBaseRequest>();

    public Simulation(ConfigDatabase configDatabase)
    {
        Frame = new Frame();
        ConfigDatabase = configDatabase;
        CommandQueue = new CommandQueue();

        _systems.Add(new BuildSystem());
        _systems.Add(new SpawnSystem());
        _systems.Add(new BaseUpgradeSystem());
    }

    public void EnqueueCommand(ICommand command)
    {
        CommandQueue.Enqueue(command);
    }

    public void Tick()
    {
        CommandQueue.ExecuteAll(this);

        for (var i = 0; i < _systems.Count; i++)
        {
            _systems[i].Run(this);
        }

        Frame.Tick++;
    }
}

