using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Turret Config")]
public sealed class TurretConfig : ScriptableObject, EntityConfig {
    public ConfigId Id { get; private set; }
    public int Cost;
    public float AttackRange;
    public float AttackInterval;
    public float ProjectileSpeed;
    public int Damage;

    private void OnValidate() {
        Id = ConfigId.ForObject(this);
    }
}