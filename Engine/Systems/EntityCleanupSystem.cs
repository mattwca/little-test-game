using System;
using System.Collections.Generic;
using Engine.ECS;
using Microsoft.Xna.Framework;

namespace Engine.Systems;

public class EntityCleanupSystem : IUpdateSystem
{
    private readonly EntityManager _entityManager;
    private readonly Queue<string> _entitiesToDelete;

    public EntityCleanupSystem(EntityManager entityManager)
    {
        _entityManager = entityManager;
        _entitiesToDelete = new Queue<string>();
    }

    public void MarkForCleanup(string entityId)
    {
        _entitiesToDelete.Enqueue(entityId);
    }

    public void Update(GameTime gameTime)
    {
        if (_entitiesToDelete.Count == 0)
        {
            return;
        }

        while (_entitiesToDelete.TryDequeue(out var entityId))
        {
            _entityManager.RemoveEntity(entityId);
        }
    }
}
