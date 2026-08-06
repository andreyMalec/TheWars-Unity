using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Turret Config")]
public sealed class TurretConfig : ScriptableObject {
    public ConfigId Id;
    public int Cost;
    public float AttackRange;
    public float AttackInterval;
    public float ProjectileSpeed;
    public int Damage;

    private void OnValidate() {
        Id = ConfigId.ForObject(this);
    }
}