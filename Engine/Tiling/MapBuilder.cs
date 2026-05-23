using Engine.ECS;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Tiling;

public class MapBuilder
{
    private readonly ContentManager _contentManager;

    public MapBuilder(ContentManager contentManager)
    {
        _contentManager = contentManager;
    }

    public Entity BuildTileComponents(Map map)
    {
        var entity = new Entity(map.MapId);
        var texture = _contentManager.Load<Texture2D>(map.TileMapDefinition.TileTexturePath);

        for (var i = 0; i < map.Width; i++)
        {
            for (var j = 0; j < map.Height; j++)
            {
                var tileIndex = map.MapData[i][j];
                var tile = map.TileMapDefinition.TileDefinitions[tileIndex];

                var tileComponent = new TiledRenderingComponent(
                    texture,
                    map.TileMapDefinition.TileSize,
                    tile.X,
                    tile.Y,
                    castsShadow: tile.CastsShadow,
                    layer: map.Layer
                );

                entity.AddComponent(tileComponent);
            }
        }

        return entity;
    }
}