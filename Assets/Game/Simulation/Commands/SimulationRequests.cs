using UnityEngine;

public struct SpawnUnitRequest {
    public Team Team;
    public ConfigId UnitConfigId;
    public Vector2 Position;
}

public struct BuildTurretRequest {
    public Team Team;
    public ConfigId TurretConfigId;
    public Vector2 Position;
}

public struct UpgradeBaseRequest {
    public int BaseEntityId;
}

public struct DamageRequest {
    public int SourceEntityId;
    public int TargetEntityId;
    public int Amount;
}