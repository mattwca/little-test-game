using Microsoft.Xna.Framework;

namespace Engine.Rendering;

public class RenderItem
{
    public Vector2 Position { get; set; }
    public Rectangle SourceRectangle { get; set; }
    public Rectangle DestinationRectangle { get; set; }
}