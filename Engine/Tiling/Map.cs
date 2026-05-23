using Microsoft.Xna.Framework;

namespace Engine.Tiling;

public record Map(string MapId, Vector2 Position, int[][] MapData, TileMapDefinition TileMapDefinition, int Layer = 0)
{
    public int Width => MapData.Length;
    public int Height => MapData[0].Length;
}