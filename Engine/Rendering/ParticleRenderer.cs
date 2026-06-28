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

    public void RenderParticles()
    {
        var cameraTransform = _helper.GetCameraTransform();

        _spriteBatch.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            samplerState: SamplerState.PointClamp,
            transformMatrix: cameraTransform
        // effect: _spriteEffect
        );

        var emitterEntities = _entityManager.GetEntitiesWithComponents(
            typeof(ParticleEmitterComponent),
            typeof(PositionComponent)
        );

        foreach (var emitter in emitterEntities)
        {
            var emitterComponent = emitter.GetComponent<ParticleEmitterComponent>();
            var emitterPosition = emitter.GetComponent<PositionComponent>();

            var emitterTexture = _contentManager.Load<Texture2D>(emitterComponent.ParticleTexture);

            foreach (var particle in emitterComponent.Particles)
            {
                if (particle.Age > emitterComponent.MaxAge)
                {
                    continue;
                }

                var worldPosition = emitterPosition.Position + particle.Position;
                _spriteBatch.Draw(emitterTexture, worldPosition, Color.White);
            }
        }

        _spriteBatch.End();
    }
}
