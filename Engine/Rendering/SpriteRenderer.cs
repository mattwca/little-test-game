using System.Linq;
using Engine.Components;
using Engine.Configuration;
using Engine.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering;

public class SpriteRenderer
{
    private readonly EntityManager _entityManager;
    private readonly SpriteBatch _spriteBatch;
    private readonly GameConfiguration _config;

    public SpriteRenderer(EntityManager entityManager, SpriteBatch spriteBatch, GameConfiguration config)
    {
        _entityManager = entityManager;
        _spriteBatch = spriteBatch;
        _config = config;
    }

    public void RenderSprites(bool shadowCastersOnly)
    {
        var entitiesToRender = _entityManager
            .GetEntitiesWithComponents(
                typeof(PositionComponent),
                typeof(RenderingComponent),
                typeof(VisibilityComponent)
            )
            .Where((entity) => entity.GetComponent<VisibilityComponent>().IsVisible)
            .ToArray();

        foreach (var entity in entitiesToRender)
        {
            var heightComponent = entity.GetComponentOptional<HeightComponent>();
            var renderingComponents = entity.GetComponents<RenderingComponent>();
            var positionComponent = entity.GetComponent<PositionComponent>();

            foreach (var renderingComponent in renderingComponents)
            {
                if (shadowCastersOnly && !renderingComponent.CastsShadow)
                {
                    continue;
                }

                var worldPosition =
                    positionComponent.Position
                    + renderingComponent.Offset
                    - (heightComponent is null ? Vector2.Zero : new Vector2(0, heightComponent.Z));

                var depthZ = renderingComponent.DepthHeightOverride ?? positionComponent.Position.Y;
                var layer = (depthZ + renderingComponent.DepthBias) / _config.WorldHeight;

                var spriteEffect =
                    (renderingComponent.FlipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None)
                    | (renderingComponent.FlipY ? SpriteEffects.FlipVertically : SpriteEffects.None);

                _spriteBatch.Draw(
                    renderingComponent.Texture,
                    worldPosition,
                    renderingComponent.SourceRectangle,
                    renderingComponent.Colour,
                    positionComponent.Rotation,
                    renderingComponent.Origin,
                    renderingComponent.Scale,
                    spriteEffect,
                    layer
                );
            }
        }
    }
}
