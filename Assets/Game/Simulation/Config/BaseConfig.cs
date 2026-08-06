using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Base Config")]
public sealed class BaseConfig : ScriptableObject {
    public ConfigId Id;
    public int StartHealth;
    public int StartResources;
    public int IncomePerSecond;
    public int UpgradeCost;
    public int HealthPerUpgrade;

    private void OnValidate() {
        Id = ConfigId.ForObject(this);
    }
}