using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Turret Config")]
public sealed class TurretConfig : ScriptableObject
{
    public int ConfigId;
    public int Cost;
    public int MaxHealth;
    public float AttackRange;
    public int Damage;
}

