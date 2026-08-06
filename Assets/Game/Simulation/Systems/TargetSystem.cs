public sealed class TargetSystem : ISystem
{
    public void Run(Simulation simulation)
    {
        var world = simulation.Frame.World;

        foreach (var pair in world.Units)
        {
            var unit = pair.Value;
            unit.TargetEntityId = TargetingUtility.FindNearestEnemy(world, unit.Team, unit.Position);
        }

        foreach (var pair in world.Turrets)
        {
            var turret = pair.Value;
            turret.TargetEntityId = TargetingUtility.FindNearestEnemy(world, turret.Team, turret.Position);
        }
    }
}

