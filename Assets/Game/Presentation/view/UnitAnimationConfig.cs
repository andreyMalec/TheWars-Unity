using System;
using UnityEngine;

[Serializable]
public struct UnitAnimationConfig {
    public AnimationClip Idle;
    public AnimationClip StandingMeleeAttack;
    public AnimationClip StandingRangedAttack;
    public AnimationClip Walking;
    public AnimationClip WalkingRangedAttack;
    public AnimationClip Death;
}