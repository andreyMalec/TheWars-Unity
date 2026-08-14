using System;
using System.Linq;
using JetBrains.Annotations;
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

        var view = GetComponent<UnitView>();
        unitConfig.attackTicks.executeStandingMelee = ExecuteTicks(view.animationConfig.StandingMeleeAttack);
        unitConfig.attackTicks.recoveryStandingMelee = RecoveryTicks(view.animationConfig.StandingMeleeAttack) - unitConfig.attackTicks.executeStandingMelee;
        unitConfig.attackTicks.executeStandingRanged = ExecuteTicks(view.animationConfig.StandingRangedAttack);
        unitConfig.attackTicks.recoveryStandingRanged = RecoveryTicks(view.animationConfig.StandingRangedAttack) - unitConfig.attackTicks.executeStandingRanged;
        unitConfig.attackTicks.executeWalkingRanged = ExecuteTicks(view.animationConfig.WalkingRangedAttack);
        unitConfig.attackTicks.recoveryWalkingRanged = RecoveryTicks(view.animationConfig.WalkingRangedAttack) - unitConfig.attackTicks.executeWalkingRanged;

        var renderer = GetComponentInChildren<SpriteRenderer>();
        unitConfig.movementCenter = renderer.sprite.pivot / renderer.sprite.pixelsPerUnit * 0.8f;
    }

    private int ExecuteTicks([CanBeNull] AnimationClip clip) {
        if (clip == null) return 1;
        var events = AnimationUtility.GetAnimationEvents(clip);
        for (int j = 0; j < events.Length; j++) {
            var e = events[j];
            if (e.functionName == "Execute") {
                return Mathf.RoundToInt(e.time * Simulation.TickRate);
            }
        }

        return 1;
    }

    private int RecoveryTicks([CanBeNull] AnimationClip clip) {
        if (clip == null) return 1;
        return Mathf.RoundToInt(clip.length * Simulation.TickRate);
    }

    private void OnDrawGizmos() {
        if (unitConfig == null) return;
        if (unitConfig.collider.Length == 0) return;
        var mirrored = transform.localScale.x < 0f;
        var origin = (Vector2)transform.position;
        var rangeDirection = mirrored ? -1f : 1f;

        if (unitConfig.attackType == UnitAttackType.Ranged) {
            var projectileLocal = unitConfig.projectilePosition;
            var projectileStart = UnitColliderUtility.ToWorldPoint(projectileLocal, origin, mirrored);
            Gizmos.color = Color.deepSkyBlue;
            Gizmos.DrawLine(
                new Vector3(projectileStart.x, projectileStart.y, 0f),
                new Vector3(projectileStart.x + unitConfig.attackRangeRanged * rangeDirection, projectileStart.y, 0f));
        }

        Gizmos.color = Color.darkGoldenRod;
        Gizmos.DrawWireSphere(new Vector3(origin.x, origin.y - unitConfig.movementCenter.y, 0f),
            unitConfig.movementCenter.x);

        Gizmos.color = Color.lightSkyBlue;
        Gizmos.DrawLine(transform.position,
            new Vector3(origin.x + unitConfig.attackRangeMelee * rangeDirection, origin.y, 0f));

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