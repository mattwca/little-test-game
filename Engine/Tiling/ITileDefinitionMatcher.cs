namespace Engine.Tiling;

public interface ITileDefinitionMatcher
{
    TileDefinition? FindMatchForTile(int tileX, int tileY);
}
