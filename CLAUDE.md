# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

A small 2D top-down game built on **MonoGame 3.8** (DesktopGL) targeting **.NET 9**. It is structured as a custom ECS-style engine (under `Engine/`) with the game wiring living in `LittleTestGame.cs`. Notable features: a custom deferred 2D lighting + shadow pipeline (HLSL shaders), auto-tiling, and a camera/visibility system.

## Commands

```bash
dotnet build                 # Build (also compiles Content/ via MonoGame.Content.Builder.Task)
dotnet run                   # Build + run the game
./scripts/lint.sh            # Format the whole solution (dotnet format)
dotnet format                # Same as lint.sh
```

There is **no test project** — do not assume `dotnet test` runs anything.

Code style is enforced by `.editorconfig` (4-space indent, 120 col max line, nullable enabled). Run `./scripts/lint.sh` before considering a change done.

## Architecture

The engine is a hand-rolled ECS. Three concepts hold it together:

### SystemManager — DI container + game loop driver (`Engine/ECS/SystemManager.cs`)
Acts as both a service locator and the orchestrator. Key behaviours:
- `Register<T>(instance)` adds a singleton to the service collection keyed by its **compile-time** type `T` (so register/resolve by the concrete type — interfaces won't resolve unless registered as that interface).
- `Construct<T>()` reflectively instantiates `T`, resolving each constructor parameter from the service collection. This is how systems get their dependencies. Construction is **eager and order-dependent**: a dependency must be `Register`ed before anything that `Construct`s against it.
- `AddSystem<T>()` constructs a system and enrolls it into the update and/or render loops based on which interfaces it implements.
- `Update()` runs `IUpdateSystem`s in registration order. `Render()` runs `IRenderSystem`s ordered by `IRenderSystemOrder.RenderOrder` (systems without it sort last).

`LittleTestGame.LoadContent()` is the composition root — it registers services, constructs renderers, creates all entities, and adds systems **in dependency order**. New systems/renderers are wired up here.

### Entities & Components (`Engine/ECS/Entity.cs`, `EntityManager.cs`)
- An `Entity` is an id plus a `Dictionary<Type, List<IComponent>>` — a single entity can hold **multiple components of the same type** (e.g. the player has two `RenderingComponent`s: sprite + shadow). `GetComponent<T>()` returns the first; `GetComponents<T>()` returns all.
- `EntityManager` is the queryable store: `GetEntitiesWithComponent<T>()` / `GetEntitiesWithComponents(params Type[])` are the standard way systems find their inputs.
- Components are plain data (`IComponent`); all logic lives in systems.

### Systems (`Engine/Systems/`, `Engine/Lighting/`)
A system implements `IUpdateSystem` (per-frame logic) and/or `IRenderSystem` (drawing), pulling entities from `EntityManager` and reading/mutating components. Cross-frame UI-ish flags (debug overlay, lighting debug view) are stored in the shared `StateManager` (`Engine/ECS/StateManager.cs`), a simple string-keyed bool store toggled by input in `PlayerSystem`.

## Rendering & Lighting Pipeline

Rendering is **deferred and multi-pass**, split across two ordered render systems and reusable renderer helpers (`Engine/Rendering/`). Order matters:

1. **`LightSystem` (RenderOrder 0)** — renders shadow-casting sprites into an occluder texture, then runs `Effects/ShadowMapEffect.fx` per light to produce a 1D shadow map (`RenderTarget2D` of 512×1) per light entity, stored in `LightSystem.ShadowMaps`.
2. **`RenderingSystem` (RenderOrder 1)** — renders the scene (tiles then sprites) into a scene buffer, accumulates each visible light's contribution into a lighting buffer (over an ambient base colour) using `Effects/LightingEffect.fx` + the shadow maps, then composites scene × lighting onto the back buffer using a custom multiply `BlendState`.

`DebugTextSystem` draws on top. Pressing **P** toggles `renderLightingMap` to view the raw lighting buffer; **`** (backtick/tilde) toggles the debug overlay (FPS).

Sprites are depth-sorted by world Y (`positionComponent.Position.Y / MAX_WORLD_HEIGHT`) via `SpriteSortMode.FrontToBack` for top-down layering. `RenderingComponent.CastsShadow` controls occluder inclusion; the `SpriteRenderer.RenderSprites(filter)` overload is how the lighting pass selects occluders.

HLSL shaders live in `Content/Effects/` and are compiled by the content pipeline.

## Tiling & Auto-tiling

- A `MapComponent` holds a 2D `int[,]` grid plus a `TileMapDefinition` (`WallTileDefinitions.cs` is the concrete example). Each `TileDefinition` declares which `TileNeighbours` bitmask it matches plus an optional `RectangleF` bounding box.
- `MapSystem` (update) lazily spawns one child entity per grid cell (id pattern `map:{mapId}:{x}x{y}`), then on each pass uses `TileDefinitionMatcher` to pick the correct tile sprite (source rectangle into the tile sheet) and bounding box based on neighbouring filled cells — this is the auto-tiling. Empty cells get their `RenderingComponent` removed.

## Content Pipeline

Assets are declared in `Content/Content.mgcb` and built automatically during `dotnet build` via `MonoGame.Content.Builder.Task`. **Adding a new texture/shader/font means adding an entry to `Content.mgcb`** (the `.mgcb` is editable by hand or via the MGCB editor); loading is then `Content.Load<Texture2D>("name")` (no extension). Built output lands in `Content/bin/`.

## Conventions

- Engine code uses the `Engine.*` namespaces mirroring folders; game entry types (`LittleTestGame`, `Program`, `WallTileDefinitions`) sit at the root with no namespace.
- C# 12 collection expressions (`[]`, `[.. x]`) and target-typed `new` are used throughout — match that style.
- `Engine/Utils/` holds extension helpers (`GraphicsDeviceExtensions.WithRenderTarget`, `SpriteBatchExtensions`, `KeyboardHandler` for edge-triggered key presses) — prefer reusing these over re-implementing.
