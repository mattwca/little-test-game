using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Events;

/// <summary>
/// An event bus which allows one system to react to events emitted by another. To allow out-of-orders systems to
/// react to events correctly, events are double buffered (1-frame lag).
/// </summary>
public class EventBus
{
    private Dictionary<Type, List<IEvent>> _previousEvents;
    private Dictionary<Type, List<IEvent>> _currentEvents;

    public EventBus()
    {
        _previousEvents = [];
        _currentEvents = [];
    }

    public void Publish<T>(T gameEvent)
        where T : IEvent
    {
        var eventType = typeof(T);

        if (!_currentEvents.ContainsKey(eventType))
        {
            _currentEvents.Add(eventType, [gameEvent]);
            return;
        }

        _currentEvents[eventType].Add(gameEvent);
    }

    public List<T> ReadAll<T>()
        where T : IEvent
    {
        var eventType = typeof(T);

        if (!_previousEvents.ContainsKey(eventType))
        {
            return [];
        }

        return [.. _previousEvents[eventType].Cast<T>()];
    }

    public void Clear()
    {
        _previousEvents = _currentEvents;
        _currentEvents = [];
    }
}
