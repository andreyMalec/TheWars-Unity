using UnityEngine;

public sealed class TurretView : MonoBehaviour
{
    public int EntityId;

    public void Bind(int entityId)
    {
        EntityId = entityId;
        name = "TurretView_" + entityId;
    }

    public void Present(TurretState state)
    {
        transform.position = new Vector3(state.Position.x, 0f, state.Position.y);
    }
}


