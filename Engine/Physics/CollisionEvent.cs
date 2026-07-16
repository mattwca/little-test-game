using Engine.Events;
using Microsoft.Xna.Framework;

namespace Engine.Physics;

public class CollisionEvent(
    string entityA,
    string entityB,
    Vector2 contactPoint,
    Vector2 normal,
    float penetrationDepth
) : Event
{
    public readonly string EntityA = entityA;
    public readonly string EntityB = entityB;
    public readonly Vector2 ContactPoint = contactPoint;
    public readonly Vector2 Normal = normal;
    public readonly float PenetrationDepth = penetrationDepth;
}
