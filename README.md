[README (10).md](https://github.com/user-attachments/files/35389262/README.10.md)
# Convoy *(working title — swap this for whatever you two land on)*

A 4-player co-op survival game built in Unity. You're driving a truck through bandit territory with a crew, trying to hit quota and get out alive. No GPS, just a paper map one player has to read out loud while someone else is driving and everyone else is getting shot at.

Built with a friend. In development, no prior game engine experience going in.

## Status

Early WIP. The project scaffolding is in place (truck controller, role system, combat, economy, enemy AI, networking setup) but the core loop isn't playable end-to-end yet. This is a learning project from scratch, not a finished build.

## Stack

- Unity 6 (6000.3.23f1)
- C#
- [Mirror](https://mirror-networking.com/) for netcode (host/client multiplayer)
- Unity's new Input System

## Core concept

- 4 players share one truck cabin: **Driver, Navigator, Gunner, Engineer**
- No GPS. The Navigator holds the only map and calls out turns to the Driver in real time.
- Bandits raid convoys on the road, including helicopter snipers. Everyone, including the Driver, can lean out and fight back with a pistol.
- The truck breaks down mid-route. The Engineer has to repair it under fire, or anyone else can step in slower if the Engineer goes down.
- **Loose roles**: anyone can take any role, but each player levels their own role stats independently. A Driver you've leveled is not the same as one you haven't.
- **Sanity/Trauma**: drive too long and the Driver starts hallucinating input errors; gun down enough enemies and Trauma builds for the Gunner. Sleeping in the cabin brings both down.
- Earn money hauling cargo, spend it upgrading the truck (mounted guns, a plow, a sidecar) or unlocking new transport modes.

Planned future modes, same core loop, different flavor:
- **Train** — Wild West themed, bandits on horseback, lassos
- **Boat** — 1600s pirate themed, cannonballs

Full design notes: see [`docs/design-doc.md`](docs/design-doc.md).

## Roadmap

**Phase 1 — Vertical slice (solo)**
- [ ] Truck driving feels right (acceleration, braking, automatic gearing)
- [ ] One weapon, one enemy type that fights back
- [ ] Win/lose condition on a single short route

**Phase 2 — Networked co-op**
- [ ] Mirror integration for 2–4 players sharing one truck
- [ ] Driver / Gunner / Navigator roles functional and synced across clients

**Phase 3 — Core systems**
- [ ] Engineer role + breakdown/repair-under-fire loop
- [ ] Per-player role stats and progression
- [ ] Sanity/Trauma meters and their gameplay effects (hallucinated inputs, proximity chat glitches)

**Phase 4 — Economy & structure**
- [ ] Job board, quotas, payout
- [ ] Item shop with limited, timed stock
- [ ] Convenience store resupply runs + self-funded gas economy

**Phase 5 — Content**
- [ ] Slums map, forest highway map
- [ ] Weather: fog, blood moon, thunderstorm, rain

**Phase 6 — Train mode** (Wild West)

**Phase 7 — Boat mode** (Pirates)

## Why this project

This started as a late-night brainstorm and turned into a full design document before a single line of code was written: interdependent player roles, a hallucination/trauma system tied directly to gameplay inputs, an economy that actively punishes you for stopping. Still early, but it's a real exercise in systems-level game design and multiplayer architecture built from zero prior engine experience: networked physics, state synchronization across player roles, and a C# codebase organized by system rather than by scene.

## Getting started

```
git clone https://github.com/eknoorsidhu2007-pixel/mvp-for-game.git
```

1. Install Unity Hub and Unity **6000.3.23f1** (exact version matters for Unity 6 projects)
2. Open the cloned folder as a project in Unity Hub
3. Mirror is already vendored under `Assets/Mirror`, no package install needed
4. Gameplay scripts live under `Assets/_Scripts`, organized by system (see below)

## Project structure

```
Assets/
  _Scripts/
    Combat/       weapons, projectiles
    Economy/      cargo, payout
    Enemies/      bandit + monster AI
    GameLoop/     day/night cycle, game phases
    Navigation/   routes, waypoints
    Networking/   Mirror NetworkManager + connections
    Roles/        driver / navigator / gunner / engineer
    Truck/        truck controller, health, breakdowns
  Mirror/         Mirror networking library (vendored)
```
