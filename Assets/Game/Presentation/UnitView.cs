using System;
using UnityEngine;

public sealed class UnitView : MonoBehaviour {
    public int EntityId;
    private UnitConfig _unitConfig;

    public void Bind(int entityId, UnitConfig config) {
        EntityId = entityId;
        _unitConfig = config;
        name = _unitConfig.name + "_" + entityId;
    }

    public void Present(UnitState state) {
        transform.position = new Vector3(state.Position.x, 0f, state.Position.y);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(_unitConfig.Size, 1f, 1f));
    }
}