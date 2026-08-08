using UnityEngine;

public sealed class BuildTurretCommand : ICommand {
    private readonly Team _team;
    private readonly ConfigId _turretConfigId;
    private readonly TurretSlot _slot;

    public BuildTurretCommand(Team team, ConfigId turretConfigId, TurretSlot slot) {
        _team = team;
        _turretConfigId = turretConfigId;
        _slot = slot;
    }

    public void Execute(Simulation simulation) {
        simulation.BuildRequests.Enqueue(new BuildTurretRequest {
            Team = _team,
            TurretConfigId = _turretConfigId,
            Slot = _slot
        });
    }
}