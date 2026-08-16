using System;
using NaughtyAttributes;
using UnityEngine;

public sealed class TurretView : MonoBehaviour {
    private static readonly int Fire = Animator.StringToHash("Fire");

    public int EntityId;
    [Expandable] public TurretConfig turretConfig;
    public AnimationClip fireAnimation;
    private Animator _animator;
    private AnimatorOverrideController _animatorController;
    private bool _animated;

    private void Awake() {
        _animator = GetComponentInChildren<Animator>();
        if (_animator != null) {
            _animated = true;
            _animatorController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _animatorController;
        }
    }

    public void Bind(int entityId, TurretConfig config) {
        EntityId = entityId;
        turretConfig = config;
        name = config.name + "_" + entityId;
        if (_animated) {
            _animatorController["_Fire"] = fireAnimation;
        }
    }

    public void Present(TurretState state) {
        transform.position = new Vector3(state.Position.x, state.Position.y, 0f);
        transform.rotation = Quaternion.Euler(0f, 0f, state.Rotation);
    }

    public void OnEvent(TurretEvent e) {
        switch (e) {
            case TurretEvent.AttackStarted attackStarted:
                if (_animated) {
                    _animator.SetTrigger(Fire);
                }

                break;
        }
    }
}