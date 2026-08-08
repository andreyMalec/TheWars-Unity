using System;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class BaseConfigBaker : MonoBehaviour {
    [SerializeField] private BaseConfig baseConfig;
    [SerializeField] private Transform[] slotPositions;

    private void OnValidate() {
        if (baseConfig == null) return;

        var thisPrefab = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
        if (string.IsNullOrEmpty(thisPrefab)) return;
        baseConfig.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(thisPrefab);

        Debug.Log($"[BaseConfigBaker] Baked base config for {thisPrefab}");
    }

    [Button]
    private void BakeConfigs() {
        baseConfig.slotPositions = slotPositions.Select(t => t.localPosition.ToVector2()).ToArray();
        var collider = GetComponent<BoxCollider2D>();
        baseConfig.colliderOffset = collider.offset;
        baseConfig.colliderSize = collider.size;
        collider.enabled = false;
    }

    private void OnDrawGizmos() {
        if (baseConfig == null) return;
        if (baseConfig.slotPositions == null) return;

        var mirrored = transform.localScale.x < 0f;
        var origin = (Vector2)transform.position;
        for (int i = 0; i < baseConfig.slotPositions.Length; i++) {
            var slotLocal = baseConfig.slotPositions[i];
            var slotStart = UnitColliderUtility.ToWorldPoint(slotLocal, origin, mirrored);

            Gizmos.color = Color.greenYellow;
            Gizmos.DrawWireSphere(slotStart, 0.25f);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(
            new Vector3(origin.x + baseConfig.colliderOffset.x, origin.y + baseConfig.colliderOffset.y, 0f),
            new Vector3(baseConfig.colliderSize.x, baseConfig.colliderSize.y, 0f));
    }
}