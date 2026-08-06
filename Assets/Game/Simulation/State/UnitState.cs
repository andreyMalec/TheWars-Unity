using UnityEngine;

public sealed class UnitState {
    public int Id;
    public int Team;
    public ConfigId ConfigId;
    public Vector2 Position;
    public float Size;
    public int Health;
    public int TargetEntityId;
    public float Cooldown;

    public bool GetTargetPositionAndTeam(Frame f, out Vector2 position, out int team) {
        if (TargetEntityId == 0) {
            position = default;
            team = 0;
            return false;
        }

        f.TryGetEntityPositionAndTeam(TargetEntityId, out position, out team);
        return true;
    }
}