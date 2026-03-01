using Engine.Components;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class TiledRenderingComponent : RenderingComponent
{
    public int TileSize { get; }
    public int TilesX { get; }
    public int TilesY { get; }

    public TiledRenderingComponent(Texture2D texture, int tileSize, int tilesX, int tilesY, Color colour = default, Vector2 offset = default, int layer = 0, float scale = 1f)
      : base(texture, colour, offset, layer, scale)
    {
        TileSize = tileSize;
        TilesX = tilesX;
        TilesY = tilesY;
    }
}