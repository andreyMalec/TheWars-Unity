using System;
using UnityEngine;

[Serializable]
public struct UnitAttackTicks {
    public AttackTicks[] standingMelee;
    public AttackTicks[] standingRanged;
    public AttackTicks[] walkingRanged;
}

[Serializable]
public class AttackTicks {
    [SerializeField] public AttackTickType type;
    [SerializeField] public int value;

    public AttackTicks(AttackTickType type, int value) {
        this.type = type;
        this.value = value;
    }
}

[Serializable]
public enum AttackTickType {
    Execute,
    Recovery
}

public enum AttackType {
    StandingMelee,
    StandingRanged,
    WalkingRanged,
}