using System;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public enum UnitAttackType {
    Melee,
    Ranged
}

public enum EntityType {
    Type1,
    Type2,
    Type3,
    Type4
}

[CreateAssetMenu(menuName = "Game/Config/Unit Config")]
public sealed class UnitConfig : ScriptableObject, EntityConfig, TypedEntity {
    public ConfigId id { get; private set; }
    public Epoch epoch;
    public EntityType entityType;
    public UnitAttackType attackType;
    public int cost;
    public int maxHealth;

    public float speed;
    public int damage;
    public float attackRangeMelee;
    [ShowIf("_ranged")] public float attackRangeRanged;
    [ShowIf("_ranged")] public bool canAttackWhileWalking;
    [ShowIf("_ranged")] public GameObject projectilePrefab;

    [Header("Baked")]
    [Baked] public Vector2[] collider;

    /**
     * 2 points - left bottom and right top
     */
    [Baked] public Vector2[] colliderBox;

    [Baked] public Vector2 movementCenter;

    [Baked] public AttackTicks attackTicks;

    [Baked] public ConfigId projectileId;
    [Baked] [ShowIf("_ranged")] public Vector2 projectilePosition;
    [Baked] public GameObject prefab;

    private bool _ranged;

    private void OnValidate() {
        _ranged = attackType == UnitAttackType.Ranged;
        id = ConfigId.ForObject(this);
        var radii = UnitColliderUtility.GetRadius(this);
        attackRangeMelee = Mathf.Max(radii, attackRangeMelee);
        attackRangeRanged = Mathf.Max(radii, attackRangeRanged);

        Debug.Log($"UnitConfig {name}[{id}] OnValidate; projectileId={projectileId}");
    }

    public Epoch _epoch => epoch;
    public EntityType _entityType => entityType;
}