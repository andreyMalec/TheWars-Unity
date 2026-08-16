using UnityEngine;

public enum TurretSlot {
    Slot1,
    Slot2,
    Slot3,
    Slot4,
}

public sealed class TurretState {
    public int Id;
    public Team Team;
    public ConfigId ConfigId;
    public TurretSlot Slot;
    public Vector2 Position;
    public float Rotation;
    public int TargetEntityId;
    public Vector2 LastTargetPosition;
    public int RecoveryTick;
    public int ExecuteTick;
    public int AttackIndex;
}