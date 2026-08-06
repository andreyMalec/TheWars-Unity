using UnityEngine;

public sealed class TurretState
{
    public int Id;
    public int Team;
    public ConfigId ConfigId;
    public Vector2 Position;
    public int TargetEntityId;
    public float Cooldown;
}

