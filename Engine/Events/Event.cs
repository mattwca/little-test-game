using System;

namespace Engine.Events;

public abstract class Event
{
    public readonly string EventId = Guid.NewGuid().ToString();
}
