using UnityEngine;

public enum ProjectileType {
    Linear,
    Homing,
    Ballistic,
}

public sealed class ProjectileState {
    public int Id;
    public int Team;
    public int SourceEntityId;
    public int TargetEntityId;
    public int Damage;
    public Vector2 Position;
    public Vector2 Direction;
    public float Speed;
    public float Lifetime;
    public ProjectileType Type = ProjectileType.Linear;
}