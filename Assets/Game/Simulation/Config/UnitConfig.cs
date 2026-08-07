using System;
using NaughtyAttributes;
using UnityEngine;

public enum UnitAttackType {
    Melee,
    Ranged
}

[CreateAssetMenu(menuName = "Game/Config/Unit Config")]
public sealed class UnitConfig : ScriptableObject, EntityConfig {
    public ConfigId Id { get; private set; }
    public UnitAttackType type;
    public int Cost;
    public int MaxHealth;
    public float Size;
    public float Speed;
    public int Damage;
    public float AttackInterval;
    public float AttackRange;
    [ShowIf("_ranged")] public float ProjectileSpeed;

    private bool _ranged;

    private void OnValidate() {
        _ranged = type == UnitAttackType.Ranged;
        Id = ConfigId.ForObject(this);
        AttackRange = Mathf.Max(Size, AttackRange);
    }
}