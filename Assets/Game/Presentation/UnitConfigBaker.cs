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
        _ranged = unitConfig?.type == UnitAttackType.Ranged;
        var view = GetComponent<UnitView>();
        if (view == null) return;
        if (unitConfig == null) return;
        view.unitConfig = unitConfig;

        var thisPrefab = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
        if (string.IsNullOrEmpty(thisPrefab)) return;
        Debug.Log($"[UnitConfigBaker] Baked unit config for {view.gameObject.name} ({thisPrefab})");
        unitConfig.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(thisPrefab);
    }

    [Button]
    private void BakeConfigs() {
        var view = GetComponent<UnitView>();
        var collider = GetComponent<PolygonCollider2D>();
        var points = collider.points;
        unitConfig.collider = points;
        unitConfig.colliderBox = new Vector2[2];
        var sortedX = points.OrderBy(point => point.x).ToList();
        var sortedY = points.OrderBy(point => point.y).ToList();
        unitConfig.colliderBox[0] = new Vector2(sortedX.First().x, sortedY.First().y);
        unitConfig.colliderBox[1] = new Vector2(sortedX.Last().x, sortedY.Last().y);
        collider.enabled = false;

        if (_ranged)
            unitConfig.projectilePosition = projectilePosition.localPosition;
    }

    private void OnDrawGizmos() {
        if (unitConfig == null) return;
        if (unitConfig.collider.Length == 0) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < unitConfig.collider.Length - 1; i++) {
            var point1 = unitConfig.collider[i];
            var point2 = unitConfig.collider[i + 1];
            Gizmos.DrawLine(transform.position + new Vector3(point1.x, point1.y, 0),
                transform.position + new Vector3(point2.x, point2.y, 0));
        }

        Gizmos.DrawLine(
            transform.position + new Vector3(unitConfig.collider[unitConfig.collider.Length - 1].x,
                unitConfig.collider[unitConfig.collider.Length - 1].y, 0f),
            transform.position + new Vector3(unitConfig.collider[0].x, unitConfig.collider[0].y, 0f));


        Gizmos.color = new Color(1f, 0.0f, 0.0f, .25f);
        Gizmos.DrawLine(
            transform.position + new Vector3(unitConfig.colliderBox[0].x, unitConfig.colliderBox[0].y, 0f),
            transform.position + new Vector3(unitConfig.colliderBox[0].x, unitConfig.colliderBox[1].y, 0f));
        Gizmos.DrawLine(
            transform.position + new Vector3(unitConfig.colliderBox[1].x, unitConfig.colliderBox[0].y, 0f),
            transform.position + new Vector3(unitConfig.colliderBox[1].x, unitConfig.colliderBox[1].y, 0f));
        Gizmos.DrawLine(
            transform.position + new Vector3(unitConfig.colliderBox[0].x, unitConfig.colliderBox[0].y, 0f),
            transform.position + new Vector3(unitConfig.colliderBox[1].x, unitConfig.colliderBox[0].y, 0f));
        Gizmos.DrawLine(
            transform.position + new Vector3(unitConfig.colliderBox[1].x, unitConfig.colliderBox[1].y, 0f),
            transform.position + new Vector3(unitConfig.colliderBox[0].x, unitConfig.colliderBox[1].y, 0f));
    }
}