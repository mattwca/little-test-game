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
    /// <summary>
    /// Max world height for layer depth calcs
    /// </summary>
    private const int MAX_WORLD_HEIGHT = 1000;

    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly EntityManager _entityManager;
    private readonly SpriteBatch _spriteBatch;
    private readonly Helper _helprer;

    private readonly Effect _spriteEffect;

    public SpriteRenderer(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice,
        EntityManager entityManager,
        SpriteBatch spriteBatch,
        Helper helper
    )
    {
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
        _entityManager = entityManager;
        _spriteBatch = spriteBatch;
        _helprer = helper;

        _spriteEffect = _contentManager.Load<Effect>("Effects/SpriteEffect");
    }

    public void RenderSprites(Func<RenderingComponent, bool>? filter = null)
    {
        var cameraTransform = _helprer.GetCameraTransform();

        var entitiesToRender = _entityManager
            .GetEntitiesWithComponents(
                typeof(PositionComponent),
                typeof(RenderingComponent),
                typeof(VisibilityComponent)
            )
            .Where((entity) => entity.GetComponent<VisibilityComponent>().IsVisible)
            .ToArray();

        _spriteBatch.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            samplerState: SamplerState.PointClamp,
            transformMatrix: cameraTransform,
            effect: _spriteEffect
        );

        foreach (var entity in entitiesToRender)
        {
            var heightComponent = entity.GetComponentOptional<HeightComponent>();
            var renderingComponents = entity.GetComponents<RenderingComponent>();
            var positionComponent = entity.GetComponent<PositionComponent>();

            foreach (var renderingComponent in renderingComponents)
            {
                if (filter is not null && !filter(renderingComponent))
                {
                    continue;
                }

                var worldPosition =
                    positionComponent.Position
                    + renderingComponent.Offset
                    - (heightComponent is null ? Vector2.Zero : new Vector2(0, heightComponent.Z));

                var spriteEffect =
                    (renderingComponent.FlipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None)
                    | (renderingComponent.FlipY ? SpriteEffects.FlipVertically : SpriteEffects.None);

                var layerDepth = positionComponent.Position.Y / MAX_WORLD_HEIGHT;

                _spriteBatch.Draw(
                    renderingComponent.Texture,
                    worldPosition,
                    renderingComponent.SourceRectangle,
                    renderingComponent.Colour,
                    positionComponent.Rotation,
                    renderingComponent.Origin,
                    renderingComponent.Scale,
                    spriteEffect,
                    layerDepth
                );
            }
        }

        _spriteBatch.End();
    }
}
