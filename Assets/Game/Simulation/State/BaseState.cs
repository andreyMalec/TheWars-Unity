using UnityEngine;

public sealed class BaseState : Damageable {
    public int Id { get; set; }
    public ConfigId ConfigId { get; set; }
    public int Team { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Level { get; set; }
    public int Resources { get; set; }
    public Vector2 Position { get; set; }
}