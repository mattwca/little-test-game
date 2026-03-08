using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Engine.ECS;

public class Entity
{
    public string Id { get; }

    private readonly Dictionary<Type, List<IComponent>> _components;

    public Entity(string id)
    {
        Id = id;
        _components = new Dictionary<Type, List<IComponent>>();
    }

    public Entity AddComponent<T>(T component) where T : IComponent
    {
        var componentType = typeof(T);

        if (_components.ContainsKey(componentType))
        {
            _components[componentType].Add(component);
            return this;
        }

        _components[componentType] = [component];
        return this;
    }

    public bool HasComponent<T>() where T : IComponent => _components.ContainsKey(typeof(T));
    public bool HasComponents(params Type[] components) => components.All(c => _components.ContainsKey(c));

    public void RemoveComponent<T>(T component) where T : IComponent
    {
        var componentType = typeof(T);

        if (!HasComponent<T>())
        {
            return;
        }

        _components[componentType].Remove(component);

        if (_components[componentType].Count == 0)
        {
            _components.Remove(componentType);
        }
    }

    public T GetComponent<T>() where T : IComponent
    {
        if (_components.ContainsKey(typeof(T)))
        {
            return (T)_components[typeof(T)].First();
        }

        throw new Exception($"Entity {Id} does not have component of type {typeof(T)}");
    }

    public T[] GetComponents<T>() where T : IComponent
    {
        if (_components.ContainsKey(typeof(T)))
        {
            return _components[typeof(T)].Cast<T>().ToArray();
        }

        throw new Exception($"Entity {Id} does not have component of type {typeof(T)}");
    }
}