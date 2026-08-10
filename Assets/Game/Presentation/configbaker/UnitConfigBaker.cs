using System;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class UnitConfigBaker : MonoBehaviour {
    [SerializeField] private UnitConfig unitConfig;
    [SerializeField] [ShowIf("_ranged")] private Transform projectilePosition;

    private bool _ranged;

    private void OnValidate() {
        _ranged = unitConfig?.attackType == UnitAttackType.Ranged;
        if (unitConfig == null) return;

        var thisPrefab = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
        if (string.IsNullOrEmpty(thisPrefab)) return;
        unitConfig.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(thisPrefab);

        if (_ranged) {
            var d = unitConfig.projectilePrefab.GetComponent<ProjectileConfigBaker>();
            var projId = ConfigId.ForObject(d.projectileConfig);
            unitConfig.projectileId = projId;
        }

        Debug.Log($"[UnitConfigBaker] Baked unit config for {thisPrefab}");
    }

    [Button]
    private void BakeConfigs() {
        var collider = GetComponent<PolygonCollider2D>();
        var points = collider.points;
        unitConfig.collider = points;
        unitConfig.colliderBox = new Vector2[2];
        var sortedX = points.OrderBy(point => point.x).ToList();
        var sortedY = points.OrderBy(point => point.y).ToList();
        unitConfig.colliderBox[0] = new Vector2(sortedX.First().x, sortedY.First().y);
        unitConfig.colliderBox[1] = new Vector2(sortedX.Last().x, sortedY.Last().y);
        collider.enabled = false;

        unitConfig.projectilePosition = _ranged ? projectilePosition.localPosition : Vector2.zero;
        var animator = GetComponentInChildren<Animator>();
        if (animator == null) return;
        var clips = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++) {
            var clip = clips[i];
            var events = AnimationUtility.GetAnimationEvents(clip);
            for (int j = 0; j < events.Length; j++) {
                var e = events[j];
                if (e.functionName == "Execute") {
                    unitConfig.attackExecuteTick = Mathf.RoundToInt(e.time  * Simulation.TickRate);
                    break;
                }
            }
        }
    }

    private void OnDrawGizmos() {
        if (unitConfig == null) return;
        if (unitConfig.collider.Length == 0) return;
        var mirrored = transform.localScale.x < 0f;
        var origin = (Vector2)transform.position;
        var rangeDirection = mirrored ? -1f : 1f;
        var projectileLocal = unitConfig.projectilePosition;
        var projectileStart = UnitColliderUtility.ToWorldPoint(projectileLocal, origin, mirrored);

        Gizmos.color = Color.deepSkyBlue;
        Gizmos.DrawLine(
            new Vector3(projectileStart.x, projectileStart.y, 0f),
            new Vector3(projectileStart.x + unitConfig.attackRange * rangeDirection, projectileStart.y, 0f));

        Gizmos.color = Color.red;
        for (int i = 0; i < unitConfig.collider.Length - 1; i++) {
            var point1 = UnitColliderUtility.ToWorldPoint(unitConfig.collider[i], origin, mirrored);
            var point2 = UnitColliderUtility.ToWorldPoint(unitConfig.collider[i + 1], origin, mirrored);
            Gizmos.DrawLine(new Vector3(point1.x, point1.y, 0f), new Vector3(point2.x, point2.y, 0f));
        }

        var lastPoint =
            UnitColliderUtility.ToWorldPoint(unitConfig.collider[unitConfig.collider.Length - 1], origin, mirrored);
        var firstPoint = UnitColliderUtility.ToWorldPoint(unitConfig.collider[0], origin, mirrored);
        Gizmos.DrawLine(
            new Vector3(lastPoint.x, lastPoint.y, 0f),
            new Vector3(firstPoint.x, firstPoint.y, 0f));


        Gizmos.color = new Color(1f, 0.0f, 0.0f, .25f);
        var boxMin = UnitColliderUtility.ToWorldPoint(unitConfig.colliderBox[0], origin, mirrored);
        var boxMax = UnitColliderUtility.ToWorldPoint(unitConfig.colliderBox[1], origin, mirrored);
        var minX = Mathf.Min(boxMin.x, boxMax.x);
        var maxX = Mathf.Max(boxMin.x, boxMax.x);
        var minY = Mathf.Min(boxMin.y, boxMax.y);
        var maxY = Mathf.Max(boxMin.y, boxMax.y);
        Gizmos.DrawLine(
            new Vector3(minX, minY, 0f),
            new Vector3(minX, maxY, 0f));
        Gizmos.DrawLine(
            new Vector3(maxX, minY, 0f),
            new Vector3(maxX, maxY, 0f));
        Gizmos.DrawLine(
            new Vector3(minX, minY, 0f),
            new Vector3(maxX, minY, 0f));
        Gizmos.DrawLine(
            new Vector3(maxX, maxY, 0f),
            new Vector3(minX, maxY, 0f));
    }
}