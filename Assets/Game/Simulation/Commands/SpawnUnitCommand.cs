using UnityEngine;

public sealed class SpawnUnitCommand : ICommand
{
    private readonly int _team;
    private readonly ConfigId _unitConfigId;
    private readonly Vector2 _position;
    private readonly Vector2 _destination;

    public SpawnUnitCommand(int team, ConfigId unitConfigId, Vector2 position)
        : this(team, unitConfigId, position, position)
    {
    }

    public SpawnUnitCommand(int team, ConfigId unitConfigId, Vector2 position, Vector2 destination)
    {
        _team = team;
        _unitConfigId = unitConfigId;
        _position = position;
        _destination = destination;
    }

    public void Execute(Simulation simulation)
    {
        simulation.SpawnRequests.Enqueue(new SpawnUnitRequest
        {
            Team = _team,
            UnitConfigId = _unitConfigId,
            Position = _position,
            Destination = _destination
        });
    }
}

