using UnityEngine;

public sealed class BaseView : MonoBehaviour {
    public int EntityId;

    public void Bind(int entityId) {
        EntityId = entityId;
        name = "BaseView_" + entityId;
    }

    public void Present(BaseState state) {
        transform.position = new Vector3(state.Position.x, 0f, state.Position.y);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.25f, new Vector3(0.1f, 1f, 1f));
    }
}