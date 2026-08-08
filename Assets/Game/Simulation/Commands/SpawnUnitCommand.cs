using UnityEngine;

public sealed class SpawnUnitCommand : ICommand {
    private readonly Team _team;
    private readonly ConfigId _unitConfigId;
    private readonly UnitType _unitType;

    public SpawnUnitCommand(Team team, ConfigId unitConfigId, UnitType unitType) {
        _team = team;
        _unitConfigId = unitConfigId;
        _unitType = unitType;
    }

    public SpawnUnitCommand(Team team, UnitConfig unitConfig) : this(team, unitConfig.id, unitConfig.unitType) {
    }

    public void Execute(Simulation simulation) {
        simulation.SpawnQueue.Enqueue(new SpawnUnitRequest {
            Team = _team,
            UnitConfigId = _unitConfigId,
            UnitType = _unitType,
        });
    }
}