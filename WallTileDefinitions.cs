using Engine.Physics;
using Engine.Tiling;

public static class WallTileDefinitions
{
    public static TileMapDefinition Definition = new TileMapDefinition(
        "wall",
        "wall",
        32,
        [
            new TileDefinition(
                0,
                0,
                TileNeighbours.Left | TileNeighbours.Above | TileNeighbours.Right,
                boundingBox: new RectangleF(0, 0, 32, 32)
            ),
            new TileDefinition(
                1,
                0,
                TileNeighbours.Left | TileNeighbours.Below | TileNeighbours.Right,
                boundingBox: new RectangleF(0, 0, 32, 32)
            ),
            new TileDefinition(
                0,
                1,
                TileNeighbours.Above | TileNeighbours.Below | TileNeighbours.Left,
                boundingBox: new RectangleF(0, 0, 22, 32)
            ),
            new TileDefinition(
                1,
                1,
                TileNeighbours.Above | TileNeighbours.Below | TileNeighbours.Right,
                boundingBox: new RectangleF(9, 0, 22, 32)
            ),
            new TileDefinition(
                2,
                0,
                TileNeighbours.Right | TileNeighbours.Below,
                boundingBox: new RectangleF(9, 0, 22, 32)
            ),
            new TileDefinition(
                3,
                0,
                TileNeighbours.Left | TileNeighbours.Below,
                boundingBox: new RectangleF(0, 0, 22, 32)
            ),
            new TileDefinition(
                2,
                1,
                TileNeighbours.Right | TileNeighbours.Above,
                boundingBox: new RectangleF(9, 0, 22, 32)
            ),
            new TileDefinition(
                3,
                1,
                TileNeighbours.Left | TileNeighbours.Above,
                boundingBox: new RectangleF(0, 0, 22, 32)
            ),
            new TileDefinition(
                4,
                0,
                TileNeighbours.Left | TileNeighbours.Right | TileNeighbours.Above | TileNeighbours.Below,
                boundingBox: new RectangleF(0, 0, 32, 32)
            ),
        ]
    );
}
