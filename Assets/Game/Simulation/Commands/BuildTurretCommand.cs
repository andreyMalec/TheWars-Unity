using UnityEngine;

public sealed class BuildTurretCommand : ICommand
{
    private readonly int _team;
    private readonly int _turretConfigId;
    private readonly Vector2 _position;

    public BuildTurretCommand(int team, int turretConfigId, Vector2 position)
    {
        _team = team;
        _turretConfigId = turretConfigId;
        _position = position;
    }

    public void Execute(Simulation simulation)
    {
        simulation.BuildRequests.Enqueue(new BuildTurretRequest
        {
            Team = _team,
            TurretConfigId = _turretConfigId,
            Position = _position
        });
    }
}

