using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class FramePresenter : MonoBehaviour {
    private readonly Dictionary<int, BaseView> _baseViews = new Dictionary<int, BaseView>();
    private readonly Dictionary<int, UnitView> _unitViews = new Dictionary<int, UnitView>();
    private readonly Dictionary<int, TurretView> _turretViews = new Dictionary<int, TurretView>();
    private readonly Dictionary<int, ProjectileView> _projectileViews = new Dictionary<int, ProjectileView>();
    private readonly List<int> _removeBuffer = new List<int>();

    private Simulation _simulation;
    private PlayerUiView _uiView;

    public void Initialize(Simulation simulation, PlayerUiView uiView) {
        _simulation = simulation;
        _uiView = uiView;
    }

    private void LateUpdate() {
        if (_simulation == null) return;
        PresentBases();
        PresentUnits();
        PresentTurrets();
        PresentProjectiles();
    }

    private void PresentBases() {
        var bases = _simulation.Frame.Bases;
        foreach (var pair in bases) {
            var view = GetOrCreateBaseView(pair.Key, _simulation.Frame.FindConfig<BaseConfig>(pair.Value.ConfigId));
            view.Present(pair.Value);
            if (pair.Value.Team == _simulation.Frame.LocalPlayerTeam())
                _uiView.Present(pair.Value);
        }

        CleanupMissing(_baseViews, bases);
    }

    private void PresentUnits() {
        var units = _simulation.Frame.Units;
        foreach (var pair in units) {
            var view = GetOrCreateUnitView(pair.Key, _simulation.Frame.FindConfig<UnitConfig>(pair.Value.ConfigId));
            view.Present(pair.Value);
        }

        CleanupMissing(_unitViews, units);
    }

    private void PresentTurrets() {
        var turrets = _simulation.Frame.Turrets;
        foreach (var pair in turrets) {
            var view = GetOrCreateTurretView(pair.Key, _simulation.Frame.FindConfig<TurretConfig>(pair.Value.ConfigId));
            view.Present(pair.Value);
        }

        CleanupMissing(_turretViews, turrets);
    }

    private void PresentProjectiles() {
        var projectiles = _simulation.Frame.Projectiles;
        foreach (var pair in projectiles) {
            var view = GetOrCreateProjectileView(pair.Key,
                _simulation.Frame.FindConfig<ProjectileConfig>(pair.Value.ConfigId));
            view.Present(pair.Value);
        }

        CleanupMissing(_projectileViews, projectiles);
    }

    private BaseView GetOrCreateBaseView(int entityId, BaseConfig config) {
        if (_baseViews.TryGetValue(entityId, out var view)) {
            return view;
        }

        var viewObject = Instantiate(config.prefab);
        var newView = viewObject.GetComponent<BaseView>();
        newView.Bind(entityId, config);
        _baseViews.Add(entityId, newView);
        return newView;
    }

    private UnitView GetOrCreateUnitView(int entityId, UnitConfig config) {
        if (_unitViews.TryGetValue(entityId, out var view)) {
            return view;
        }

        var viewObject = Instantiate(config.prefab);
        var newView = viewObject.GetComponent<UnitView>();
        newView.Bind(entityId, config);
        _unitViews.Add(entityId, newView);
        return newView;
    }

    private TurretView GetOrCreateTurretView(int entityId, TurretConfig config) {
        if (_turretViews.TryGetValue(entityId, out var view)) {
            return view;
        }

        var viewObject = Instantiate(config.prefab);
        var newView = viewObject.GetComponent<TurretView>();
        newView.Bind(entityId, config);
        _turretViews.Add(entityId, newView);
        return newView;
    }

    private ProjectileView GetOrCreateProjectileView(int entityId, ProjectileConfig config) {
        if (_projectileViews.TryGetValue(entityId, out var view)) {
            return view;
        }

        var viewObject = Instantiate(config.prefab);
        var newView = viewObject.GetComponent<ProjectileView>();
        newView.Bind(entityId, config);
        _projectileViews.Add(entityId, newView);
        return newView;
    }

    private void CleanupMissing<TView, TState>(Dictionary<int, TView> views, IReadOnlyDictionary<int, TState> source)
        where TView : MonoBehaviour {
        _removeBuffer.Clear();
        foreach (var pair in views) {
            if (!source.ContainsKey(pair.Key)) {
                _removeBuffer.Add(pair.Key);
            }
        }

        for (var i = 0; i < _removeBuffer.Count; i++) {
            var id = _removeBuffer[i];
            Destroy(views[id].gameObject);
            views.Remove(id);
        }
    }
}