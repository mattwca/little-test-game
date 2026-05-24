using Engine.ECS;
using Engine.Tiling;

namespace Engine.Components;

public record MapComponent : IComponent
{
    public TileMapDefinition MapDefinition { get; set; }
    public int[,] MapData { get; set; }
    public int Layer { get; set; }
    public int[,] TileIndexMap { get; set; }

    public MapComponent(TileMapDefinition mapDefinition, int[,] mapData, int layer)
    {
        MapDefinition = mapDefinition;
        MapData = mapData;
        Layer = layer;
        TileIndexMap = new int[mapData.GetLength(0), mapData.GetLength(1)];
    }
}
