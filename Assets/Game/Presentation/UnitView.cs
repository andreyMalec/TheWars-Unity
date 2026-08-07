using System;
using UnityEngine;

public sealed class UnitView : MonoBehaviour {
    public int EntityId;
    public UnitConfig unitConfig;
    private SpriteRenderer _renderer;

    private Vector2 _position;

    private void Awake() {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Bind(int entityId, UnitConfig config) {
        EntityId = entityId;
        unitConfig = config;
        name = unitConfig.name + "_" + entityId;
    }

    public void Present(UnitState state) {
        if (!Mathf.Approximately(state.Position.x, _position.x))
            _renderer.flipX = state.Position.x < _position.x;

        transform.position = new Vector3(state.Position.x, state.Position.y, 0f);
        _position = state.Position;
    }
}