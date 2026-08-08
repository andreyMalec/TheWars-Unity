using UnityEngine;

public sealed class DestroyTurretCommand : ICommand {
    private readonly Team _team;
    private readonly TurretSlot _slot;

    public DestroyTurretCommand(Team team, TurretSlot slot) {
        _team = team;
        _slot = slot;
    }

    public void Execute(Simulation simulation) {
        //TODo
    }
}