using System;
using UnityEngine;

public sealed class UnitView : MonoBehaviour
{
    public int EntityId;

    public void Bind(int entityId)
    {
        EntityId = entityId;
        name = "UnitView_" + entityId;
    }

    public void Present(UnitState state)
    {
        transform.position = new Vector3(state.Position.x, 0f, state.Position.y);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}


