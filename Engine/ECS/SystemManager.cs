using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework.Graphics;

namespace Engine.ECS;

public class SystemManager
{
    public EntityManager EntityManager { get; }

    private readonly Dictionary<Type, object> _serviceCollection = new();
    private readonly List<IUpdateSystem> _updateSystems;
    private readonly List<IRenderSystem> _renderSystems;

    public SystemManager()
    {
        _serviceCollection = new Dictionary<Type, object>();

        EntityManager = new EntityManager();
        Register(EntityManager);

        _updateSystems = [];
        _renderSystems = [];
    }

    public SystemManager Register<T>(T dependency) where T : class
    {
        _serviceCollection[typeof(T)] = dependency;
        return this;
    }

    public T Construct<T>() where T : class
    {
        var type = typeof(T);
        var constructor = type.GetConstructors().FirstOrDefault()
            ?? throw new InvalidOperationException($"{type.Name} has no public constructor.");

        var parameters = constructor.GetParameters()
            .Select(p => Resolve(p.ParameterType))
            .ToArray();

        return (T)constructor.Invoke(parameters);
    }

    public T AddSystem<T>() where T : class
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

    public void Update(float deltaTime)
    {
        _updateSystems.ForEach(system => system.Update(deltaTime));
    }

    public void Render(float deltaTime)
    {
        _renderSystems.ForEach(system => system.Draw(deltaTime));
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