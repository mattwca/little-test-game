using Engine.Tiling;

public static class WallTileDefinitions
{
    public static TileMapDefinition Definition = new TileMapDefinition(
        "wall",
        "wall",
        32,
        [
            new TileDefinition(0, 0, TileNeighbours.Left | TileNeighbours.Above | TileNeighbours.Right),
            new TileDefinition(1, 0, TileNeighbours.Left | TileNeighbours.Below | TileNeighbours.Right),
            new TileDefinition(0, 1, TileNeighbours.Above | TileNeighbours.Below | TileNeighbours.Left),
            new TileDefinition(1, 1, TileNeighbours.Above | TileNeighbours.Below | TileNeighbours.Right),

            new TileDefinition(2, 0, TileNeighbours.Right | TileNeighbours.Below),
            new TileDefinition(3, 0, TileNeighbours.Left | TileNeighbours.Below),
            new TileDefinition(2, 1, TileNeighbours.Right | TileNeighbours.Above),
            new TileDefinition(3, 1, TileNeighbours.Left | TileNeighbours.Above),

            new TileDefinition(4, 0, TileNeighbours.Left | TileNeighbours.Right | TileNeighbours.Above | TileNeighbours.Below),
        ]
    );
}