using System.Linq;
using Engine.Components;
using Engine.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering;

public class ParticleRenderer
{
    /// <summary>
    /// Max world height for layer depth calcs
    /// </summary>
    private const int MAX_WORLD_HEIGHT = 1000;

    private readonly ContentManager _contentManager;
    private readonly EntityManager _entityManager;
    private readonly SpriteBatch _spriteBatch;

    public ParticleRenderer(ContentManager contentManager, EntityManager entityManager, SpriteBatch spriteBatch)
    {
        _contentManager = contentManager;
        _entityManager = entityManager;
        _spriteBatch = spriteBatch;
    }

    public void RenderParticles(bool shadowCastersOnly)
    {
        var emitterEntities = _entityManager.GetEntitiesWithComponents(
            typeof(ParticleEmitterComponent),
            typeof(PositionComponent)
        );

        if (shadowCastersOnly)
        {
            emitterEntities = emitterEntities
                .Where(emitter =>
                    emitter.GetComponent<ParticleEmitterComponent>()!.ParticleType?.LightingConfig?.LightingOption
                    == ParticleLightingOption.CastsShadow
                )
                .ToList();
        }

        foreach (var emitter in emitterEntities)
        {
            var emitterComponent = emitter.GetComponent<ParticleEmitterComponent>();
            var emitterPosition = emitter.GetComponent<PositionComponent>();

            var emitterTexture = _contentManager.Load<Texture2D>(emitterComponent.ParticleType.ParticleTexture);

            foreach (var particle in emitterComponent.Particles)
            {
                if (particle.Age <= 0)
                {
                    continue;
                }

                var depthZ = emitterComponent.RenderConfig?.OverrideDepthZ ?? emitterPosition.Position.Y;
                var layer = depthZ / MAX_WORLD_HEIGHT;

                var fadeOutValue = emitterComponent.ParticleType.FadeOut
                    ? (particle.Age / emitterComponent.SpawnConfig.LifespanSeconds * 128) / 128
                    : 128;
                var fadeOutColour = new Color(fadeOutValue, fadeOutValue, fadeOutValue, fadeOutValue);
                var particleColour = particle.Colour * fadeOutColour;

                _spriteBatch.Draw(
                    emitterTexture,
                    particle.Position,
                    null,
                    particleColour,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    layer
                );
            }
        }
    }
}
