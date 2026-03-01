using Engine.ECS;
using Engine.Components;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Engine.Systems;

public class RenderingSystem : IRenderSystem
{
    private SpriteBatch _spriteBatch;

    public RenderingSystem(SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;
    }

    public void Draw(EntityManager entityManager, float deltaTime)
    {
        var cameraEntity = entityManager.GetEntitiesWithComponent<CameraComponent>().FirstOrDefault();
        var cameraComponent = cameraEntity?.GetComponent<CameraComponent>();

        var entitiesToRender = entityManager.GetEntitiesWithComponents(typeof(RenderingComponent), typeof(PositionComponent));

        _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: cameraComponent?.Transform);

        entitiesToRender.ForEach(entity =>
        {
            var positionComponent = entity.GetComponent<PositionComponent>();
            var renderingComponents = entity.GetComponents<RenderingComponent>();
            foreach (var component in renderingComponents)
            {
                var worldPosition = positionComponent.Position + component.Offset;

                _spriteBatch.Draw(
                    component.Texture,
                    worldPosition, 
                    null,
                    component.Colour,
                    0f,
                    scale: component.Scale,
                    layerDepth: component.Layer / 100f,
                    origin: Vector2.Zero,
                    effects: SpriteEffects.None
                );
            }
        });

        _spriteBatch.End();
    }
}