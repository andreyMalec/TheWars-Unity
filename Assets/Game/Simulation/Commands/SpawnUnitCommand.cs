using UnityEngine;

public sealed class SpawnUnitCommand : ICommand {
    private readonly Team _team;
    private readonly ConfigId _unitConfigId;
    private readonly EntityType _entityType;

    public SpawnUnitCommand(Team team, ConfigId unitConfigId, EntityType entityType) {
        _team = team;
        _unitConfigId = unitConfigId;
        _entityType = entityType;
    }

    public SpawnUnitCommand(Team team, UnitConfig unitConfig) : this(team, unitConfig.id, unitConfig.entityType) {
    }

    public void Execute(Simulation simulation) {
        simulation.SpawnQueue.Enqueue(new SpawnUnitRequest {
            Team = _team,
            UnitConfigId = _unitConfigId,
            EntityType = _entityType,
        });
    }
}