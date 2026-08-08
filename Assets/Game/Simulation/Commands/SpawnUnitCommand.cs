using UnityEngine;

public sealed class SpawnUnitCommand : ICommand {
    private readonly Team _team;
    private readonly ConfigId _unitConfigId;
    private readonly Vector2 _position;

    public SpawnUnitCommand(Team team, ConfigId unitConfigId, Vector2 position) {
        _team = team;
        _unitConfigId = unitConfigId;
        _position = position;
    }

    public void Execute(Simulation simulation) {
        simulation.SpawnQueue.Enqueue(new SpawnUnitRequest {
            Team = _team,
            UnitConfigId = _unitConfigId,
            Position = _position,
        });
    }
}