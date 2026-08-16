using UnityEngine;

public sealed class ProjectileState {
    public int Id;
    public ConfigId ConfigId;
    public Team Team;
    public int SourceEntityId;
    public int TargetEntityId;
    public int Damage;
    public Vector2 Position;
    public Vector2 Velocity;
    public Vector2 Direction;
    public float Speed;
    public float Lifetime = 10f;
    public ProjectileType Type = ProjectileType.Linear;
}