using System;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config/Projectile Config")]
public sealed class ProjectileConfig : ScriptableObject, EntityConfig {
    public ConfigId id { get; private set; }
    public float speed;

    [Header("Baked")]
    [Baked] public float radius;

    [Baked] public GameObject prefab;

    private void OnValidate() {
        id = ConfigId.ForObject(this);
        speed = Mathf.Max(0, speed);
        radius = Mathf.Max(0, radius);
        Debug.Log($"ProjectileConfig {name}[{id}] OnValidate");
    }
}