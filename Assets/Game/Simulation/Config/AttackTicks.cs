using System;

[Serializable]
public struct AttackTicks {
    public int cooldownInterval;
    public int executeStandingMelee;
    public int executeStandingRanged;
    public int executeWalkingRanged;
}

public enum AttackType {
    StandingMelee,
    StandingRanged,
    WalkingRanged,
}