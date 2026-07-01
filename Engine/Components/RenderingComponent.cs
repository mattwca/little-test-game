using Engine.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Components;

public class RenderingComponent : IComponent
{
    public Texture2D Texture { get; }
    public Color Colour { get; set; }
    public Vector2 Offset { get; set; }
    public Vector2 Origin { get; set; }
    public float Scale { get; set; }
    public bool FlipX { get; set; }
    public bool FlipY { get; set; }
    public bool CastsShadow { get; set; }
    public Rectangle? SourceRectangle { get; set; }
    public float? DepthHeightOverride { get; set; }

    public RenderingComponent(
        Texture2D texture,
        Color colour = default,
        Vector2 offset = default,
        Vector2 origin = default,
        float scale = 1f,
        bool flipX = false,
        bool flipY = false,
        bool castsShadow = true,
        Rectangle? sourceRectangle = null,
        float? depthHeightOverride = null
    )
    {
        Texture = texture;
        Colour = colour;
        Offset = offset;
        Origin = origin;
        Scale = scale;
        FlipX = flipX;
        FlipY = flipY;
        CastsShadow = castsShadow;
        SourceRectangle = sourceRectangle;
        DepthHeightOverride = depthHeightOverride;
    }
}
