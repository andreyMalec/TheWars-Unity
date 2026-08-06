using UnityEngine;

public struct SpawnUnitRequest
{
    public int Team;
    public int UnitConfigId;
    public Vector2 Position;
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

