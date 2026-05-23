using System;
using System.Collections.Generic;

using Engine.Components;
using Engine.ECS;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Systems;

public class VisibilitySystem(EntityManager entityManager, GraphicsDevice graphicsDevice) : IUpdateSystem
{
    private readonly Vector2 _visibilityBoundaryOffset = new Vector2(400, 400);

    private readonly EntityManager _entityManager = entityManager;
    private readonly GraphicsDevice _graphicsDevice = graphicsDevice;

    public void Update(GameTime gameTime)
    {
        var cameraEntity = _entityManager.GetEntityWithComponent<CameraComponent>()!;
        var cameraComponent = cameraEntity.GetComponent<CameraComponent>();

        var entitiesToCheck = _entityManager.GetEntitiesWithComponents(
            typeof(PositionComponent),
            typeof(VisibilityComponent)
        );

        var topLeft = Vector2.Zero;
        var bottomRight = new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);

        var inverseCameraTransform = Matrix.Invert(cameraComponent.Transform);

        var transformedTopLeft = Vector2.Transform(topLeft, inverseCameraTransform) - _visibilityBoundaryOffset;
        var transformedBottomRight = Vector2.Transform(bottomRight, inverseCameraTransform) + _visibilityBoundaryOffset;
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

            var entityOffsetVector = new Vector2(entityVisibility.Offset, entityVisibility.Offset);
            var entityBoundingTopLeft = entityPosition.Position - entityOffsetVector;
            var entityBoundingDimensions = entityOffsetVector * 2;

            var entityBoundingBox = new Rectangle(entityBoundingTopLeft.ToPoint(), entityBoundingDimensions.ToPoint());

            entityVisibility.IsVisible = cameraWorldBoundingBox.Intersects(entityBoundingBox);
            if (entityVisibility.IsVisible || entityVisibility.Offset == 0f)
            {
                continue;
            }

            var offset = entityVisibility.Offset;
            var entityOffsetBoundingBox = new Rectangle(
                new Point((int)entityPosition.Position.X, (int)entityPosition.Position.Y),
                new Point((int)offset, (int)offset)
            );

            entityVisibility.IsVisible = cameraWorldBoundingBox.Intersects(entityOffsetBoundingBox);
        }
    }
}