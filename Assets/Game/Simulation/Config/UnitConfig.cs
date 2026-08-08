using System;
using NaughtyAttributes;
using UnityEngine;

public enum UnitAttackType {
    Melee,
    Ranged
}

[CreateAssetMenu(menuName = "Game/Config/Unit Config")]
public sealed class UnitConfig : ScriptableObject, EntityConfig {
    public ConfigId id { get; private set; }
    public UnitAttackType type;
    public int cost;
    public int maxHealth;

    public float speed;
    public int damage;
    public float attackInterval;
    public float attackRange;
    [ShowIf("_ranged")] public float projectileSpeed;

    [Header("Baked")]
    [Baked] public Vector2[] collider;

    /**
     * 2 points - left bottom and right top
     */
    [Baked] public Vector2[] colliderBox;

    [Baked] [ShowIf("_ranged")] public Vector2 projectilePosition;
    [Baked] public GameObject prefab;

    private bool _ranged;

    private void OnValidate() {
        _ranged = type == UnitAttackType.Ranged;
        id = ConfigId.ForObject(this);
        var radii = UnitColliderUtility.GetRadius(this);
        attackRange = Mathf.Max(radii, attackRange);
    }
}