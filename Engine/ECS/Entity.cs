using System;
using System.Collections.Generic;

namespace Engine.ECS;

public class Entity
{
    public string Id { get; }

    private readonly Dictionary<Type, IComponent> _components;

    public Entity(string id)
    {
        Id = id;
        _components = new Dictionary<Type, IComponent>();
    }

    public void AddComponent<T>(T component) where T : IComponent => _components[typeof(T)] = component;
    public bool HasComponent<T>() where T : IComponent => _components.ContainsKey(typeof(T));
    public void RemoveComponent<T>() where T : IComponent => _components.Remove(typeof(T));
    public T GetComponent<T>() where T : class, IComponent => (T)_components[typeof(T)];
}