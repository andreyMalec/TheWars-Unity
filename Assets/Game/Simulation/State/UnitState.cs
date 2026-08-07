using UnityEngine;

public sealed class UnitState : Damageable {
    public int Id { get; set; }
    public int Team { get; set; }
    public ConfigId ConfigId { get; set; }
    public Vector2 Position { get; set; }
    public float Size { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int TargetEntityId { get; set; }
    public float Cooldown { get; set; }
}