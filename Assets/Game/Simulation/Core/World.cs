using System.Collections.Generic;

public sealed class World
{
    private readonly EntityIdGenerator _entityIdGenerator = new EntityIdGenerator();

    public readonly Dictionary<int, BaseState> Bases = new Dictionary<int, BaseState>();
    public readonly Dictionary<int, UnitState> Units = new Dictionary<int, UnitState>();
    public readonly Dictionary<int, TurretState> Turrets = new Dictionary<int, TurretState>();
    public readonly Dictionary<int, ProjectileState> Projectiles = new Dictionary<int, ProjectileState>();

    public int GenerateEntityId()
    {
        return _entityIdGenerator.Next();
    }

    public void AddBase(BaseState state)
    {
        Bases[state.Id] = state;
    }

    public bool RemoveBase(int id)
    {
        return Bases.Remove(id);
    }

    public bool TryFindBase(int id, out BaseState state)
    {
        return Bases.TryGetValue(id, out state);
    }

    public void AddUnit(UnitState state)
    {
        Units[state.Id] = state;
    }

    public bool RemoveUnit(int id)
    {
        return Units.Remove(id);
    }

    public bool TryFindUnit(int id, out UnitState state)
    {
        return Units.TryGetValue(id, out state);
    }

    public void AddTurret(TurretState state)
    {
        Turrets[state.Id] = state;
    }

    public bool RemoveTurret(int id)
    {
        return Turrets.Remove(id);
    }

    public bool TryFindTurret(int id, out TurretState state)
    {
        return Turrets.TryGetValue(id, out state);
    }

    public void AddProjectile(ProjectileState state)
    {
        Projectiles[state.Id] = state;
    }

    public bool RemoveProjectile(int id)
    {
        return Projectiles.Remove(id);
    }

    public bool TryFindProjectile(int id, out ProjectileState state)
    {
        return Projectiles.TryGetValue(id, out state);
    }
}

