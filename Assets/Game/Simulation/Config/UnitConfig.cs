using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Unit Config")]
public sealed class UnitConfig : ScriptableObject
{
    public int ConfigId;
    public int Cost;
    public int MaxHealth;
    public float Speed;
    public float AttackRange;
    public int Damage;
}

