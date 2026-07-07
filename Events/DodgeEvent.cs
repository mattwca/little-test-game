using Engine.ECS;
using Engine.Events;
using Microsoft.Xna.Framework;

namespace Events;

public class DodgeEvent(Entity entity, Vector2 direction, float timeSeconds, float cooldownSeconds) : IEvent
{
    public Entity Entity { get; set; } = entity;
    public Vector2 Direction { get; set; } = direction;
    public float TimeSeconds { get; set; } = timeSeconds;
    public float CooldownSeconds { get; set; } = cooldownSeconds;
}
