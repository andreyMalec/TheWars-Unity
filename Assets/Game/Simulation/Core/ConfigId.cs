using System;
using UnityEngine;

[Serializable]
public readonly struct ConfigId : IEquatable<ConfigId> {
    public readonly ulong Value;

    public ConfigId(ulong value) {
        Value = value;
    }

    public static ConfigId ForObject(UnityEngine.Object configObject) {
        return new ConfigId(EntityId.ToULong(configObject.GetEntityId()));
    }

    public static ConfigId From(int value) {
        return new ConfigId((ulong)value);
    }

    public bool Equals(ConfigId other) {
        return Value == other.Value;
    }

    public override bool Equals(object obj) {
        return obj is ConfigId other && Equals(other);
    }

    public override int GetHashCode() {
        return Value.GetHashCode();
    }

    public static bool operator ==(ConfigId left, ConfigId right) {
        return left.Equals(right);
    }

    public static bool operator !=(ConfigId left, ConfigId right) {
        return !left.Equals(right);
    }

    public override string ToString() {
        return $"{Value}";
    }
}