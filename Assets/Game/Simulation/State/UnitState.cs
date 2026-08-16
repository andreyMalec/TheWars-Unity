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
    public bool IsMoving;
    public bool IsAlive;
    public int DeathTick;
    public UnitDirection Direction { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int TargetEntityId { get; set; }
    public Vector2 LastTargetPosition;
    public AttackState Attack;
}

public struct AttackState {
    public int RecoveryTick;
    public int ExecuteTick;
    public AttackType AttackType;
    public int AttackIndex;

    public bool IsAttacking(Frame fr) {
        return ExecuteTick > 0 || RecoveryTick > fr.Tick;
    }
}