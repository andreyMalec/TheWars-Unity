# Project Architecture

## Overview

This project is a multiplayer auto battler built with Unity.

The gameplay consists of two opposing teams controlling a base. Players can issue commands such as constructing buildings, upgrading their base and spawning autonomous units.

The project architecture is intentionally inspired by Photon Quantum, but implemented using Unity Netcode for GameObjects (NGO) as the networking layer.

The networking library must **not** contain game logic. It is only responsible for delivering commands and synchronizing game state when necessary.

---

# Core Principles

## Simulation First

The entire game runs inside a deterministic simulation.

Unity GameObjects are only visual representations of the simulation state.

Never implement gameplay directly inside MonoBehaviours or NetworkBehaviours.

Instead:

```
Player Input
      ↓
Simulation
      ↓
Frame
      ↓
Presentation Layer
      ↓
Unity Objects
```

---

## Networking is Transport

NGO is used only as a transport layer.

It should be responsible for:

- sending player commands
- synchronizing players
- session management
- optional snapshots

NGO should NOT contain gameplay logic.

Avoid:

- NetworkTransform
- NetworkVariable-driven gameplay
- RPC chains that directly modify gameplay

---

## Tick-Based Simulation

The game runs on a fixed simulation tick.

Example:

```
60 ticks / second
```

Each tick executes every simulation system in a deterministic order.

```
Tick

↓

EconomySystem

↓

BuildSystem

↓

SpawnSystem

↓

MovementSystem

↓

TargetSystem

↓

WeaponSystem

↓

DamageSystem

↓

DeathSystem
```

No gameplay code should execute outside the simulation tick.

---

# Project Layers

```
Game
│
├── Simulation
│
├── Networking
│
├── Presentation
│
└── Unity Bootstrap
```

---

# Simulation

The simulation is a pure C# layer.

It must not depend on:

- MonoBehaviour
- Transform
- GameObject
- Animator
- Physics callbacks
- UI

It should be possible to execute the simulation inside unit tests without Unity.

---

# Frame

The current game state is stored inside a Frame.

Frame represents the world at one simulation tick.

Example:

```
Frame
    Tick

    Bases
    Units
    Turrets
    Projectiles

    RandomState
```

The Frame is mutated every tick.

Do not recreate Frame every update.

---

# State vs Configuration

Separate immutable game data from runtime state.

Example:

```
UnitConfig

Speed
Damage
AttackRange
Prefab
Cost
```

Runtime:

```
UnitState

Id
Team
Position
Health
Cooldown
Target
```

Simulation reads Config data but only modifies State.

---

# Systems

Gameplay is implemented through Systems.

Each system has a single responsibility.

Example:

```
EconomySystem

BuildSystem

SpawnSystem

MovementSystem

TargetSystem

AttackSystem

DamageSystem

DeathSystem
```

Systems should communicate through the Frame instead of directly calling each other.

---

# Commands

Players do not directly modify the world.

Players submit Commands.

Example:

```
SpawnUnit

BuildTower

UpgradeBase
```

Commands are executed by the simulation on the appropriate tick.

---

# Presentation Layer

Presentation reads the simulation state and updates Unity objects.

Responsibilities:

- moving transforms
- animations
- particles
- sounds
- VFX
- UI

Presentation must never contain gameplay decisions.

Bad:

```
if (enemyInRange)
    Shoot();
```

Good:

```
if (Frame says projectile was created)
    Play muzzle flash.
```

---

# Networking

Networking sends Commands between players.

The simulation remains authoritative over gameplay.

Networking should not know:

- HP
- Damage
- AI
- Target selection

Networking only transports data.

---

# Entity IDs

Every runtime entity must have a unique integer ID.

Example:

```
Unit

Id = 15
```

The ID is used for:

- references
- targeting
- synchronization
- debugging

Avoid storing direct object references inside simulation state.

---

# Determinism

Gameplay should be deterministic whenever practical.

Avoid:

- UnityEngine.Random
- Time.deltaTime
- Update()

Prefer:

- Simulation Tick
- Tick Delta Time
- Simulation Random

---

# Code Organization

Suggested structure:

```
Assets

/GameLogic
    Frame
    Simulation
    Systems
    Commands
    Components
    Config
    Utilities

/Networking
    NGOAdapter
    CommandSender
    CommandReceiver

/Presentation
    Views
    Animation
    Effects
    UI

/Bootstrap
    GameStartup
```

---

# Responsibilities

Simulation owns:

- gameplay
- AI
- combat
- economy
- movement
- spawning

Presentation owns:

- visuals
- animation
- sounds
- camera
- UI

Networking owns:

- transport
- player connections
- command delivery

---

# Long-Term Goals

The architecture should make it possible to add in the future:

- replay system
- rollback
- spectator mode
- AI players
- save/load
- LAN multiplayer
- alternative networking transports
- dedicated server

These features should require minimal changes to gameplay code due to the separation between Simulation, Networking and Presentation.