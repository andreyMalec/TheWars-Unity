using System;
using NaughtyAttributes;
using UnityEngine;

public sealed class UnitView : MonoBehaviour {
    private static readonly int Walk = Animator.StringToHash("walk");
    private static readonly int Attack = Animator.StringToHash("attack");

    public int EntityId;
    public UnitConfig unitConfig;
    private SpriteRenderer _renderer;
    private Animator _animator;
    private bool _animated;

    private void Awake() {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        if (_animator != null) {
            _animated = true;
        }
    }

    public void Bind(int entityId, in UnitConfig config) {
        EntityId = entityId;
        unitConfig = config;
        name = unitConfig.name + "_" + entityId;
    }

    public void Present(in UnitState state) {
        var scaleX = state.Direction == UnitDirection.Left ? -1f : 1f;
        transform.localScale = new Vector3(scaleX, 1f, 1f);

        transform.position = new Vector3(state.Position.x, state.Position.y, 0f);
        if (_animated)
            _animator.SetBool(Walk, state.Moving);
    }

    public void PlayAttackAnimation() {
        if (_animated)
            _animator.SetTrigger(Attack);
    }

    [Button]
    public void DebugPlayAttackAnimation() {
        _animator.Play(Attack);
    }

    [Button]
    public void DebugPlayWalkAnimation() {
        _animator.Play(Walk);
    }
}