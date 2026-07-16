using System;
using Engine.Events;
using Microsoft.Xna.Framework;

namespace Events;

public class DodgeEvent(
    string entityId,
    Vector2 direction,
    float timeSeconds,
    float cooldownSeconds,
    Action? onFinished = null
) : Event
{
    public string EntityId { get; set; } = entityId;
    public Vector2 Direction { get; set; } = direction;
    public float TimeSeconds { get; set; } = timeSeconds;
    public float CooldownSeconds { get; set; } = cooldownSeconds;
    public Action? OnFinished { get; set; } = onFinished;
}
