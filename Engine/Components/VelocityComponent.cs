using Engine.ECS;
using Microsoft.Xna.Framework;

public class VelocityComponent : IComponent
{
    public Vector2 Velocity { get; set; }

    public VelocityComponent(Vector2 velocity)
    {
        Velocity = velocity;
    }
}
