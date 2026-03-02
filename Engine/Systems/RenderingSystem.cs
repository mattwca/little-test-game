using System;
using System.ComponentModel;
using System.Linq;

using Engine.Components;
using Engine.ECS;
using Engine.Lighting;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Systems;

public class RenderingSystem : IRenderSystem
{
    private readonly SpriteBatch _spriteBatch;
    private readonly EntityManager _entityManager;
    private readonly LightSystem _lightSystem;
    private readonly GraphicsDevice _graphicsDevice;

    public RenderingSystem(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, EntityManager entityManager, LightSystem lightSystem)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        _entityManager = entityManager;
        _lightSystem = lightSystem;
    }

    public void Draw(float deltaTime)
    {
        var cameraEntity = _entityManager.GetEntitiesWithComponent<CameraComponent>().FirstOrDefault();
        var cameraComponent = cameraEntity?.GetComponent<CameraComponent>();

        var entitiesToRender = _entityManager.GetEntitiesWithComponents(typeof(RenderingComponent), typeof(PositionComponent));

        _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: cameraComponent?.Transform);

        _spriteBatch.Draw(_lightSystem._shadowMapRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

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
                    effects: (component.FlipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None) | (component.FlipY ? SpriteEffects.FlipVertically : SpriteEffects.None)
                );
            }
        });

        _spriteBatch.End();
    }
}