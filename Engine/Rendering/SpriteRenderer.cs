using System;
using System.Linq;

using Engine.Components;
using Engine.ECS;
using Engine.Utils;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering;

public class SpriteRenderer
{
    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly EntityManager _entityManager;
    private readonly SpriteBatch _spriteBatch;
    private readonly Helper _helprer;

    private readonly Effect _spriteEffect;

    public SpriteRenderer(ContentManager contentManager, GraphicsDevice graphicsDevice, EntityManager entityManager, SpriteBatch spriteBatch, Helper helper)
    {
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
        _entityManager = entityManager;
        _spriteBatch = spriteBatch;
        _helprer = helper;

        _spriteEffect = _contentManager.Load<Effect>("Effects/SpriteEffect");
    }

    public void RenderSprites(Func<Entity, bool>? entityFilter = null)
    {
        var cameraTransform = _helprer.GetCameraTransform();

        var entitiesToRender = _entityManager
            .GetEntitiesWithComponents(typeof(PositionComponent), typeof(RenderingComponent))
            .Where((entity) => entityFilter == null || entityFilter(entity))
            .SelectMany(
                (entity) =>
                {
                    var positionComponent = entity.GetComponent<PositionComponent>();
                    var renderingComponents = entity.GetComponents<RenderingComponent>();

                    return renderingComponents.Select((renderingComponent) =>
                    (
                        positionComponent,
                        renderingComponent
                    ));
                }
            );

        _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: cameraTransform, effect: _spriteEffect);

        foreach (var (positionComponent, renderingComponent) in entitiesToRender)
        {
            var worldPosition = positionComponent.Position + renderingComponent.Offset;
            var spriteEffect = (renderingComponent.FlipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None) | (renderingComponent.FlipY ? SpriteEffects.FlipVertically : SpriteEffects.None);

            _spriteBatch.Draw(
                renderingComponent.Texture,
                worldPosition,
                null,
                renderingComponent.Colour,
                positionComponent.Rotation,
                Vector2.Zero,
                renderingComponent.Scale,
                spriteEffect,
                Math.Clamp(renderingComponent.Layer / 100f, 0f, 1f)
            );
        }

        _spriteBatch.End();
    }

}