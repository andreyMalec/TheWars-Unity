using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public static class Ext {
    public static Vector2 ToVector2(this Vector3 v) {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 ToVector2(this Vector2 v) {
        return new Vector3(v.x, v.y, 0f);
    }

    public static AttackTicks[] AttackTicks([CanBeNull] this AnimationClip clip) {
        if (clip == null) return null;
        var events = AnimationUtility.GetAnimationEvents(clip);
        var attackTicks = new List<AttackTicks>();
        var prevEvent = 0f;
        for (int i = 0; i < events.Length; i++) {
            var e = events[i];
            if (e.functionName == "Execute") {
                attackTicks.Add(new AttackTicks(AttackTickType.Execute,
                    Mathf.RoundToInt((e.time - prevEvent) * Simulation.TickRate)));
                prevEvent = e.time;
                if (i == events.Length - 1) {
                    attackTicks.Add(new AttackTicks(AttackTickType.Recovery,
                        Mathf.RoundToInt((clip.length - e.time) * Simulation.TickRate)));
                }
            }
        }

        return attackTicks.ToArray();
    }
}