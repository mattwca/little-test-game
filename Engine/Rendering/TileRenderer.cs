using Engine.Components;
using Engine.ECS;
using Engine.Utils;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering;

public class TileRenderer(EntityManager entityManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Helper helper)
{
    private readonly EntityManager _entityManager = entityManager;
    private readonly GraphicsDevice _graphicsDevice = graphicsDevice;
    private readonly SpriteBatch _spriteBatch = spriteBatch;
    private readonly Helper _helper = helper;

    public void RenderTiles()
    {
        var cameraTransform = _helper.GetCameraTransform();
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: cameraTransform);

        var tiledRenderEntities = _entityManager.GetEntitiesWithComponents(typeof(PositionComponent), typeof(TiledRenderingComponent));

        foreach (var entity in tiledRenderEntities)
        {
            var tiledRenderComponent = entity.GetComponent<TiledRenderingComponent>();
            var positionComponent = entity.GetComponent<PositionComponent>();

            for (int x = 0; x < tiledRenderComponent.TilesX; x++)
            {
                for (int y = 0; y < tiledRenderComponent.TilesY; y++)
                {
                    var worldPosition = positionComponent.Position + tiledRenderComponent.Offset;
                    var tilePosition = worldPosition + new Vector2(x * tiledRenderComponent.TileSize, y * tiledRenderComponent.TileSize);
                    _spriteBatch.Draw(tiledRenderComponent.Texture, tilePosition, tiledRenderComponent.Colour);
                }
            }
        }

        _spriteBatch.End();
    }
}