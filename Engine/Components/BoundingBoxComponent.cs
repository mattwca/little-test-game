using Engine.ECS;

using Microsoft.Xna.Framework;

namespace Engine.Components;

public class BoundingBoxComponent : IComponent
{
    public Vector2 Offset { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool IsStatic { get; set; }

    public BoundingBoxComponent(Vector2 offset, int width, int height, bool isStatic = true)
    {
        Offset = offset;
        Width = width;
        Height = height;
        IsStatic = isStatic;
    }
}
