using Engine.Configuration;
using Engine.ECS;
using Engine.Utils;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering;

public class Renderer
{
    private readonly SpriteBatch _spriteBatch;
    private readonly ContentManager _contentManager;
    private readonly Helper _helper;

    private readonly SpriteRenderer _spriteRenderer;
    private readonly ParticleRenderer _particleRenderer;
    private readonly Effect _spriteEffect;

    public Renderer(
        SpriteBatch spriteBatch,
        EntityManager entityManager,
        ContentManager contentManager,
        Helper helper,
        GameConfiguration config
    )
    {
        _spriteBatch = spriteBatch;
        _contentManager = contentManager;
        _helper = helper;

        _spriteRenderer = new SpriteRenderer(entityManager, spriteBatch, config);
        _particleRenderer = new ParticleRenderer(contentManager, entityManager, spriteBatch, config);

        _spriteEffect = _contentManager.Load<Effect>("Effects/SpriteEffect");
    }

    public void Render(bool shadowCastersOnly = false)
    {
        var cameraTransform = _helper.GetCameraTransform();

        _spriteBatch.Begin(
            sortMode: SpriteSortMode.FrontToBack,
            samplerState: SamplerState.PointClamp,
            transformMatrix: cameraTransform,
            effect: _spriteEffect
        );

        _spriteRenderer.RenderSprites(shadowCastersOnly);
        _particleRenderer.RenderParticles(shadowCastersOnly);

        _spriteBatch.End();
    }
}
