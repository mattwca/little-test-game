using Engine.Events;
using Microsoft.Xna.Framework;

namespace Engine.Physics;

public record CollisionEvent(
    string EntityA,
    string EntityB,
    Vector2 ContactPoint,
    Vector2 Normal,
    float PenetrationDepth
) : IEvent;
