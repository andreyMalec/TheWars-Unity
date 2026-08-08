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

    private readonly Dictionary<Type, Dictionary<ConfigId, EntityConfig>> _entities = new();

    private void OnEnable() {
        RebuildCache();
    }

    public void RebuildCache() {
        _entities.Clear();

        for (var i = 0; i < configs.Length; i++) {
            var config = configs[i];
            if (config.items.Length == 0) continue;
            var type = config.items[0]?.GetType();
            if (type == null) continue;
            _entities[type] = new Dictionary<ConfigId, EntityConfig>();

            for (var j = 0; j < config.items.Length; j++) {
                var item = (EntityConfig)config.items[j];
                _entities[type][item.id] = item;
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
        if (_entities.TryGetValue(typeof(T), out var map)) {
            if (map.TryGetValue(configId, out var config)) {
                if (config is T configInternal)
                    return configInternal;
            }
        }

        Debug.Log($"[GetConfig] No config found for {configId} type {typeof(T)}");
        return default(T);
    }

    public T GetConfig<T>(int index) where T : EntityConfig {
        foreach (var entry in configs) {
            if (entry.items.Length <= index) continue;
            if (entry.type != typeof(T).Name) continue;
            if (entry.items[index] is T config)
                return config;
        }

        Debug.Log($"[GetConfig] No config found for index {index} type {typeof(T)}");
        return default(T);
    }
}