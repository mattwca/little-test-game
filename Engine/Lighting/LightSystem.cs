using System.Collections.Generic;
using System.Linq;

using Engine.Components;
using Engine.ECS;
using Engine.Rendering;
using Engine.Utils;

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
    private readonly SpriteRenderer _spriteRenderer;


    private readonly LinearColourGenerator _colourGenerator;

    private readonly Dictionary<string, RenderTarget2D> _shadowMaps;

    private readonly RenderTarget2D _occluderTexture;
    public readonly RenderTarget2D _shadowMapRenderTarget;
    private readonly Effect _shadowMapEffect;

    public LightSystem(
        EntityManager entityManager,
        ContentManager contentManager,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch,
        SpriteRenderer spriteRenderer
    ) {
        _entityManager = entityManager;
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        _spriteRenderer = spriteRenderer;

        _colourGenerator = new LinearColourGenerator(LinearColourGenerator.DefaultColours);

        _shadowMaps = new Dictionary<string, RenderTarget2D>();

        _occluderTexture = new RenderTarget2D(
            _graphicsDevice,
            _graphicsDevice.PresentationParameters.BackBufferWidth,
            _graphicsDevice.PresentationParameters.BackBufferHeight
        );

        _shadowMapRenderTarget = new RenderTarget2D(
            _graphicsDevice,
            512,
            1
        );

        _shadowMapEffect = _contentManager.Load<Effect>("Effects/ShadowMapEffect");
    }

    public void Draw(float deltaTime)
    {
        var lightEntity = _entityManager.GetEntitiesWithComponents(typeof(LightComponent), typeof(PositionComponent)).FirstOrDefault();
        if (lightEntity == null)
            return;

        // Update the light texture render target
        RenderOccluders();
        RenderShadowMap(lightEntity.GetComponent<PositionComponent>());
    }

    public void Update(float deltaTime)
    {
        var lightEntity = _entityManager.GetEntityWithComponent<LightComponent>()!;
        var lightComponent = lightEntity.GetComponent<LightComponent>();
        var renderingComponent = lightEntity.GetComponent<RenderingComponent>();

        var colour = _colourGenerator.GetCyclingColor(deltaTime);
        lightComponent.Colour = colour;
        renderingComponent.Colour = colour;
    }

    private void RenderOccluders()
    {
        _graphicsDevice.WithRenderTarget(_occluderTexture, () =>
        {
            _graphicsDevice.Clear(Color.Transparent);
            _spriteRenderer.RenderSprites((entity) =>
            {
                var renderingComponent = entity.GetComponent<RenderingComponent>();
                return renderingComponent.CastsShadow;
            });
        });
    }

    private void RenderShadowMap(PositionComponent lightPosition)
    {
        var cameraEntity = _entityManager.GetEntitiesWithComponent<CameraComponent>().FirstOrDefault();
        var cameraComponent = cameraEntity?.GetComponent<CameraComponent>();

        _shadowMapEffect.Parameters["LightPosition"].SetValue(Vector2.Transform(lightPosition.Centre, cameraComponent!.Transform));
        _shadowMapEffect.Parameters["Resolution"].SetValue(new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height));

        _graphicsDevice.SetRenderTarget(_shadowMapRenderTarget);
        _graphicsDevice.Clear(Color.Transparent);

        _spriteBatch.Begin(SpriteSortMode.Immediate, effect: _shadowMapEffect);
        _spriteBatch.Draw(_occluderTexture, new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height), Color.White);
        _spriteBatch.End();

        _graphicsDevice.SetRenderTarget(null);
    }
}