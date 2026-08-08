using UnityEngine;

public sealed class ProjectileView : MonoBehaviour
{
    public int EntityId;

    public void Bind(int entityId)
    {
        EntityId = entityId;
        name = "ProjectileView_" + entityId;
    }

    public void Present(ProjectileState state)
    {
        transform.position = new Vector3(state.Position.x, state.Position.y, 0f);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.05f);
    }
}


