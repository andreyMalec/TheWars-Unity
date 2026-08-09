using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Base Config")]
public sealed class BaseConfig : ScriptableObject, EntityConfig {
    public ConfigId id { get; private set; }
    public int startHealth;
    public int startResources;
    public int incomePerSecond;
    public int upgradeCost;
    [SerializeField] public Dictionary<TurretSlot, int> slotCost;
    public int healthPerUpgrade;
    [Baked] public Vector2 colliderSize;
    [Baked] public Vector2 colliderOffset;
    [Baked] public Vector2[] slotPositions;
    [Baked] public GameObject prefab;

    private void OnValidate() {
        id = ConfigId.ForObject(this);
    }
}