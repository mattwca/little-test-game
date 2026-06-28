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
                        return emitterComponent.CastsShadows;
                    }
                ),
            ];
        }

        foreach (var emitter in emitterEntities)
        {
            var emitterComponent = emitter.GetComponent<ParticleEmitterComponent>();
            var emitterPosition = emitter.GetComponent<PositionComponent>();

            var emitterTexture = _contentManager.Load<Texture2D>(emitterComponent.ParticleTexture);

            foreach (var particle in emitterComponent.Particles)
            {
                if (particle.Age <= 0)
                {
                    continue;
                }

                var worldPosition = emitterPosition.Position + particle.Position;
                var opacity = (particle.Age / emitterComponent.MaxAge * 128) / 128;

                _spriteBatch.Draw(emitterTexture, worldPosition, new Color(opacity, opacity, opacity, opacity));
            }
        }

        _spriteBatch.End();
    }
}
