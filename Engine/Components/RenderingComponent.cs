using Engine.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Components;

public class RenderingComponent : IComponent
{
    public Texture2D Texture { get; }
    public Color Colour { get; }
    public Vector2 Offset { get; set; }
    public int Layer { get; }
    public float Scale {get; set; }

    public RenderingComponent(Texture2D texture, Color colour = default, Vector2 offset = default, int layer = 0, float scale = 1f)
    {
        Texture = texture;
        Colour = colour;
        Offset = offset;
        Layer = layer;
        Scale = scale;
    }
}