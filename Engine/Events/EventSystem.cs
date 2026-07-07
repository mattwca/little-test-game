using System;
using Engine.ECS;
using Microsoft.Xna.Framework;

namespace Engine.Events;

public class EventSystem : IUpdateSystem
{
    private readonly EventBus _eventBus;

    public EventSystem(EventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void Update(GameTime gameTime)
    {
        _eventBus.Clear();
    }
}
