using UnityEngine;

public sealed class TurretState
{
    public int Id;
    public Team Team;
    public ConfigId ConfigId;
    public Vector2 Position;
    public int TargetEntityId;
    public float Cooldown;
}

