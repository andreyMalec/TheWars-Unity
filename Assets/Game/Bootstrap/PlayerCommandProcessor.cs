public class PlayerCommandProcessor : PlayerInputListener {
    private readonly Simulation _simulation;

    public PlayerCommandProcessor(Simulation simulation) {
        _simulation = simulation;
    }

    public void OnPlayerInput(PlayerInput input) {
        var fr = _simulation.Frame;
        var team = fr.LocalPlayerTeam();
        ICommand command = null;
        switch (input) {
            case PlayerInput.UpgradeBase:
                command = new UpgradeBaseCommand(team); break;
            case PlayerInput.SpecialWeapon:
                command = new SpecialWeaponCommand(team); break;
            case PlayerInput.SpawnUnit spawn:
                command = OnInputSpawnUnit(fr, spawn); break;
            case PlayerInput.BuySlot buy:
                command = new BuySlotCommand(team); break;
            case PlayerInput.BuildTurret build:
                command = OnInputBuildTurret(fr, build); break;
            case PlayerInput.DestroyTurret destroy:
                command = OnInputDestroyTurret(fr, destroy); break;
        }

        if (command != null)
            _simulation.EnqueueCommand(command);
    }

    private ICommand OnInputSpawnUnit(Frame fr, PlayerInput.SpawnUnit input) {
        var team = fr.LocalPlayerTeam();
        if (!fr.TryFindBaseByTeam(team, out var baseState)) return null;
        var unit = fr.FindConfig<UnitConfig>(baseState.Epoch, (EntityType)input.UnitIndex);
        return new SpawnUnitCommand(team, unit);
    }

    private ICommand OnInputBuildTurret(Frame fr, PlayerInput.BuildTurret input) {
        var team = fr.LocalPlayerTeam();
        if (!fr.TryFindBaseByTeam(team, out var baseState)) return null;
        var turret = fr.FindConfig<TurretConfig>(baseState.Epoch, (EntityType)input.TurretIndex);
        return new BuildTurretCommand(team, turret.id, (TurretSlot)input.SlotIndex);
    }

    private ICommand OnInputDestroyTurret(Frame fr, PlayerInput.DestroyTurret input) {
        var team = fr.LocalPlayerTeam();
        return new DestroyTurretCommand(team, (TurretSlot)input.SlotIndex);
    }
}