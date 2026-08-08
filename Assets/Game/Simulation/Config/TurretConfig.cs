using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Turret Config")]
public sealed class TurretConfig : ScriptableObject, EntityConfig {
    public ConfigId id { get; private set; }
    public int cost;
    public float attackRange;
    public float attackInterval;
    public float projectileSpeed;
    public int damage;

    private void OnValidate() {
        id = ConfigId.ForObject(this);
    }
}