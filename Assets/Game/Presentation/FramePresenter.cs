using System.Collections.Generic;
using UnityEngine;

public sealed class FramePresenter : MonoBehaviour
{
    private readonly Dictionary<int, BaseView> _baseViews = new Dictionary<int, BaseView>();
    private readonly Dictionary<int, UnitView> _unitViews = new Dictionary<int, UnitView>();
    private readonly Dictionary<int, TurretView> _turretViews = new Dictionary<int, TurretView>();
    private readonly Dictionary<int, ProjectileView> _projectileViews = new Dictionary<int, ProjectileView>();
    private readonly List<int> _removeBuffer = new List<int>();

    private Simulation _simulation;

    public void Initialize(Simulation simulation)
    {
        _simulation = simulation;
    }

    private void LateUpdate()
    {
        if (_simulation == null) return;
        PresentBases();
        PresentUnits();
        PresentTurrets();
        PresentProjectiles();
    }

    private void PresentBases()
    {
        var bases = _simulation.Frame.World.Bases;
        foreach (var pair in bases)
        {
            var view = GetOrCreateBaseView(pair.Key);
            view.Present(pair.Value);
        }

        CleanupMissing(_baseViews, bases);
    }

    private void PresentUnits()
    {
        var units = _simulation.Frame.World.Units;
        foreach (var pair in units)
        {
            var view = GetOrCreateUnitView(pair.Key);
            view.Present(pair.Value);
        }

        CleanupMissing(_unitViews, units);
    }

    private void PresentTurrets()
    {
        var turrets = _simulation.Frame.World.Turrets;
        foreach (var pair in turrets)
        {
            var view = GetOrCreateTurretView(pair.Key);
            view.Present(pair.Value);
        }

        CleanupMissing(_turretViews, turrets);
    }

    private void PresentProjectiles()
    {
        var projectiles = _simulation.Frame.World.Projectiles;
        foreach (var pair in projectiles)
        {
            var view = GetOrCreateProjectileView(pair.Key);
            view.Present(pair.Value);
        }

        CleanupMissing(_projectileViews, projectiles);
    }

    private BaseView GetOrCreateBaseView(int entityId)
    {
        if (_baseViews.TryGetValue(entityId, out var view))
        {
            return view;
        }

        var viewObject = new GameObject();
        viewObject.transform.SetParent(transform, false);
        var newView = viewObject.AddComponent<BaseView>();
        newView.Bind(entityId);
        _baseViews.Add(entityId, newView);
        return newView;
    }

    private UnitView GetOrCreateUnitView(int entityId)
    {
        if (_unitViews.TryGetValue(entityId, out var view))
        {
            return view;
        }

        var viewObject = new GameObject();
        viewObject.transform.SetParent(transform, false);
        var newView = viewObject.AddComponent<UnitView>();
        newView.Bind(entityId);
        _unitViews.Add(entityId, newView);
        return newView;
    }

    private TurretView GetOrCreateTurretView(int entityId)
    {
        if (_turretViews.TryGetValue(entityId, out var view))
        {
            return view;
        }

        var viewObject = new GameObject();
        viewObject.transform.SetParent(transform, false);
        var newView = viewObject.AddComponent<TurretView>();
        newView.Bind(entityId);
        _turretViews.Add(entityId, newView);
        return newView;
    }

    private ProjectileView GetOrCreateProjectileView(int entityId)
    {
        if (_projectileViews.TryGetValue(entityId, out var view))
        {
            return view;
        }

        var viewObject = new GameObject();
        viewObject.transform.SetParent(transform, false);
        var newView = viewObject.AddComponent<ProjectileView>();
        newView.Bind(entityId);
        _projectileViews.Add(entityId, newView);
        return newView;
    }

    private void CleanupMissing<TView, TState>(Dictionary<int, TView> views, Dictionary<int, TState> source)
        where TView : MonoBehaviour
    {
        _removeBuffer.Clear();
        foreach (var pair in views)
        {
            if (!source.ContainsKey(pair.Key))
            {
                _removeBuffer.Add(pair.Key);
            }
        }

        for (var i = 0; i < _removeBuffer.Count; i++)
        {
            var id = _removeBuffer[i];
            Destroy(views[id].gameObject);
            views.Remove(id);
        }
    }
}


