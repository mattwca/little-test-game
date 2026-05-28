using Engine.ECS;
using Engine.Tiling;

namespace Engine.Components;

public record MapComponent : IComponent
{
    public TileMapDefinition MapDefinition { get; set; }
    public int[,] MapData { get; set; }
    public int Layer { get; set; }

    public MapComponent(TileMapDefinition mapDefinition, int[,] mapData, int layer)
    {
        MapDefinition = mapDefinition;
        MapData = mapData;
        Layer = layer;
    }
}
