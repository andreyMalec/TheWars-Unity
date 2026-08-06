using UnityEngine;

public sealed class SpawnUnitCommand : ICommand
{
    private readonly int _team;
    private readonly int _unitConfigId;
    private readonly Vector2 _position;

    public SpawnUnitCommand(int team, int unitConfigId, Vector2 position)
    {
        _team = team;
        _unitConfigId = unitConfigId;
        _position = position;
    }

    public void Execute(Simulation simulation)
    {
        simulation.SpawnRequests.Enqueue(new SpawnUnitRequest
        {
            Team = _team,
            UnitConfigId = _unitConfigId,
            Position = _position
        });
    }
}

