using UnityEngine;

public sealed class ProjectileView : MonoBehaviour {
    public int EntityId;
    public ProjectileConfig projectileConfig;

    public void Bind(int entityId, ProjectileConfig config) {
        EntityId = entityId;
        projectileConfig = config;
        name = config.name + "_" + entityId;
    }

    public void Present(ProjectileState state) {
        var scaleX = state.Direction.x < 0 ? -1f : 1f;
        transform.localScale = new Vector3(scaleX, 1f, 1f);

        transform.position = new Vector3(state.Position.x, state.Position.y, 0f);
    }
}