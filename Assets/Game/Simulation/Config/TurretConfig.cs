using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Turret Config")]
public sealed class TurretConfig : ScriptableObject, EntityConfig, TypedEntity {
    public ConfigId id { get; private set; }
    public Epoch epoch;
    public EntityType entityType;
    public int cost;
    public float attackRange;
    public float attackInterval;
    public int damage;
    public bool rotateToTarget;

    [Header("Baked")]
    [Baked] public GameObject projectilePrefab;

    [Baked] public ConfigId projectileId;
    [Baked] public Vector2 projectilePosition;
    [Baked] public GameObject prefab;

    private void OnValidate() {
        id = ConfigId.ForObject(this);
        Debug.Log($"TurretConfig {name}[{id}] OnValidate; projectileId={projectileId}");
    }

    public Epoch _epoch => epoch;
    public EntityType _entityType => entityType;
}