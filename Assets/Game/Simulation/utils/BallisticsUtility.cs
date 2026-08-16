using UnityEngine;

public static class BallisticsUtility {
    private const float Epsilon = 0.0001f;

    public static Vector2 ResolveTargetVelocity(Frame fr, int targetEntityId) {
        if (fr.TryGetUnit(targetEntityId, out var unit, out var config) && unit.IsMoving) {
            var destination = fr.GetEnemyBasePosition(unit);
            var toDestination = destination - unit.Position;
            if (toDestination.sqrMagnitude > Epsilon) {
                return toDestination.normalized * config.speed;
            }
        }

        return Vector2.zero;
    }

    public static Vector2 CalculateAimedBallisticDirection(
        Vector2 origin,
        Vector2 targetPosition,
        Vector2 targetVelocity,
        float launchSpeed,
        float gravity,
        bool preferredHighArc,
        bool autoSwitchArcRoot
    ) {
        var toTarget = targetPosition - origin;
        var direction = toTarget.sqrMagnitude > Epsilon ? toTarget.normalized : Vector2.right;

        if (launchSpeed <= Epsilon || gravity <= Epsilon || toTarget.sqrMagnitude <= Epsilon) {
            return direction;
        }

        var interceptTime = CalculateInterceptTime(toTarget, targetVelocity, launchSpeed);

        for (var i = 0; i < 2; i++) {
            var predictedPosition = targetPosition + targetVelocity * interceptTime;
            var toPredicted = predictedPosition - origin;
            if (!TryCalculateBallisticLaunchWithFallback(toPredicted, launchSpeed, gravity, preferredHighArc,
                    autoSwitchArcRoot,
                    out var ballisticDirection, out var flightTime)) {
                if (toPredicted.sqrMagnitude > Epsilon) {
                    direction = toPredicted.normalized;
                }

                break;
            }

            direction = ballisticDirection;
            interceptTime = flightTime;
        }

        return direction;
    }

    public static bool TryCalculateBallisticLaunchWithFallback(
        Vector2 toTarget,
        float launchSpeed,
        float gravity,
        bool preferredHighArc,
        bool autoSwitchArcRoot,
        out Vector2 direction,
        out float flightTime
    ) {
        if (TryCalculateBallisticLaunch(toTarget, launchSpeed, gravity, preferredHighArc, out direction, out flightTime)) {
            return true;
        }

        if (!autoSwitchArcRoot) {
            return false;
        }

        return TryCalculateBallisticLaunch(toTarget, launchSpeed, gravity, !preferredHighArc, out direction,
            out flightTime);
    }

    public static bool TryCalculateBallisticLaunch(
        Vector2 toTarget,
        float launchSpeed,
        float gravity,
        bool highArc,
        out Vector2 direction,
        out float flightTime
    ) {
        direction = Vector2.zero;
        flightTime = 0f;

        if (toTarget.sqrMagnitude <= Epsilon || launchSpeed <= Epsilon || gravity <= Epsilon) {
            return false;
        }

        var dx = toTarget.x;
        var dy = toTarget.y;
        var absDx = Mathf.Abs(dx);
        if (absDx <= Epsilon) {
            return false;
        }

        var speedSq = launchSpeed * launchSpeed;
        var discriminant = speedSq * speedSq - gravity * (gravity * absDx * absDx + 2f * dy * speedSq);
        if (discriminant < 0f) {
            return false;
        }

        var sqrtDiscriminant = Mathf.Sqrt(discriminant);
        var numerator = highArc ? speedSq + sqrtDiscriminant : speedSq - sqrtDiscriminant;
        var tanTheta = numerator / (gravity * absDx);
        var cosTheta = 1f / Mathf.Sqrt(1f + tanTheta * tanTheta);
        if (cosTheta <= Epsilon) {
            return false;
        }

        var sinTheta = tanTheta * cosTheta;
        var horizontalSign = dx >= 0f ? 1f : -1f;
        direction = new Vector2(cosTheta * horizontalSign, sinTheta);
        flightTime = absDx / (launchSpeed * cosTheta);
        return true;
    }

    public static float CalculateInterceptTime(Vector2 toTarget, Vector2 targetVelocity, float projectileSpeed) {
        var speedSq = projectileSpeed * projectileSpeed;
        var targetSpeedSq = targetVelocity.sqrMagnitude;
        var a = targetSpeedSq - speedSq;
        var b = 2f * Vector2.Dot(targetVelocity, toTarget);
        var c = toTarget.sqrMagnitude;

        if (Mathf.Abs(a) <= Epsilon) {
            if (Mathf.Abs(b) <= Epsilon) {
                return 0f;
            }

            var linearTime = -c / b;
            return Mathf.Max(0f, linearTime);
        }

        var discriminant = b * b - 4f * a * c;
        if (discriminant < 0f) {
            return 0f;
        }

        var sqrtDiscriminant = Mathf.Sqrt(discriminant);
        var t1 = (-b - sqrtDiscriminant) / (2f * a);
        var t2 = (-b + sqrtDiscriminant) / (2f * a);

        var hasPositiveTime = false;
        var bestTime = 0f;
        if (t1 > 0f) {
            bestTime = t1;
            hasPositiveTime = true;
        }

        if (t2 > 0f) {
            bestTime = hasPositiveTime ? Mathf.Min(bestTime, t2) : t2;
            hasPositiveTime = true;
        }

        return hasPositiveTime ? bestTime : 0f;
    }
}

