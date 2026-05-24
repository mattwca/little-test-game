using System.Collections.Generic;
using Engine.Components;
using Engine.ECS;
using Engine.Tiling;
using Engine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering;

public class AutoTileRenderer
{
    private readonly EntityManager _entityManager;
    private readonly SpriteBatch _spriteBatch;
    private readonly ContentManager _contentManager;
    private readonly StateManager _stateManager;
    private readonly Helper _helper;

    private readonly SpriteFont _debugFont;
    private readonly Dictionary<string, Texture2D> _mapTextures = [];

    public AutoTileRenderer(
        EntityManager entityManager,
        SpriteBatch spriteBatch,
        ContentManager contentManager,
        StateManager stateManager,
        Helper helper
    )
    {
        _entityManager = entityManager;
        _spriteBatch = spriteBatch;
        _contentManager = contentManager;
        _stateManager = stateManager;
        _helper = helper;

        _debugFont = contentManager.Load<SpriteFont>("debugFont");
    }

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

            for (var i = 0; i < mapComponent.MapData.GetLength(0); i++)
            {
                for (var j = 0; j < mapComponent.MapData.GetLength(1); j++)
                {
                    var tileDefinition = tileMatcher.FindMatchForTile(i, j);

                    var offset = new Vector2(
                        i * mapComponent.MapDefinition.TileSize,
                        j * mapComponent.MapDefinition.TileSize
                    );
                    var worldPosition = positionComponent.Position + offset;

                    var texture = _mapTextures.TryGetOrAdd(
                        mapComponent.MapDefinition.TileType,
                        () => _contentManager.Load<Texture2D>(mapComponent.MapDefinition.TileTexturePath)
                    );

                    var sourceRectangle = new Rectangle(
                        tileDefinition.X * mapComponent.MapDefinition.TileSize,
                        tileDefinition.Y * mapComponent.MapDefinition.TileSize,
                        mapComponent.MapDefinition.TileSize,
                        mapComponent.MapDefinition.TileSize
                    );

                    _spriteBatch.Draw(texture, worldPosition, sourceRectangle, Color.White);

                    if (_stateManager.GetBool("debugModeEnabled"))
                    {
                        _spriteBatch.DrawString(
                            _debugFont,
                            ((int)tileDefinition.Neighbours).ToString(),
                            new Vector2(
                                worldPosition.X + mapComponent.MapDefinition.TileSize / 4,
                                worldPosition.Y + mapComponent.MapDefinition.TileSize / 4
                            ),
                            Color.White
                        );
                    }
                }
            }
        }

        _spriteBatch.End();
    }
}
