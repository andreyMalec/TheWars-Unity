using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Unit Config")]
public sealed class UnitConfig : ScriptableObject {
    public ConfigId Id;
    public int Cost;
    public int MaxHealth;
    public float Size;
    public float Speed;
    public float AttackRange;
    public float AttackInterval;
    public float ProjectileSpeed;
    public int Damage;

    private void OnValidate() {
        Id = ConfigId.ForObject(this);
    }
}