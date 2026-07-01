using System.Linq;
using Engine.Components;
using Engine.ECS;
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
    private readonly EntityManager _entityManager;
    private readonly SpriteBatch _spriteBatch;

    public SpriteRenderer(ContentManager contentManager, EntityManager entityManager, SpriteBatch spriteBatch)
    {
        _contentManager = contentManager;
        _entityManager = entityManager;
        _spriteBatch = spriteBatch;
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
                var layer = depthZ / MAX_WORLD_HEIGHT;

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
