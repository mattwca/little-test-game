using System;
using System.Collections.Generic;
using Engine.ECS;
using Engine.Events;

namespace Engine.Components;

public interface IEventHandler<T>
    where T : Event
{
    public void OnUpdate();
}

/// <summary>
/// A component responsible for allowing event-driven development.
/// </summary>
public class EventHandlerComponent() : IComponent
{
    public Dictionary<Type, IEventHandler<Event>> _eventHandlers = [];

    public EventHandlerComponent AddEventHandler<T>(IEventHandler<T> eventHandler)
        where T : Event
    {
        var eventType = typeof(T);
        _eventHandlers[eventType] = eventHandler as IEventHandler<Event>;
        return this;
    }
}
