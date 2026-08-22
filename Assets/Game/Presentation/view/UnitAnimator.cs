using NaughtyAttributes;
using UnityEngine;

public class UnitAnimator : MonoBehaviour {
    private static readonly int StandingMeleeAttack = Animator.StringToHash("StandingMeleeAttack");
    private static readonly int StandingRangedAttack = Animator.StringToHash("StandingRangedAttack");
    private static readonly int WalkingRangedAttack = Animator.StringToHash("WalkingRangedAttack");

    private static readonly int Walking = Animator.StringToHash("Walking");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int Alive = Animator.StringToHash("Alive");
    private static readonly int Ranged = Animator.StringToHash("Ranged");
    private static readonly int Debug_Restore = Animator.StringToHash("restore");

    private Animator _animator;
    private AnimatorOverrideController _animatorController;
    private bool _animated;

    public UnitAnimationConfig animationConfig;

    private void Awake() {
        _animator = GetComponentInChildren<Animator>();
        if (_animator != null) {
            _animated = true;
            _animatorController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _animatorController;
        }
    }

    public void Bind(int entityId, in UnitConfig config) {
        if (_animated) {
            _animator.SetBool(Ranged, config.attackType == UnitAttackType.Ranged);
            _animatorController["_Idle"] = animationConfig.Idle;
            _animatorController["_StandingMeleeAttack"] = animationConfig.StandingMeleeAttack;
            _animatorController["_StandingRangedAttack"] = animationConfig.StandingRangedAttack;
            _animatorController["_Walking"] = animationConfig.Walking;
            _animatorController["_WalkRangedAttack"] = animationConfig.WalkingRangedAttack;
            _animatorController["_Death"] = animationConfig.Death;
        }
    }

    public void Present(in UnitState state) {
        if (_animated) {
            _animator.SetBool(Walking, state.IsMoving);
            _animator.SetBool(Alive, state.IsAlive);
        }
    }

    public void PlayAttackAnimation(AttackType type) {
        switch (type) {
            case AttackType.StandingMelee: PlayAttackAnimation(StandingMeleeAttack); break;
            case AttackType.StandingRanged: PlayAttackAnimation(StandingRangedAttack); break;
            case AttackType.WalkingRanged: PlayAttackAnimation(WalkingRangedAttack); break;
        }
    }

    private void PlayAttackAnimation(int trigger) {
        if (_animated)
            _animator.SetTrigger(trigger);
    }

    public void PlayDeathAnimation() {
        if (_animated)
            _animator.SetTrigger(Death);
    }

    [Button]
    public void DebugStandingMeleeAttack() {
        _animator.Play(StandingMeleeAttack);
    }

    [Button]
    public void DebugStandingRangedAttack() {
        _animator.Play(StandingRangedAttack);
    }

    [Button]
    public void DebugWalkRangedAttack() {
        _animator.Play(WalkingRangedAttack);
    }

    private bool _debugWalk;

    [Button]
    public void DebugPlayWalkAnimation() {
        _animator.SetBool(Walking, !_debugWalk);
    }

    private bool _debugDeath;

    [Button]
    public void DebugPlayDeathAnimation() {
        _debugDeath = !_debugDeath;
        if (!_debugDeath)
            _animator.SetTrigger(Debug_Restore);
        else
            _animator.SetTrigger(Death);
    }
}