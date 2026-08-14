using System;

[Serializable]
public struct AttackTicks {
    public int executeStandingMelee;
    public int recoveryStandingMelee;
    public int executeStandingRanged;
    public int recoveryStandingRanged;
    public int executeWalkingRanged;
    public int recoveryWalkingRanged;
}

public enum AttackType {
    StandingMelee,
    StandingRanged,
    WalkingRanged,
}