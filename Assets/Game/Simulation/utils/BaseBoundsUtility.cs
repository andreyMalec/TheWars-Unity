using System;
using UnityEngine;

public static class BaseBoundsUtility {
    public static bool ContainsPoint(BaseState baseState, BaseConfig baseConfig, Vector2 point) {
        GetWorldBounds(baseState, baseConfig, out var min, out var max);
        return point.x >= min.x && point.x <= max.x && point.y >= min.y && point.y <= max.y;
    }

    public static bool SegmentIntersects(BaseState baseState, BaseConfig baseConfig, Vector2 from, Vector2 to) {
        GetWorldBounds(baseState, baseConfig, out var min, out var max);

        var dx = to.x - from.x;
        var dy = to.y - from.y;
        var tMin = 0f;
        var tMax = 1f;

        if (!ClipAxis(from.x, dx, min.x, max.x, ref tMin, ref tMax)) {
            return false;
        }

        if (!ClipAxis(from.y, dy, min.y, max.y, ref tMin, ref tMax)) {
            return false;
        }

        return true;
    }

    private static bool ClipAxis(float origin, float delta, float min, float max, ref float tMin, ref float tMax) {
        const float epsilon = 0.000001f;

        if (Math.Abs(delta) < epsilon) {
            return origin >= min && origin <= max;
        }

        var inv = 1f / delta;
        var t1 = (min - origin) * inv;
        var t2 = (max - origin) * inv;

        if (t1 > t2) {
            var temp = t1;
            t1 = t2;
            t2 = temp;
        }

        if (t1 > tMin) {
            tMin = t1;
        }

        if (t2 < tMax) {
            tMax = t2;
        }

        return tMin <= tMax;
    }

    private static void GetWorldBounds(BaseState baseState, BaseConfig baseConfig, out Vector2 min, out Vector2 max) {
        var origin = baseState.Position;
        var offset = baseConfig.colliderOffset;
        var size = baseConfig.colliderSize;

        min = origin + offset - size * 0.5f;
        max = origin + offset + size * 0.5f;
    }
}