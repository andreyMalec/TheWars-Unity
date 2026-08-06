# Game Layer Bootstrap (Phases 1-5)

Implemented foundation for `ARCHITECTURE.md` and first five phases from `PLAN.md`:

- Bootstrap pipeline that creates and ticks simulation.
- Pure simulation core (`Frame`, `World`, `Simulation`, `TickManager`).
- Runtime state entities (`BaseState`, `UnitState`, `TurretState`, `ProjectileState`).
- Static config layer via `ScriptableObject` configs + `ConfigDatabase`.
- Command queue + commands (`SpawnUnit`, `BuildTurret`, `UpgradeBase`).

## Quick setup in Unity

1. Create a `ConfigDatabase` asset via `Create/Game/Config Database`.
2. Add `GameStartup` component to a startup scene object.
3. Assign the `ConfigDatabase` asset in `GameStartup`.
4. Press Play.

`GameStartup` creates `SimulationRunner`, and `SimulationRunner` advances deterministic ticks in `FixedUpdate`.

