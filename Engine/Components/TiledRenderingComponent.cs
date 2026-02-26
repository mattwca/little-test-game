using Engine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class TiledRenderingComponent : RenderingComponent
{
    public int TileSize { get; }
    public int TilesX { get; }
    public int TilesY { get; }

    public TiledRenderingComponent(Texture2D texture, Vector2 position, Color colour, int tileSize, int tilesX, int tilesY, int layer = 0)
      : base(texture, position, colour, layer)
    {
        TileSize = tileSize;
        TilesX = tilesX;
        TilesY = tilesY;
    }
}