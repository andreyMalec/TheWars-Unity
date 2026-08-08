using UnityEngine;

public struct SpawnUnitRequest {
    public Team Team;
    public ConfigId UnitConfigId;
    public UnitType UnitType;
}

public struct BuildTurretRequest {
    public Team Team;
    public ConfigId TurretConfigId;
    public TurretSlot Slot;
}

public struct UpgradeBaseRequest {
    public Team Team;
}

public struct SpecialWeaponRequest {
    public Team Team;
}

public struct DamageRequest {
    public int SourceEntityId;
    public int TargetEntityId;
    public int Amount;
}