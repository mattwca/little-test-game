using Engine.ECS;

using Microsoft.Xna.Framework;

namespace Engine.Components;

public class PositionComponent : IComponent
{
    public Vector2 Position { get; set; }
    public float Width {get; set;}
    public float Height { get;set;}

    public Vector2 Centre => Position + new Vector2(Width / 2f, Height / 2f);

    public PositionComponent(Vector2 position, float width = 0f, float height = 0f)
    {
        Position = position;
        Width = width;
        Height = height;
    }
}