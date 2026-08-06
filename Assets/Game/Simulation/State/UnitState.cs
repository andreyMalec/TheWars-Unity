using UnityEngine;

public sealed class UnitState
{
    public int Id;
    public int Team;
    public int ConfigId;
    public Vector2 Position;
    public float Size;
    public Vector2 Destination;
    public bool HasDestination;
    public int Health;
    public int TargetEntityId;
    public float Cooldown;
}

