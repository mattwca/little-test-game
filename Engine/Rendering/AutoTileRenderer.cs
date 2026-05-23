using System.Collections.Generic;

using Engine.Components;
using Engine.ECS;
using Engine.Tiling;
using Engine.Utils;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering;

public class AutoTileRenderer(
    EntityManager entityManager,
    GraphicsDevice graphicsDevice,
    SpriteBatch spriteBatch,
    ContentManager contentManager,
    StateManager stateManager,
    Helper helper
)
{
    private readonly EntityManager _entityManager = entityManager;
    private readonly SpriteBatch _spriteBatch = spriteBatch;
    private readonly ContentManager _contentManager = contentManager;
    private readonly StateManager _stateManager = stateManager;
    private readonly Helper _helper = helper;
    private readonly Dictionary<string, Texture2D> _mapTextures = [];

    public void RenderMaps()
    {
        var cameraTransform = _helper.GetCameraTransform();
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: cameraTransform);

        var debugModeEnabled = _stateManager.GetBool("debugModeEnabled");

        var mapEntities = _entityManager.GetEntitiesWithComponents(typeof(PositionComponent), typeof(MapComponent));
        foreach (var entity in mapEntities)
        {
            var positionComponent = entity.GetComponent<PositionComponent>();
            var mapComponent = entity.GetComponent<MapComponent>();

            var tileMatcher = new TileDefinitionMatcher(mapComponent.MapDefinition, mapComponent.MapData);
            var (mapDefinition, mapData, layer) = mapComponent;

            for (var i = 0; i < mapData.Length; i++)
            {
                for (var j = 0; j < mapData[i].Length; j++)
                {
                    var tileDefinition = tileMatcher.FindMatchForTile(i, j);

                    var offset = new Vector2(i * mapDefinition.TileSize, j * mapDefinition.TileSize);
                    var worldPosition = positionComponent.Position + offset;

                    var texture = _mapTextures.TryGetOrAdd(
                        mapDefinition.TileType,
                        () => _contentManager.Load<Texture2D>(mapDefinition.TileTexturePath)
                    );

                    var sourceRectangle = new Rectangle(
                        tileDefinition.X,
                        tileDefinition.Y,
                        mapDefinition.TileSize,
                        mapDefinition.TileSize
                    );

                    _spriteBatch.Draw(texture, worldPosition, sourceRectangle, Color.White);
                }
            }
        }

        _spriteBatch.End();
    }
}