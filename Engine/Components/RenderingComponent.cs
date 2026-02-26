using Engine.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Components;

public class RenderingComponent : IComponent
{
    public Texture2D Texture { get; }
    public Vector2 Position { get; set; }
    public Color Colour { get; }
    public int Layer { get; }

    public RenderingComponent(Texture2D texture, Vector2 position, Color colour, int layer = 0)
    {
        Texture = texture;
        Position = position;
        Colour = colour;
        Layer = layer;
    }
}