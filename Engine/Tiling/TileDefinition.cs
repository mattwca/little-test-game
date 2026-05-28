using System;
using Engine.Physics;

namespace Engine.Tiling;

[Flags]
public enum TileNeighbours
{
    None = 0,
    Above = 1 << 0,
    Below = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3,
}

public record TileDefinition(
    int X,
    int Y,
    TileNeighbours Neighbours,
    bool CastsShadow = true,
    RectangleF? boundingBox = null
);

public class TileMapDefinition(string tileId, int tileSize, TileDefinition[] tileDefinitions)
{
    public int TileSize { get; } = tileSize;
    public string TileId { get; } = tileId;
    public TileDefinition[] TileDefinitions { get; } = tileDefinitions;
}
