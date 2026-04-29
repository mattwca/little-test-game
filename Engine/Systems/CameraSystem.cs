using Engine.Components;
using Engine.ECS;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Systems;

public class CameraSystem : IUpdateSystem
{
    private EntityManager _entityManager;
    private GraphicsDevice _graphicsDevice;

    public CameraSystem(EntityManager entityManager, GraphicsDevice graphicsDevice)
    {
        this._entityManager = entityManager;
        this._graphicsDevice = graphicsDevice;
    }

    public void Update(GameTime gameTime)
    {
        var cameraEntity = _entityManager.GetEntityWithComponent<CameraComponent>()!;
        var cameraComponent = cameraEntity.GetComponent<CameraComponent>();
        if (cameraComponent.Following == null)
        {
            return;
        }

        var followingEntity = _entityManager.GetEntity(cameraComponent.Following);
        if (followingEntity == null || !followingEntity.HasComponent<PositionComponent>())
        {
            return;
        }

        var entityPosition = followingEntity.GetComponent<PositionComponent>();
        var screenWidth = this._graphicsDevice.Viewport.Width;
        var screenHeight = this._graphicsDevice.Viewport.Height;
        
        cameraComponent.Position = entityPosition.Position - new Vector2(200, 150); // - new Vector2(screenWidth / 2f, screenHeight / 2f);
    }
}