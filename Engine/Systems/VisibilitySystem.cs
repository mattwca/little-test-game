using System;
using System.Collections.Generic;

using Engine.Components;
using Engine.ECS;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Systems;

public class VisibilitySystem(EntityManager entityManager, GraphicsDevice graphicsDevice) : IUpdateSystem
{
    private readonly EntityManager _entityManager = entityManager;
    private readonly GraphicsDevice _graphicsDevice = graphicsDevice;

    public void Update(GameTime gameTime)
    {
        var cameraEntity = _entityManager.GetEntityWithComponent<CameraComponent>()!;
        var cameraComponent = cameraEntity.GetComponent<CameraComponent>();

        var entitiesToCheck = _entityManager.GetEntitiesWithComponents(typeof(PositionComponent), typeof(VisibilityComponent));

        var topLeft = Vector2.Zero;
        var bottomRight = new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);

        var inverseCameraTransform = Matrix.Invert(cameraComponent.Transform);

        var transformedTopLeft = Vector2.Transform(topLeft, inverseCameraTransform);
        var transformedBottomRight = Vector2.Transform(bottomRight, inverseCameraTransform);

        var cameraWorldBoundingBox = new Rectangle(
            (int)transformedTopLeft.X,
            (int)transformedTopLeft.Y,
            (int)transformedBottomRight.X - (int)transformedTopLeft.X,
            (int)transformedBottomRight.Y - (int)transformedTopLeft.Y
        );

        foreach (var entity in entitiesToCheck)
        {
            var entityPosition = entity.GetComponent<PositionComponent>();
            var entityVisibility = entity.GetComponent<VisibilityComponent>();
            
            entityVisibility.IsVisible = cameraWorldBoundingBox.Contains(entityPosition.Position);
        }
    }
}