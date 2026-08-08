using System;
using UnityEngine;

public sealed class UnitView : MonoBehaviour {
    public int EntityId;
    public UnitConfig unitConfig;
    private SpriteRenderer _renderer;

    private void Awake() {
        _renderer = GetComponentInChildren<SpriteRenderer>();
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
    }
}