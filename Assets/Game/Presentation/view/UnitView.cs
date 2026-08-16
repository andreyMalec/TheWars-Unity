using System;
using NaughtyAttributes;
using UnityEngine;

public sealed class UnitView : MonoBehaviour {
    private static readonly int StandingMeleeAttack = Animator.StringToHash("StandingMeleeAttack");
    private static readonly int StandingRangedAttack = Animator.StringToHash("StandingRangedAttack");
    private static readonly int WalkingRangedAttack = Animator.StringToHash("WalkingRangedAttack");

    private static readonly int Walking = Animator.StringToHash("Walking");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int Alive = Animator.StringToHash("Alive");
    private static readonly int Ranged = Animator.StringToHash("Ranged");

    private static readonly int Debug_Restore = Animator.StringToHash("restore");

    public int EntityId;
    [Expandable] public UnitConfig unitConfig;
    public UnitAnimationConfig animationConfig;
    [SerializeField] private GameObject onHitPrefab;
    private SpriteRenderer _renderer;
    private Animator _animator;
    private AnimatorOverrideController _animatorController;
    private bool _animated;
    private UnitState _state;

    private void Awake() {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        if (_animator != null) {
            _animated = true;
            _animatorController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _animatorController;
        }
    }

    public void Bind(int entityId, in UnitConfig config) {
        EntityId = entityId;
        unitConfig = config;
        name = unitConfig.name + "_" + entityId;

        if (_animated) {
            _animator.SetBool(Ranged, unitConfig.attackType == UnitAttackType.Ranged);
            _animatorController["_Idle"] = animationConfig.Idle;
            _animatorController["_StandingMeleeAttack"] = animationConfig.StandingMeleeAttack;
            _animatorController["_StandingRangedAttack"] = animationConfig.StandingRangedAttack;
            _animatorController["_Walking"] = animationConfig.Walking;
            _animatorController["_WalkRangedAttack"] = animationConfig.WalkingRangedAttack;
            _animatorController["_Death"] = animationConfig.Death;
        }
    }

    public void Present(in UnitState state) {
        _state = state;
        var scaleX = state.Direction == UnitDirection.Left ? -1f : 1f;
        transform.localScale = new Vector3(scaleX, 1f, 1f);

        if (state.IsAlive)
            transform.position = new Vector3(state.Position.x, state.Position.y, state.Position.x);
        else
            transform.position = new Vector3(state.Position.x, state.Position.y, 5f);
        if (_animated) {
            _animator.SetBool(Walking, state.IsMoving);
            _animator.SetBool(Alive, state.IsAlive);
        }
    }

    public void OnEvent(UnitEvent e) {
        switch (e) {
            case UnitEvent.AttackStarted attack:
                if (!_state.IsAlive) return;
                switch (attack.AttackType) {
                    case AttackType.StandingMelee: PlayAttackAnimation(StandingMeleeAttack); break;
                    case AttackType.StandingRanged: PlayAttackAnimation(StandingRangedAttack); break;
                    case AttackType.WalkingRanged: PlayAttackAnimation(WalkingRangedAttack); break;
                }

                break;
            case UnitEvent.DeathStarted:
                PlayDeathAnimation();
                break;
            case UnitEvent.DamageTaken damageTaken:
                var hit = Instantiate(onHitPrefab, damageTaken.HitPoint, Quaternion.identity);
                hit.transform.SetParent(transform);
                break;
        }
    }

    private void PlayAttackAnimation(int trigger) {
        if (_animated)
            _animator.SetTrigger(trigger);
    }

    private void PlayDeathAnimation() {
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