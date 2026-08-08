using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Base Config")]
public sealed class BaseConfig : ScriptableObject, EntityConfig {
    public ConfigId id { get; private set; }
    public int startHealth;
    public int startResources;
    public int incomePerSecond;
    public int upgradeCost;
    public int healthPerUpgrade;
    public Vector4 bounds;
    public Vector2[] slotPositions;

    private void OnValidate() {
        id = ConfigId.ForObject(this);
    }
}