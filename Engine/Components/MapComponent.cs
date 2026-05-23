using Engine.ECS;
using Engine.Tiling;

namespace Engine.Components;

public record MapComponent(TileMapDefinition MapDefinition, int[][] MapData, int Layer) : IComponent;