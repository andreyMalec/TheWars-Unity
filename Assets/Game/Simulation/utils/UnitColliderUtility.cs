using UnityEngine;

public static class UnitColliderUtility {
    private const float Epsilon = 0.000001f;

    public static UnitDirection ResolveDirection(UnitDirection current, float deltaX) {
        if (deltaX > Epsilon) {
            return UnitDirection.Right;
        }

        if (deltaX < -Epsilon) {
            return UnitDirection.Left;
        }

        return current;
    }

    public static bool IsMirrored(UnitDirection direction) {
        return direction == UnitDirection.Left;
    }

    public static Vector2 ToWorldPoint(Vector2 localPoint, Vector2 origin, bool mirrored) {
        var x = mirrored ? -localPoint.x : localPoint.x;
        return new Vector2(origin.x + x, origin.y + localPoint.y);
    }

    public static float GetRadius(UnitConfig config) {
        var collider = config.collider;
        if (collider.Length == 0) {
            return 0f;
        }

        var maxSqr = 0f;
        for (var i = 0; i < collider.Length; i++) {
            var sqr = collider[i].sqrMagnitude;
            if (sqr > maxSqr) {
                maxSqr = sqr;
            }
        }

        return Mathf.Sqrt(maxSqr);
    }

    public static bool PolygonsOverlap(
        Vector2[] localA,
        Vector2 originA,
        bool mirroredA,
        Vector2[] localB,
        Vector2 originB,
        bool mirroredB
    ) {
        for (var i = 0; i < localA.Length; i++) {
            var a1 = ToWorldPoint(localA[i], originA, mirroredA);
            var a2 = ToWorldPoint(localA[(i + 1) % localA.Length], originA, mirroredA);

            for (var j = 0; j < localB.Length; j++) {
                var b1 = ToWorldPoint(localB[j], originB, mirroredB);
                var b2 = ToWorldPoint(localB[(j + 1) % localB.Length], originB, mirroredB);
                if (SegmentsIntersect(a1, a2, b1, b2)) {
                    return true;
                }
            }
        }

        if (PointInPolygon(ToWorldPoint(localA[0], originA, mirroredA), localB, originB, mirroredB)) {
            return true;
        }

        if (PointInPolygon(ToWorldPoint(localB[0], originB, mirroredB), localA, originA, mirroredA)) {
            return true;
        }

        return false;
    }

    private static bool SegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2) {
        var o1 = Orientation(p1, p2, q1);
        var o2 = Orientation(p1, p2, q2);
        var o3 = Orientation(q1, q2, p1);
        var o4 = Orientation(q1, q2, p2);

        if ((o1 > Epsilon && o2 < -Epsilon || o1 < -Epsilon && o2 > Epsilon) &&
            (o3 > Epsilon && o4 < -Epsilon || o3 < -Epsilon && o4 > Epsilon)) {
            return true;
        }

        if (Mathf.Abs(o1) <= Epsilon && OnSegment(p1, p2, q1)) {
            return true;
        }

        if (Mathf.Abs(o2) <= Epsilon && OnSegment(p1, p2, q2)) {
            return true;
        }

        if (Mathf.Abs(o3) <= Epsilon && OnSegment(q1, q2, p1)) {
            return true;
        }

        if (Mathf.Abs(o4) <= Epsilon && OnSegment(q1, q2, p2)) {
            return true;
        }

        return false;
    }

    private static float Orientation(Vector2 a, Vector2 b, Vector2 c) {
        return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
    }

    private static bool OnSegment(Vector2 a, Vector2 b, Vector2 p) {
        return p.x >= Mathf.Min(a.x, b.x) - Epsilon && p.x <= Mathf.Max(a.x, b.x) + Epsilon &&
               p.y >= Mathf.Min(a.y, b.y) - Epsilon && p.y <= Mathf.Max(a.y, b.y) + Epsilon;
    }

    private static bool PointInPolygon(Vector2 worldPoint, Vector2[] localPolygon, Vector2 origin, bool mirrored) {
        var inside = false;
        var j = localPolygon.Length - 1;

        for (var i = 0; i < localPolygon.Length; i++) {
            var pi = ToWorldPoint(localPolygon[i], origin, mirrored);
            var pj = ToWorldPoint(localPolygon[j], origin, mirrored);

            if (Mathf.Abs(Orientation(pj, pi, worldPoint)) <= Epsilon && OnSegment(pj, pi, worldPoint)) {
                return true;
            }

            var intersects = (pi.y > worldPoint.y) != (pj.y > worldPoint.y) &&
                             worldPoint.x < (pj.x - pi.x) * (worldPoint.y - pi.y) / (pj.y - pi.y) + pi.x;
            if (intersects) {
                inside = !inside;
            }

            j = i;
        }

        return inside;
    }
    
    public static bool RayPolygonIntersection(
        Vector2 rayOrigin,
        Vector2 rayDirection,
        bool mirrored,
        Vector2 polygonPosition,
        Vector2[] polygon,
        out Vector2 intersection)
    {
        intersection = default;

        if (rayDirection.sqrMagnitude < Mathf.Epsilon || polygon == null || polygon.Length < 2)
            return false;

        rayDirection.Normalize();

        float closestDistance = float.PositiveInfinity;
        bool found = false;

        for (int i = 0; i < polygon.Length; i++)
        {
            Vector2 a = ToWorldPoint(polygon[i], polygonPosition, mirrored);
            Vector2 b = ToWorldPoint( polygon[(i + 1) % polygon.Length], polygonPosition, mirrored);

            if (RaySegmentIntersection(
                    rayOrigin,
                    rayDirection,
                    a,
                    b,
                    out Vector2 point,
                    out float distance))
            {
                if (distance >= 0f && distance < closestDistance)
                {
                    closestDistance = distance;
                    intersection = point;
                    found = true;
                }
            }
        }

        return found;
    }

    private static bool RaySegmentIntersection(
        Vector2 rayOrigin,
        Vector2 rayDirection,
        Vector2 segmentA,
        Vector2 segmentB,
        out Vector2 point,
        out float rayDistance)
    {
        point = default;
        rayDistance = 0f;

        Vector2 segmentDirection = segmentB - segmentA;

        float cross = Cross(rayDirection, segmentDirection);

        // Луч и ребро параллельны
        if (Mathf.Abs(cross) < 0.000001f)
            return false;

        Vector2 delta = segmentA - rayOrigin;

        float t = Cross(delta, segmentDirection) / cross;
        float u = Cross(delta, rayDirection) / cross;

        // t — расстояние вдоль луча
        // u — положение на отрезке [0..1]
        if (t < 0f || u < 0f || u > 1f)
            return false;

        point = rayOrigin + rayDirection * t;
        rayDistance = t;

        return true;
    }

    private static float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }
}