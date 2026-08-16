using System;
using NaughtyAttributes;
using UnityEngine;

public enum ProjectileType {
    Linear,
    Homing,
    Ballistic
}

[CreateAssetMenu(menuName = "Game/Config/Projectile Config")]
public sealed class ProjectileConfig : ScriptableObject, EntityConfig {
    public ConfigId id { get; private set; }
    public float speed;
    public ProjectileType type;
    [ShowIf("_ballistic")] public float gravity;
    [ShowIf("_ballistic")] public bool highArc;
    [ShowIf("_ballistic")] public bool autoSwitchArcRoot;

    [Header("Baked")]
    [Baked] public float radius;

    [Baked] public GameObject prefab;

    private bool _ballistic;

    private void OnValidate() {
        id = ConfigId.ForObject(this);
        speed = Mathf.Max(0, speed);
        radius = Mathf.Max(0, radius);

        _ballistic = type == ProjectileType.Ballistic;

        Debug.Log($"ProjectileConfig {name}[{id}] OnValidate");
    }
}