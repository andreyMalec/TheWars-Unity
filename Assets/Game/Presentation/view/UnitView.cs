using System;
using NaughtyAttributes;
using UnityEngine;

public sealed class UnitView : MonoBehaviour {
    public int EntityId;
    [Expandable] public UnitConfig unitConfig;
    [SerializeField] private GameObject onHitPrefab;
    private SpriteRenderer _renderer;
    private UnitState _state;
    private UnitAnimator _animator;
    private HealthBar _healthBar;

    private void Awake() {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponent<UnitAnimator>();
        _healthBar = Instantiate(PrefabCollectionManager.Instance.hpProgress, transform).GetComponent<HealthBar>();
    }

    public void Bind(int entityId, in UnitConfig config) {
        EntityId = entityId;
        unitConfig = config;
        name = unitConfig.name + "_" + entityId;
        _animator.Bind(entityId, config);
    }

    public void Present(in UnitState state) {
        _state = state;
        var scaleX = state.Direction == UnitDirection.Left ? -1f : 1f;
        transform.localScale = new Vector3(scaleX, 1f, 1f);

        if (state.IsAlive)
            transform.position = new Vector3(state.Position.x, state.Position.y, state.Position.x);
        else
            transform.position = new Vector3(state.Position.x, state.Position.y, 5f);
        _animator.Present(state);
        _healthBar.Present(state);
    }

    public void OnEvent(UnitEvent e) {
        switch (e) {
            case UnitEvent.AttackStarted attack:
                if (!_state.IsAlive) return;
                _animator.PlayAttackAnimation(attack.AttackType);

                break;
            case UnitEvent.DeathStarted:
                _animator.PlayDeathAnimation();
                break;
            case UnitEvent.DamageTaken damageTaken:
                var hit = Instantiate(onHitPrefab, damageTaken.HitPoint, Quaternion.identity);
                hit.transform.SetParent(transform);
                break;
        }
    }
}