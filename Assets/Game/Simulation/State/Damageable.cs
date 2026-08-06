using UnityEngine;

public interface Damageable {
    public int Id { get; }
    public ConfigId ConfigId { get; }
    public int Team { get; }
    public int Health { get; }
    public int MaxHealth { get; }
    public Vector2 Position { get; }
}