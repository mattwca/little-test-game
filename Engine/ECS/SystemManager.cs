using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Configuration;
using Engine.Systems;
using Microsoft.Xna.Framework;

namespace Engine.ECS;

/// <summary>
/// The System Manager, acts as a DI container for update and render systems.
///
/// Exposes `update` and `render` methods which should be called as part of the game loop,
/// and will invoke each registered system. Systems are invoked in the order that they are
/// registered.
///
/// Note that dependencies are constructed as and when they are registered - they are not
/// invoked lazily.
/// </summary>
public class SystemManager
{
    public EntityManager EntityManager { get; }

    private readonly Dictionary<Type, object> _serviceCollection = new();
    private readonly List<IUpdateSystem> _updateSystems;
    private readonly List<IRenderSystem> _renderSystems;
    private readonly StateManager _stateManager;

    public SystemManager(GameConfiguration gameConfiguration)
    {
        _serviceCollection = new Dictionary<Type, object>();

        EntityManager = new EntityManager();
        Register(EntityManager);

        _stateManager = new StateManager();
        Register(_stateManager);

        Register(gameConfiguration);

        _updateSystems = [];
        _renderSystems = [];

        AddSystem<EntityCleanupSystem>();
    }

    public SystemManager Register<T>(T dependency)
        where T : class
    {
        _serviceCollection[typeof(T)] = dependency;
        return this;
    }

    public T Construct<T>()
        where T : class
    {
        var type = typeof(T);
        var constructor =
            type.GetConstructors().FirstOrDefault()
            ?? throw new InvalidOperationException($"{type.Name} has no public constructor.");

        var parameters = constructor.GetParameters().Select(p => Resolve(p.ParameterType)).ToArray();

        return (T)constructor.Invoke(parameters);
    }

    public T AddSystem<T>()
        where T : class
    {
        var system = Construct<T>();

        if (system is IUpdateSystem updateSystem)
        {
            _updateSystems.Add(updateSystem);
        }

        if (system is IRenderSystem renderSystem)
        {
            _renderSystems.Add(renderSystem);
        }

        Register(system);

        return system;
    }

    public void Update(GameTime gameTime)
    {
        _updateSystems.ForEach(system => system.Update(gameTime));
    }

    public void Render(GameTime gameTime)
    {
        _renderSystems
            .OrderBy(system =>
                system is IRenderSystemOrder renderSystemOrder ? renderSystemOrder.RenderOrder : int.MaxValue
            )
            .ToList()
            .ForEach(system => system.Draw(gameTime));
    }

    private object Resolve(Type type)
    {
        if (_serviceCollection.TryGetValue(type, out var service))
        {
            return service;
        }

        throw new InvalidOperationException($"No service registered for type {type.Name}.");
    }
}
