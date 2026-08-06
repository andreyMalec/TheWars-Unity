using UnityEngine;

public struct SpawnUnitRequest
{
    public int Team;
    public int UnitConfigId;
    public Vector2 Position;
    public Vector2 Destination;
}

public struct BuildTurretRequest
{
    public int Team;
    public int TurretConfigId;
    public Vector2 Position;
}

public struct UpgradeBaseRequest
{
    public int BaseEntityId;
}

public struct DamageRequest
{
    public int SourceEntityId;
    public int TargetEntityId;
    public int Amount;
}

