using System;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class ProjectileConfigBaker : MonoBehaviour {
    [SerializeField] public ProjectileConfig projectileConfig;

    private void OnValidate() {
        if (projectileConfig == null) return;

        var thisPrefab = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
        if (string.IsNullOrEmpty(thisPrefab)) return;
        Debug.Log($"[ProjectileConfigBaker] Baked projectile config for {thisPrefab}");
        projectileConfig.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(thisPrefab);
    }

    [Button]
    private void BakeConfigs() {
        var collider = GetComponent<CircleCollider2D>();
        projectileConfig.radius = collider.radius;
        collider.enabled = false;
    }

    private void OnDrawGizmos() {
        if (projectileConfig == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, projectileConfig.radius);
    }
}