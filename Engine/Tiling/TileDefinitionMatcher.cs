using System.Linq;

namespace Engine.Tiling;

public class TileDefinitionMatcher : ITileDefinitionMatcher
{
    private readonly TileMapDefinition _mapDefinition;
    private readonly int[][] _mapData;

    public TileDefinitionMatcher(TileMapDefinition mapDefinition, int[][] mapData)
    {
        _mapDefinition = mapDefinition;
        _mapData = mapData;
    }

    public TileDefinition FindMatchForTile(int tileX, int tileY)
    {
        var tileAbove = IsTileOccupied(tileX, tileY - 1);
        var tileBelow = IsTileOccupied(tileX, tileY + 1);
        var tileLeft = IsTileOccupied(tileX - 1, tileY);
        var tileRight = IsTileOccupied(tileX + 1, tileY);
        
        var mask =
            (tileAbove ? TileNeighbours.Above : 0) |
            (tileBelow ? TileNeighbours.Below : 0) |
            (tileLeft ? TileNeighbours.Left : 0) |
            (tileRight ? TileNeighbours.Right : 0);

        return _mapDefinition.TileDefinitions.First((definition) => definition.Neighbours == mask);
    }

    private bool IsTileOccupied(int tileX, int tileY)
    {
        if (tileX < 0 || tileY < 0 || tileX > _mapData.Length - 1 || tileY > _mapData[0].Length - 1)
        {
            return false;
        }

        return _mapData[tileX][tileY] >= 0;
    }
}