using UnityEngine;

public class Slot {
    public TurretSlot Type;
    public bool IsActive;
    public bool HasTurret;
    public int TurretId;
    public ConfigId TurretConfigId;
}

public sealed class BaseState : Damageable {
    public int Id { get; set; }
    public ConfigId ConfigId { get; set; }
    public Team Team { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Epoch { get; set; }
    public int Resources { get; set; }
    public Vector2 Position { get; set; }
    public Slot[] Slots { get; set; }
}