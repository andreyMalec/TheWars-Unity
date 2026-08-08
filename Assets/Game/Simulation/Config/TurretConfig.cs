using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Turret Config")]
public sealed class TurretConfig : ScriptableObject, EntityConfig {
    public ConfigId id { get; private set; }
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
}