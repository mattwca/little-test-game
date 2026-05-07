using Engine.Components;
using Engine.ECS;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Systems;

public class CameraSystem : IUpdateSystem
{
    private readonly EntityManager _entityManager;
    private readonly GraphicsDevice _graphicsDevice;

    public CameraSystem(EntityManager entityManager, GraphicsDevice graphicsDevice)
    {
        _entityManager = entityManager;
        _graphicsDevice = graphicsDevice;
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
        var screenWidth = _graphicsDevice.Viewport.Width / 2f;
        var screenHeight = _graphicsDevice.Viewport.Height / 2f;

        var newPosition = entityPosition.Position - new Vector2(screenWidth / 2, screenHeight / 2);
        newPosition.Round();

        cameraComponent.Position = newPosition;
    }
}