using Engine.ECS;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Components;

public class RenderingComponent : IComponent
{
    public Texture2D Texture { get; }
    public Color Colour { get; set; }
    public Vector2 Offset { get; set; }
    public int Layer { get; }
    public float Scale { get; set; }
    public bool FlipX { get; set; }
    public bool FlipY { get; set; }
    public bool CastsShadow { get; set; }

    public RenderingComponent(Texture2D texture, Color colour = default, Vector2 offset = default, int layer = 0, float scale = 1f, bool flipX = false, bool flipY = false, bool castsShadow = true)
    {
        Texture = texture;
        Colour = colour;
        Offset = offset;
        Layer = layer;
        Scale = scale;
        FlipX = flipX;
        FlipY = flipY;
        CastsShadow = castsShadow;
    }
}