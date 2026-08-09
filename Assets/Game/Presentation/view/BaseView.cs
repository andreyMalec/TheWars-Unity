using UnityEngine;

public sealed class BaseView : MonoBehaviour {
    public int EntityId;
    public Transform[] slots;

    public void Bind(int entityId, BaseConfig baseConfig) {
        EntityId = entityId;
        name = baseConfig.name + "_" + entityId;
    }

    public void Present(BaseState state) {
        var scaleX = state.Team == Team.Left ? 1f : -1f;
        transform.localScale = new Vector3(scaleX, 1f, 1f);
        transform.position = new Vector3(state.Position.x, state.Position.y, 0f);

        for (int i = 0; i < state.Slots.Length; i++) {
            slots[i].gameObject.SetActive(state.Slots[i].IsActive);
        }
    }
}