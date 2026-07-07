using Engine.ECS;
using Microsoft.Xna.Framework;

public record VelocityComponent : IComponent
{
    public Vector2 Velocity { get; set; }

    public VelocityComponent()
    {
        Velocity = Vector2.Zero;
    }

    public VelocityComponent(Vector2 velocity)
    {
        Velocity = velocity;
    }
}
