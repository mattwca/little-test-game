using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Engine.Components;
using Engine.ECS;
using Engine.Tiling;
using Microsoft.Xna.Framework;

namespace Engine.Systems;

public class MapSystem : IUpdateSystem
{
    private readonly EntityManager _entityManager;
    private readonly Dictionary<string, int> _tileHashes;

    public MapSystem(EntityManager entityManager)
    {
        _entityManager = entityManager;
        _tileHashes = [];
    }

    public void Update(GameTime gameTime)
    {
        var mapEntities = _entityManager.GetEntitiesWithComponent<MapComponent>();

        foreach (var entity in mapEntities)
        {
            var mapComponent = entity.GetComponent<MapComponent>();
            if (!GetHashChanged(entity.Id, mapComponent.MapData))
            {
                return;
            }

            // Rebuild the tile index map and bounding boxes if tile layout has changed.
            BuildTileIndexMap(mapComponent);
            BuildBoundingBoxes(entity, mapComponent);
        }
    }

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

    /// <summary>
    /// Rebuilds the "tile definition index" map used to determine which tile definition is
    /// used for each square in the map, and therefore the final tile sprite that is rendered.
    /// </summary>
    private void BuildTileIndexMap(MapComponent mapComponent)
    {
        var newIndexMap = new int[mapComponent.MapData.GetLength(0), mapComponent.MapData.GetLength(1)];
        var tileMatcher = new TileDefinitionMatcher(mapComponent.MapDefinition, mapComponent.MapData);

        for (var i = 0; i < mapComponent.MapData.GetLength(0); i++)
        {
            for (var j = 0; j < mapComponent.MapData.GetLength(1); j++)
            {
                // Get the tile definition to use for the current tile
                var tileDefinition = tileMatcher.FindMatchForTile(i, j);
                var tileDefinitionIndex = mapComponent.MapDefinition.TileDefinitions.ToList().IndexOf(tileDefinition);

                if (tileDefinitionIndex == -1)
                {
                    continue;
                }

                // Store the index of the definition into our map.
                newIndexMap[i, j] = tileDefinitionIndex;
            }
        }

        mapComponent.TileIndexMap = newIndexMap;
    }

    private void BuildBoundingBoxes(Entity mapEntity, MapComponent mapComponent)
    {
        mapEntity.RemoveComponents<BoundingBoxComponent>();

        List<BoundingBoxComponent> boundingBoxComponents = [];

        for (var i = 0; i < mapComponent.MapData.GetLength(0); i++)
        {
            for (var j = 0; j < mapComponent.MapData.GetLength(1); j++)
            {
                var tileDefinitionIndex = mapComponent.TileIndexMap[i, j];
                var tileDefinition = mapComponent.MapDefinition.TileDefinitions[tileDefinitionIndex];

                if (tileDefinition.boundingBox == null)
                {
                    continue;
                }

                var tilePosition = new Vector2(
                    i * mapComponent.MapDefinition.TileSize,
                    j * mapComponent.MapDefinition.TileSize
                );

                var boundingBoxPosition =
                    tilePosition + new Vector2(tileDefinition.boundingBox.Value.X, tileDefinition.boundingBox.Value.Y);

                var newBonudingBoxComponent = new BoundingBoxComponent(
                    boundingBoxPosition,
                    tileDefinition.boundingBox.Value.Width,
                    tileDefinition.boundingBox.Value.Height
                );

                Console.WriteLine(
                    $"{newBonudingBoxComponent.Offset}, {newBonudingBoxComponent.Width} x {newBonudingBoxComponent.Height}"
                );

                boundingBoxComponents.Add(newBonudingBoxComponent);
            }
        }

        mapEntity.AddComponents(boundingBoxComponents);
    }
}
