# Game Layer Bootstrap (Phases 1-14)

Implemented foundation for `ARCHITECTURE.md` and first fourteen phases from `PLAN.md`:

- Bootstrap pipeline that creates and ticks simulation.
- Pure simulation core (`Frame`, `World`, `Simulation`, `TickManager`).
- Runtime state entities (`BaseState`, `UnitState`, `TurretState`, `ProjectileState`).
- Static config layer via `ScriptableObject` configs + `ConfigDatabase`.
- Command queue + commands (`SpawnUnit`, `BuildTurret`, `UpgradeBase`).
- Tick systems for economy, spawning, movement, targeting, weapon, projectile, damage and death.
- Presentation layer (`FramePresenter` + `BaseView`/`UnitView`/`TurretView`/`ProjectileView`) that only reads `Frame`.

## Quick setup in Unity

1. Create a `ConfigDatabase` asset via `Create/Game/Config Database`.
2. Add `GameStartup` component to a startup scene object.
3. Assign the `ConfigDatabase` asset in `GameStartup`.
4. Configure `initialBaseConfigIds`, `initialBaseTeams` and `initialBasePositions` in `GameStartup`.
5. Press Play.

`GameStartup` creates `SimulationRunner` and `FramePresenter`.

`SimulationRunner` advances deterministic ticks in `FixedUpdate`, while `FramePresenter` visualizes simulation state in `LateUpdate`.

