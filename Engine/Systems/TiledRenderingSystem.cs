using System.Linq;

using Engine.Components;
using Engine.ECS;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class TiledRenderingSystem : IRenderSystem
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly EntityManager _entityManager;

    public TiledRenderingSystem(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, EntityManager entityManager)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        _entityManager = entityManager;
    }

    public void Draw(float deltaTime)
    {
        var cameraEntity = _entityManager.GetEntitiesWithComponent<CameraComponent>().FirstOrDefault();
        var cameraComponent = cameraEntity?.GetComponent<CameraComponent>();

        _graphicsDevice.SetRenderTarget(null);
        _graphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: cameraComponent?.Transform);

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