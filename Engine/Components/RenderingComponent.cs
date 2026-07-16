using Engine.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Components;

public class RenderingComponent(
    Texture2D texture,
    Color colour = default,
    Vector2 offset = default,
    Vector2 origin = default,
    float scale = 1f,
    bool flipX = false,
    bool flipY = false,
    bool castsShadow = true,
    Rectangle? sourceRectangle = null,
    float? depthHeightOverride = null,
    float depthBias = 0f
) : IComponent
{
    public Texture2D Texture { get; } = texture;
    public Color Colour { get; set; } = colour;
    public Vector2 Offset { get; set; } = offset;
    public Vector2 Origin { get; set; } = origin;
    public float Scale { get; set; } = scale;
    public bool FlipX { get; set; } = flipX;
    public bool FlipY { get; set; } = flipY;
    public bool CastsShadow { get; set; } = castsShadow;
    public Rectangle? SourceRectangle { get; set; } = sourceRectangle;

    /// <summary>
    /// Depth value, overrides the depth position-based depth calculated by the renderer.
    /// </summary>
    public float? DepthHeightOverride { get; set; } = depthHeightOverride;

    /// <summary>
    /// Depth bias, allows an entities depth value to be adjusted. Useful when adjusting depth sorting
    /// between entities attached to one another.
    /// </summary>
    public float DepthBias { get; set; } = depthBias;
}
