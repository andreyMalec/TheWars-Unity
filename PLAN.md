# DEVELOPMENT_ROADMAP.md

# Development Roadmap

This document defines the implementation order for the project.

The goal is to build a minimal playable prototype first and expand the architecture incrementally.

---

# Phase 1 — Project Foundation

## Goal

Create the project structure and the core architecture.

## Tasks

- Create Unity project
- Configure NGO
- Create project folder structure
- Create Assembly Definitions
- Configure bootstrap scene
- Implement game startup pipeline

Expected result:

- Project launches
- Simulation can be initialized
- Networking can be started
- No gameplay yet

---

# Phase 2 — Simulation Core

## Goal

Implement the simulation framework.

## Tasks

Create:

- Frame
- Simulation
- TickManager
- EntityId generator

Create interfaces:

```
ISystem
```

```
ICommand
```

Simulation loop:

```
Tick

↓

Execute Commands

↓

Run Systems

↓

Increase Tick
```

Expected result:

Simulation can execute empty ticks.

---

# Phase 3 — Runtime State

## Goal

Implement runtime data structures.

## Tasks

Create states:

- BaseState
- UnitState
- TurretState
- ProjectileState

Create collections inside Frame.

Implement:

- Add Entity
- Remove Entity
- Find Entity
- Generate IDs

Expected result:

Entities can exist inside the simulation.

---

# Phase 4 — Static Config

## Goal

Separate runtime state from configuration.

## Tasks

Create configs:

- UnitConfig
- TurretConfig
- BaseConfig

Implement ConfigDatabase.

Simulation should reference ConfigId instead of storing gameplay values inside states.

Expected result:

Simulation reads immutable configs.

---

# Phase 5 — Commands

## Goal

Implement the command pipeline.

## Tasks

Create command queue.

Implement:

- SpawnUnitCommand
- BuildTurretCommand
- UpgradeBaseCommand

Commands should execute only inside Simulation.

Expected result:

Player actions become simulation commands.

---

# Phase 6 — Economy

## Goal

Implement resource management.

## Tasks

EconomySystem

Responsibilities:

- generate income
- spend resources
- validate purchases

Expected result:

Simulation controls economy.

---

# Phase 7 — Unit Spawning

## Goal

Spawn autonomous units.

## Tasks

SpawnSystem

Responsibilities:

- validate spawn
- create UnitState
- initialize stats
- assign ID

Expected result:

Units appear inside Frame.

---

# Phase 8 — Movement

## Goal

Move units.

## Tasks

MovementSystem

Responsibilities:

- movement
- lane following
- destination updates

No Unity navigation.

Movement exists only inside simulation.

Expected result:

Units move every tick.

---

# Phase 9 — Target Selection

## Goal

Implement combat targeting.

## Tasks

TargetSystem

Responsibilities:

- search enemies
- select nearest target
- validate targets

Expected result:

Units acquire targets.

---

# Phase 10 — Combat

## Goal

Implement attacks.

## Tasks

WeaponSystem

Responsibilities:

- cooldowns
- attack timing
- projectile creation

Expected result:

Units attack automatically.

---

# Phase 11 — Projectiles

## Goal

Implement projectile simulation.

## Tasks

ProjectileSystem

Responsibilities:

- movement
- collision
- lifetime

Expected result:

Projectiles exist completely inside simulation.

---

# Phase 12 — Damage

## Goal

Apply damage.

## Tasks

DamageSystem

Responsibilities:

- damage calculation
- health reduction
- death events

Expected result:

Entities lose HP.

---

# Phase 13 — Death

## Goal

Destroy entities.

## Tasks

DeathSystem

Responsibilities:

- remove dead entities
- cleanup references
- notify presentation

Expected result:

Dead units disappear.

---

# Phase 14 — Presentation

## Goal

Visualize simulation.

## Tasks

Create:

- UnitView
- TurretView
- ProjectileView
- BaseView

Implement:

Frame

↓

Presentation

↓

Unity Objects

Presentation must never modify simulation.

Expected result:

Simulation becomes visible.

---

# Phase 15 — Networking

## Goal

Connect multiple players.

## Tasks

Implement NGO adapter.

Networking responsibilities:

- send Commands
- receive Commands
- synchronize players

Networking must never execute gameplay.

Expected result:

Players can issue commands over the network.

---

# Phase 16 — Game Loop

## Goal

Create a playable match.

## Tasks

Implement:

- game start
- victory
- defeat
- restart

Expected result:

Playable multiplayer prototype.

---

# Phase 17 — Polish

## Tasks

- animations
- particles
- sounds
- UI
- balancing
- optimization

---

# Future Work

Possible future features:

- Rollback
- Replay
- Save / Load
- AI opponents
- Spectator mode
- Dedicated server
- Alternative transports
- Bluetooth transport
- LAN discovery

---

# Definition of Done

Every new feature should satisfy the following:

- Implemented inside Simulation whenever possible.
- Independent from Unity objects.
- Independent from NGO.
- Testable without rendering.
- Presentation only visualizes state.
- Networking only transports commands.

Architecture is always more important than feature speed.