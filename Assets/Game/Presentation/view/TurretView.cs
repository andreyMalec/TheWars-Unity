using UnityEngine;

public sealed class TurretView : MonoBehaviour {
    public int EntityId;
    private TurretConfig _config;

    public void Bind(int entityId, TurretConfig config) {
        EntityId = entityId;
        _config = config;
        name = config.name + "_" + entityId;
    }

    public void Present(TurretState state) {
        transform.position = new Vector3(state.Position.x, state.Position.y, 0f);
        transform.rotation = Quaternion.Euler(0f, 0f, state.Rotation);
    }
}