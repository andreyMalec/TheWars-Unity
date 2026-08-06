using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Base Config")]
public sealed class BaseConfig : ScriptableObject
{
    public int ConfigId;
    public int StartHealth;
    public int StartResources;
    public int IncomePerTick;
    public int UpgradeCost;
    public int HealthPerUpgrade;
}

