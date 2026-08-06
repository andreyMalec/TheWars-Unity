using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config Database")]
public sealed class ConfigDatabase : ScriptableObject
{
    [SerializeField] private UnitConfig[] units = new UnitConfig[0];
    [SerializeField] private TurretConfig[] turrets = new TurretConfig[0];
    [SerializeField] private BaseConfig[] bases = new BaseConfig[0];

    private readonly Dictionary<int, UnitConfig> _unitById = new Dictionary<int, UnitConfig>();
    private readonly Dictionary<int, TurretConfig> _turretById = new Dictionary<int, TurretConfig>();
    private readonly Dictionary<int, BaseConfig> _baseById = new Dictionary<int, BaseConfig>();

    private void OnEnable()
    {
        RebuildCache();
    }

    public void RebuildCache()
    {
        _unitById.Clear();
        _turretById.Clear();
        _baseById.Clear();

        for (var i = 0; i < units.Length; i++)
        {
            _unitById[units[i].ConfigId] = units[i];
        }

        for (var i = 0; i < turrets.Length; i++)
        {
            _turretById[turrets[i].ConfigId] = turrets[i];
        }

        for (var i = 0; i < bases.Length; i++)
        {
            _baseById[bases[i].ConfigId] = bases[i];
        }
    }

    public UnitConfig GetUnitConfig(int configId)
    {
        return _unitById[configId];
    }

    public TurretConfig GetTurretConfig(int configId)
    {
        return _turretById[configId];
    }

    public BaseConfig GetBaseConfig(int configId)
    {
        return _baseById[configId];
    }
}

