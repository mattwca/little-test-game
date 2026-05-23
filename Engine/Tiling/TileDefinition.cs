using System;

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

public record TileDefinition(int X, int Y, TileNeighbours Neighbours, bool CastsShadow = true);

public class TileMapDefinition(string tileType, string tileTexturePath, int tileSize, TileDefinition[] tileDefinitions)
{
    public int TileSize { get; } = tileSize;
    public string TileType { get; } = tileType;
    public string TileTexturePath { get; } = tileTexturePath;
    public TileDefinition[] TileDefinitions { get; } = tileDefinitions;
}