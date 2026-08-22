using System;
using UnityEngine;

public class HealthBar : MonoBehaviour {
    [SerializeField] private Transform root;
    [SerializeField] private SpriteRenderer bar;
    [SerializeField] private Transform barBackground;

    private Vector2 _size;

    private void Awake() {
        _size = bar.size;
    }

    public void Present(in UnitState state) {
        if (!state.IsAlive) {
            root.gameObject.SetActive(false);
            return;
        }

        var scaleX = state.Direction == UnitDirection.Left ? -1f : 1f;
        root.localScale = new Vector3(scaleX, 1f, 1f);

        var healthRatio = Mathf.Clamp01((float)state.Health / state.MaxHealth);
        bar.size = new Vector2(_size.x * healthRatio, _size.y);
    }
}