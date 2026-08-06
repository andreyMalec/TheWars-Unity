using UnityEngine;

public sealed class ProjectileState
{
    public int Id;
    public int Team;
    public int SourceEntityId;
    public int TargetEntityId;
    public Vector2 Position;
    public Vector2 Direction;
    public float Speed;
    public float Lifetime;
}

