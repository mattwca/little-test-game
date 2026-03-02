using System.Linq;

using Engine.Components;
using Engine.ECS;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Lighting;

public class LightSystem : IRenderSystem, IUpdateSystem
{
    private readonly EntityManager _entityManager;
    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;

    private readonly RenderTarget2D _lightRenderTarget;
    public readonly RenderTarget2D _shadowMapRenderTarget;
    private readonly Effect _shadowMapEffect;

    public LightSystem(EntityManager entityManager, ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _entityManager = entityManager;
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;

        _lightRenderTarget = new RenderTarget2D(
            _graphicsDevice,
            _graphicsDevice.PresentationParameters.BackBufferWidth,
            _graphicsDevice.PresentationParameters.BackBufferHeight
        );

        _shadowMapRenderTarget = new RenderTarget2D(
            _graphicsDevice,
            512,
            512
        );

        _shadowMapEffect = _contentManager.Load<Effect>("Effects/ShadowMapEffect");
    }

    public void Draw(float deltaTime)
    {
        var lightEntity = _entityManager.GetEntitiesWithComponents(typeof(LightComponent), typeof(PositionComponent)).FirstOrDefault();
        if (lightEntity == null)
            return;

        var lightComponent = lightEntity.GetComponent<LightComponent>();
        var positionComponent = lightEntity.GetComponent<PositionComponent>();

        // Update the light texture render target
        RenderLightTexture();
        RenderShadowMap();
    }

    public void Update(float deltaTime)
    {
        // TODO
    }

    private void RenderLightTexture()
    {
        var cameraEntity = _entityManager.GetEntityWithComponent<CameraComponent>();
        var cameraComponent = cameraEntity!.GetComponent<CameraComponent>();

        _graphicsDevice.SetRenderTarget(_lightRenderTarget);
        _graphicsDevice.Clear(Color.Transparent);
        _spriteBatch.Begin(blendState: BlendState.Additive, transformMatrix: cameraComponent.Transform);

        var entitiesToRender = _entityManager.GetEntitiesWithComponents(typeof(RenderingComponent), typeof(PositionComponent));
        foreach (var entity in entitiesToRender)
        {
            var position = entity.GetComponent<PositionComponent>();
            var sprite = entity.GetComponent<RenderingComponent>();

            _spriteBatch.Draw(
                sprite.Texture,
                position.Position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                sprite.Scale,
                SpriteEffects.None,
                sprite.Layer
            );
        }

        _spriteBatch.End();
        _graphicsDevice.SetRenderTarget(null);
    }

    private void RenderShadowMap()
    {
        _graphicsDevice.SetRenderTarget(_shadowMapRenderTarget);
        _graphicsDevice.Clear(Color.White);

        _spriteBatch.Begin(effect: _shadowMapEffect);
        _spriteBatch.Draw(_lightRenderTarget, Vector2.Zero, Color.White);
        _spriteBatch.End();

        _graphicsDevice.SetRenderTarget(null);
    }
}