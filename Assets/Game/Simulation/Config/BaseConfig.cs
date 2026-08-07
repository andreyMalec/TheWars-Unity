using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Base Config")]
public sealed class BaseConfig : ScriptableObject, EntityConfig {
    public ConfigId Id { get; private set; }
    public int StartHealth;
    public int StartResources;
    public int IncomePerSecond;
    public int UpgradeCost;
    public int HealthPerUpgrade;
    public Vector4 Bounds;

    private void OnValidate() {
        Id = ConfigId.ForObject(this);
    }
}