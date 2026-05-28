using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Engine.Components;
using Engine.ECS;
using Engine.Tiling;
using Engine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Systems;

public class MapSystem : IUpdateSystem
{
    private readonly EntityManager _entityManager;
    private readonly ContentManager _contentManager;

    private readonly Dictionary<string, int> _tileHashes;
    private readonly Dictionary<string, Dictionary<(int x, int y), string>> _mapTileEntities;

    private const string TILE_ENTITY_ID = "map:{0}:{1}x{2}";

    public MapSystem(EntityManager entityManager, ContentManager contentManager)
    {
        _entityManager = entityManager;
        _contentManager = contentManager;

        _tileHashes = [];
        _mapTileEntities = [];
    }

    public void Update(GameTime gameTime)
    {
        var mapEntities = _entityManager.GetEntitiesWithComponent<MapComponent>();

        foreach (var mapEntity in mapEntities)
        {
            if (_mapTileEntities.ContainsKey(mapEntity.Id))
            {
                UpdateTileEntitiesForMap(mapEntity);
                return;
            }

            _mapTileEntities.Add(mapEntity.Id, []);
            BuildTileEntitiesForMap(mapEntity);
        }
    }

    // private void CheckChangesAndUpdate(Entity mapEntity)
    // {
    //     var mapComponent = mapEntity.GetComponent<MapComponent>();

    //     if (!GetHashChanged(mapEntity.Id, mapComponent.MapData))
    //     {
    //         return;
    //     }
    // }

    private bool GetHashChanged(string entityId, int[,] mapData)
    {
        var currentHash = mapData.GetHashCode();

        if (_tileHashes.ContainsKey(entityId) && _tileHashes[entityId] == currentHash)
        {
            return false;
        }

        _tileHashes[entityId] = currentHash;
        return true;
    }

    private void BuildTileEntitiesForMap(Entity mapEntity)
    {
        var mapComponent = mapEntity.GetComponent<MapComponent>();
        var mapPositionComponent = mapEntity.GetComponent<PositionComponent>();

        var width = mapComponent.MapData.GetLength(1);
        var height = mapComponent.MapData.GetLength(0);
        var tileSize = mapComponent.MapDefinition.TileSize;

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var tilePosition = mapPositionComponent.Position + new Vector2(x * tileSize, y * tileSize);

                _entityManager
                    .CreateEntity(string.Format(TILE_ENTITY_ID, mapEntity.Id, x, y))
                    .AddComponent(new PositionComponent(tilePosition, tileSize, tileSize))
                    .AddComponent(new VisibilityComponent());
            }
        }

        UpdateTileEntitiesForMap(mapEntity);
    }

    private void UpdateTileEntitiesForMap(Entity mapEntity)
    {
        var mapComponent = mapEntity.GetComponent<MapComponent>();

        int width = mapComponent.MapData.GetLength(1),
            height = mapComponent.MapData.GetLength(0),
            tileSize = mapComponent.MapDefinition.TileSize;

        var mapTexture = _contentManager.Load<Texture2D>(mapComponent.MapDefinition.TileId);
        var tileMatcher = new TileDefinitionMatcher(mapComponent.MapDefinition, mapComponent.MapData);

        var definitions = new string[height, width];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var tileEntityId = string.Format(TILE_ENTITY_ID, mapEntity.Id, x, y);
                var tileEntity = _entityManager.GetEntity(tileEntityId)!;
                var tileDefinition = tileMatcher.FindMatchForTile(x, y);

                definitions[y, x] = tileDefinition?.Neighbours.ToString() ?? "";

                if (tileDefinition == null)
                {
                    tileEntity.RemoveComponents<RenderingComponent>();
                    continue;
                }

                var updatedRenderingComponent = new RenderingComponent(
                    mapTexture,
                    castsShadow: tileDefinition.CastsShadow,
                    sourceRectangle: new Rectangle(
                        tileDefinition.X * tileSize,
                        tileDefinition.Y * tileSize,
                        tileSize,
                        tileSize
                    ),
                    colour: Color.White
                );

                tileEntity.ReplaceComponent(updatedRenderingComponent);

                if (!tileDefinition.boundingBox.HasValue)
                {
                    continue;
                }

                var bb = tileDefinition.boundingBox.Value;
                var updatedBoundingBoxComponent = new BoundingBoxComponent(
                    new Vector2(bb.X, bb.Y),
                    bb.Width,
                    bb.Height
                );

                tileEntity.ReplaceComponent(updatedBoundingBoxComponent);
            }
        }
    }
}
