using System;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class TurretConfigBaker : MonoBehaviour {
    [SerializeField] private TurretConfig turretConfig;
    [SerializeField] private Transform[] projectilePosition;

    private void OnValidate() {
        if (turretConfig == null) return;

        var thisPrefab = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
        if (string.IsNullOrEmpty(thisPrefab)) return;
        turretConfig.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(thisPrefab);

        var d = turretConfig.projectilePrefab.GetComponent<ProjectileConfigBaker>();
        var projId = ConfigId.ForObject(d.projectileConfig);
        turretConfig.projectileId = projId;

        Debug.Log($"[TurretConfigBaker] Baked turret config for {thisPrefab}");
    }

    [Button]
    private void BakeConfigs() {
        turretConfig.projectilePositions = projectilePosition.Select(p => p.localPosition.ToVector2()).ToArray();
        var view = GetComponent<TurretView>();
        view.turretConfig = turretConfig;

        turretConfig.attackTicks = view.fireAnimation.AttackTicks();
    }

    private void OnDrawGizmos() {
        if (turretConfig == null) return;
        var mirrored = transform.lossyScale.x < 0f;
        var origin = (Vector2)transform.position;
        var rangeDirection = mirrored ? -1f : 1f;
        if (turretConfig.projectilePositions.Length == 0) return;
        var projectileLocal = turretConfig.projectilePositions[0];
        var projectileStart = UnitColliderUtility.ToWorldPoint(projectileLocal, origin, mirrored);

        Gizmos.color = Color.deepSkyBlue;
        Gizmos.DrawWireSphere(
            new Vector3(projectileStart.x, projectileStart.y, 0f),
            turretConfig.attackRange);
    }
}