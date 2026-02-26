using System.Linq;
using Engine.Components;
using Engine.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class TiledRenderingSystem : IRenderSystem
{
    private SpriteBatch _spriteBatch;

    public TiledRenderingSystem(SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;
    }

    public void Draw(EntityManager entityManager, float deltaTime)
    {
        var cameraEntity = entityManager.GetEntitiesWithComponent<CameraComponent>().FirstOrDefault();
        var cameraComponent = cameraEntity?.GetComponent<CameraComponent>();

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: cameraComponent?.Transform);

        var tiledRenderEntities = entityManager.GetEntitiesWithComponent<TiledRenderingComponent>();

        foreach (var entity in tiledRenderEntities)
        {
            var tiledRenderComponent = entity.GetComponent<TiledRenderingComponent>();

            for (int x = 0; x < tiledRenderComponent.TilesX; x++)
            {
                for (int y = 0; y < tiledRenderComponent.TilesY; y++)
                {
                    var tilePosition = tiledRenderComponent.Position + new Vector2(x * tiledRenderComponent.TileSize, y * tiledRenderComponent.TileSize);
                    _spriteBatch.Draw(tiledRenderComponent.Texture, tilePosition, tiledRenderComponent.Colour);
                }
            }
        }

        _spriteBatch.End();
    }
}