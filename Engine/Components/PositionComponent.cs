using Engine.ECS;
using Microsoft.Xna.Framework;

namespace Engine.Components;

public class PositionComponent : IComponent
{
    public Vector2 Position { get; set; }

    public PositionComponent(Vector2 position)
    {
        Position = position;
    }
}