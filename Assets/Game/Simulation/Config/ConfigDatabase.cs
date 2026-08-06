using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config Database")]
public sealed class ConfigDatabase : ScriptableObject {
    [SerializeField] private UnitConfig[] units = Array.Empty<UnitConfig>();
    [SerializeField] private TurretConfig[] turrets = Array.Empty<TurretConfig>();
    [SerializeField] private BaseConfig[] bases = Array.Empty<BaseConfig>();

    private readonly Dictionary<ConfigId, UnitConfig> _unitById = new();
    private readonly Dictionary<ConfigId, TurretConfig> _turretById = new();
    private readonly Dictionary<ConfigId, BaseConfig> _baseById = new();

    private void OnEnable() {
        RebuildCache();
    }

    public void RebuildCache() {
        _unitById.Clear();
        _turretById.Clear();
        _baseById.Clear();

        for (var i = 0; i < units.Length; i++) {
            _unitById[units[i].Id] = units[i];
        }

        for (var i = 0; i < turrets.Length; i++) {
            _turretById[turrets[i].Id] = turrets[i];
        }

        for (var i = 0; i < bases.Length; i++) {
            _baseById[bases[i].Id] = bases[i];
        }
    }

    public UnitConfig GetUnitConfig(ConfigId configId) {
        return _unitById.TryGetValue(configId, out var config) ? config : units[0];
    }

    public UnitConfig GetUnitConfig(int index) {
        return units[index];
    }

    public TurretConfig GetTurretConfig(ConfigId configId) {
        return _turretById.TryGetValue(configId, out var config) ? config : turrets[0];
    }

    public BaseConfig GetBaseConfig(ConfigId configId) {
        return _baseById.TryGetValue(configId, out var config) ? config : bases[0];
    }
}