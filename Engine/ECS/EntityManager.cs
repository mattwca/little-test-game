using System;
using System.Collections.Generic;

namespace Engine.ECS;

public class EntityManager
{
    public List<Entity> Entities { get; }

    public EntityManager()
    {
        Entities = [];
    }

    public Entity CreateEntity(string id)
    {
        var entity = new Entity(id);
        Entities.Add(entity);
        return entity;
    }

    public void RemoveEntity(string id) => Entities.RemoveAll(entity => entity.Id == id);

    public Entity? GetEntity(string id) => Entities.Find(entity => entity.Id == id);

    public List<Entity> GetEntitiesWithComponent<T>() where T : IComponent => Entities.FindAll((entity) => entity.HasComponent<T>());

    public List<Entity> GetEntitiesWithComponents(params Type[] components) => Entities.FindAll((entity) => entity.HasComponents(components));
    public bool HasEntity(string id) => GetEntity(id) != null;
}