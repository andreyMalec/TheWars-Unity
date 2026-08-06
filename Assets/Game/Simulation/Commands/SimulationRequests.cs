using UnityEngine;

public struct SpawnUnitRequest
{
    public int Team;
    public ConfigId UnitConfigId;
    public Vector2 Position;
    public Vector2 Destination;
}

public struct BuildTurretRequest
{
    public int Team;
    public ConfigId TurretConfigId;
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

