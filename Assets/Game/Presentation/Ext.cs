using UnityEngine;

public static class Ext {
    public static Vector2 ToVector2(this Vector3 v) {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 ToVector2(this Vector2 v) {
        return new Vector3(v.x, v.y, 0f);
    }
}