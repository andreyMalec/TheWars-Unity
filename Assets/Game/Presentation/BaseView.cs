using UnityEngine;

public sealed class BaseView : MonoBehaviour {
    public int EntityId;
    private BaseConfig _baseConfig;

    public void Bind(int entityId, BaseConfig baseConfig) {
        EntityId = entityId;
        _baseConfig = baseConfig;
        name = baseConfig.name + "_" + entityId;
    }

    public void Present(BaseState state) {
        transform.position = new Vector3(state.Position.x, state.Position.y, 0f);
    }

    private void OnDrawGizmos() {
        if (_baseConfig == null) {
            return;
        }

        var w = _baseConfig.Bounds.z - _baseConfig.Bounds.x;
        var h = _baseConfig.Bounds.w - _baseConfig.Bounds.y;
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(w, h, 1f));
    }
}