using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public sealed class ProjectileView : MonoBehaviour {
    private const float Epsilon = 0.0001f;

    public int EntityId;
    [Expandable] public ProjectileConfig projectileConfig;

    [Header("Trajectory Gizmos")]
    [SerializeField] private bool drawTrajectory = true;
    [SerializeField] private int maxHistoryPoints = 64;
    [SerializeField] private int futureSteps = 24;
    [SerializeField] private float futureStepTime = 0.08f;

    private readonly List<Vector3> _historyPoints = new();
    private readonly List<Vector3> _futurePoints = new();
    private bool _hasState;

    public void Bind(int entityId, ProjectileConfig config) {
        EntityId = entityId;
        projectileConfig = config;
        name = config.name + "_" + entityId;

        _hasState = false;
        _historyPoints.Clear();
        _futurePoints.Clear();
    }

    public void Present(ProjectileState state) {
        var scaleX = state.Direction.x < 0 ? -1f : 1f;
        transform.localScale = new Vector3(scaleX, 1f, 1f);

        var current = new Vector3(state.Position.x, state.Position.y, 0f);
        transform.position = current;

        if (_historyPoints.Count == 0 || (current - _historyPoints[^1]).sqrMagnitude > Epsilon) {
            _historyPoints.Add(current);
            if (_historyPoints.Count > maxHistoryPoints) {
                _historyPoints.RemoveAt(0);
            }
        }

        _hasState = true;
        RebuildFutureTrajectory(state);
    }

    private void OnDisable() {
        _hasState = false;
        _historyPoints.Clear();
        _futurePoints.Clear();
    }

    private void OnDrawGizmos() {
        if (!drawTrajectory || !_hasState) {
            return;
        }

        Gizmos.color = Color.green;
        for (var i = 1; i < _historyPoints.Count; i++) {
            Gizmos.DrawLine(_historyPoints[i - 1], _historyPoints[i]);
        }

        Gizmos.color = Color.blue;
        for (var i = 1; i < _futurePoints.Count; i++) {
            Gizmos.DrawLine(_futurePoints[i - 1], _futurePoints[i]);
        }
    }

    private void RebuildFutureTrajectory(ProjectileState state) {
        _futurePoints.Clear();

        var position = state.Position;
        var velocity = state.Velocity;
        if (velocity.sqrMagnitude <= Epsilon && state.Direction.sqrMagnitude > Epsilon) {
            velocity = state.Direction * state.Speed;
        }

        var remainingLifetime = state.Lifetime;
        _futurePoints.Add(new Vector3(position.x, position.y, 0f));

        for (var i = 0; i < futureSteps && remainingLifetime > 0f; i++) {
            var step = Mathf.Min(futureStepTime, remainingLifetime);
            if (state.Type == ProjectileType.Ballistic) {
                velocity += Vector2.down * (projectileConfig.gravity * step);
            }

            position += velocity * step;
            _futurePoints.Add(new Vector3(position.x, position.y, 0f));
            remainingLifetime -= step;
        }
    }
}