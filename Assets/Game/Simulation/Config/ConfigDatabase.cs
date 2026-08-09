using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[Serializable]
public struct ConfigEntry {
    public string type;
    public ScriptableObject[] items;
}

[CreateAssetMenu(menuName = "Game/Config Database")]
public sealed class ConfigDatabase : ScriptableObject {
    [SerializeField] private bool validate;
    [SerializeField] private ConfigEntry[] configs = Array.Empty<ConfigEntry>();

    private readonly Dictionary<Type, Dictionary<ConfigId, EntityConfig>> _byId = new();
    private readonly Dictionary<Type, Dictionary<Epoch, Dictionary<EntityType, EntityConfig>>> _byEpochAndType = new();

    private void OnEnable() {
        RebuildCache();
    }

    public void RebuildCache() {
        _byId.Clear();

        for (var i = 0; i < configs.Length; i++) {
            var config = configs[i];
            if (config.items.Length == 0) continue;
            var type = config.items[0]?.GetType();
            if (type == null) continue;
            _byId[type] = new Dictionary<ConfigId, EntityConfig>();
            _byEpochAndType[type] = new Dictionary<Epoch, Dictionary<EntityType, EntityConfig>>();

            for (var j = 0; j < config.items.Length; j++) {
                var item = (EntityConfig)config.items[j];
                _byId[type][item.id] = item;
                if (config.items[j] is TypedEntity typed) {
                    if (!_byEpochAndType[type].ContainsKey(typed._epoch)) {
                        _byEpochAndType[type][typed._epoch] = new Dictionary<EntityType, EntityConfig>();
                    }

                    _byEpochAndType[type][typed._epoch][typed._entityType] = item;
                }
            }
        }
    }

    private void OnValidate() {
        for (var i = 0; i < configs.Length; i++) {
            var config = configs[i];
            if (config.items.Length == 0) continue;
            var type = config.items[0]?.GetType();
            if (type == null) continue;
            configs[i].type = type.Name;

            for (var j = 0; j < config.items.Length; j++) {
                var item = config.items[j];
                if (item is EntityConfig) {
                    if (item.GetType() != type) {
                        Debug.LogError(
                            $"[ConfigDatabase] Config item {item.name} is of type {item.GetType()} but expected {type}");
                    }
                } else {
                    Debug.LogError($"[ConfigDatabase] Config item {item.name} is not of type EntityConfig");
                }
            }
        }

        Debug.Log($"[ConfigDatabase] Invalidated");
    }

    public T GetConfig<T>(ConfigId configId) where T : EntityConfig {
        if (_byId.TryGetValue(typeof(T), out var map)) {
            if (map.TryGetValue(configId, out var config)) {
                if (config is T configInternal)
                    return configInternal;
            }
        }

        Debug.Log($"[GetConfig] No config {typeof(T)} found for {configId}");
        return default(T);
    }

    public T GetConfig<T>(Epoch epoch, EntityType type) where T : EntityConfig {
        if (_byEpochAndType.TryGetValue(typeof(T), out var epoches)) {
            if (epoches.TryGetValue(epoch, out var map)) {
                if (map.TryGetValue(type, out var config)) {
                    if (config is T configInternal)
                        return configInternal;
                }
            }
        }

        Debug.Log($"[GetConfig] No config {typeof(T)} found for epoch {epoch}, type {type}");
        return default(T);
    }
}