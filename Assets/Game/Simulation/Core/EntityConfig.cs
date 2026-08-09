using System;

public interface EntityConfig {
    public ConfigId id { get; }
}

public interface TypedEntity {
    public Epoch _epoch { get; }
    public EntityType _entityType { get; }
}

/**
 * Marks a field or method as baked, meaning it will be serialized into the entity config and not be changed at runtime.
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class BakedAttribute : Attribute {
}