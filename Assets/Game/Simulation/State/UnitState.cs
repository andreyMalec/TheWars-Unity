using UnityEngine;

public enum UnitDirection {
    Left = -1,
    Right = 1
}

public sealed class UnitState : Damageable {
    public int Id { get; set; }
    public Team Team { get; set; }
    public ConfigId ConfigId { get; set; }
    public Vector2 Position { get; set; }
    public bool Moving;
    public UnitDirection Direction { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int TargetEntityId { get; set; }
    public AttackState Attack;
}

public struct AttackState {
    public int CooldownTick;
    public int ExecuteTick;
}