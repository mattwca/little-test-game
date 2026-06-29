using System;
using System.Linq;
using Engine.Components;
using Engine.ECS;
using Engine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering;

public class ParticleRenderer
{
    private readonly EntityManager _entityManager;
    private readonly ContentManager _contentManager;
    private readonly SpriteBatch _spriteBatch;
    private readonly Helper _helper;

    public ParticleRenderer(
        EntityManager entityManager,
        ContentManager contentManager,
        SpriteBatch spriteBatch,
        Helper helper
    )
    {
        _entityManager = entityManager;
        _contentManager = contentManager;
        _spriteBatch = spriteBatch;
        _helper = helper;
    }

    public void RenderParticles(bool onlyShadowCasters = false)
    {
        var cameraTransform = _helper.GetCameraTransform();

        _spriteBatch.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            samplerState: SamplerState.PointClamp,
            blendState: BlendState.AlphaBlend,
            transformMatrix: cameraTransform
        // effect: _spriteEffect
        );

        var emitterEntities = _entityManager.GetEntitiesWithComponents(
            typeof(ParticleEmitterComponent),
            typeof(PositionComponent)
        );

        if (onlyShadowCasters)
        {
            emitterEntities =
            [
                .. emitterEntities.Where(
                    (entity) =>
                    {
                        var emitterComponent = entity.GetComponent<ParticleEmitterComponent>();
                        return emitterComponent.ParticleType.CastsShadow;
                    }
                ),
            ];
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

                var worldPosition = emitterPosition.Position + particle.Position;

                var fadeOutValue = emitterComponent.ParticleType.FadeOut
                    ? (particle.Age / emitterComponent.SpawnConfig.LifespanSeconds * 128) / 128
                    : 128;
                var fadeOutColour = new Color(fadeOutValue, fadeOutValue, fadeOutValue, fadeOutValue);

                var particleColour = particle.Colour * fadeOutColour;

                _spriteBatch.Draw(emitterTexture, worldPosition, particleColour);
            }
        }

        _spriteBatch.End();
    }
}
