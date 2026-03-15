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


    private readonly RenderTarget2D _occluderTexture;
    private readonly Effect _shadowMapEffect;

    public Dictionary<string, RenderTarget2D> ShadowMaps { get; }

    public LightSystem(
        EntityManager entityManager,
        ContentManager contentManager,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch,
        SpriteRenderer spriteRenderer
    )
    {
        _entityManager = entityManager;
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        _spriteRenderer = spriteRenderer;

        ShadowMaps = new Dictionary<string, RenderTarget2D>();

        _occluderTexture = new RenderTarget2D(
            _graphicsDevice,
            _graphicsDevice.PresentationParameters.BackBufferWidth,
            _graphicsDevice.PresentationParameters.BackBufferHeight
        );

        _shadowMapEffect = _contentManager.Load<Effect>("Effects/ShadowMapEffect");
    }

    public Entity[] GetVisibleLights()
    {
        var lights = _entityManager.GetEntitiesWithComponents(typeof(LightComponent), typeof(PositionComponent), typeof(VisibilityComponent));
        return lights.Where((light) => light.GetComponent<VisibilityComponent>().IsVisible).ToArray();
    }

    public void Draw(GameTime gameTime)
    {
        // Update the light texture render target
        RenderOccluders();
        
        var visibleLights = GetVisibleLights();
        foreach (var light in visibleLights)
        {
            var positionComponent = light.GetComponent<PositionComponent>();
            RenderShadowMap(light.Id, positionComponent);
        }
    }

    public void Update(GameTime gameTime)
    {
        // var lightEntity = _entityManager.GetEntityWithComponent<LightComponent>()!;
        // var lightComponent = lightEntity.GetComponent<LightComponent>();
        // var renderingComponent = lightEntity.GetComponent<RenderingComponent>();

        // var colour = _colourGenerator.GetCyclingColor(deltaTime);
        // lightComponent.Colour = colour;
        // renderingComponent.Colour = colour;
    }

    private void RenderOccluders()
    {
        _graphicsDevice.WithRenderTarget(_occluderTexture, () =>
        {
            _graphicsDevice.Clear(Color.Transparent);
            _spriteRenderer.RenderSprites((renderComponent) =>
            {
                return renderComponent.CastsShadow;
            });
        });
    }

    private void RenderShadowMap(string entityId, PositionComponent lightPosition)
    {
        var cameraEntity = _entityManager.GetEntitiesWithComponent<CameraComponent>().FirstOrDefault();
        var cameraComponent = cameraEntity?.GetComponent<CameraComponent>();

        _shadowMapEffect.Parameters["LightPosition"].SetValue(Vector2.Transform(lightPosition.Centre, cameraComponent!.Transform));
        _shadowMapEffect.Parameters["Resolution"].SetValue(new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height));

        if (!ShadowMaps.ContainsKey(entityId))
        {
            ShadowMaps.Add(entityId, CreateShadowMap());
        }

        _graphicsDevice.SetRenderTarget(ShadowMaps[entityId]);
        _graphicsDevice.Clear(Color.Transparent);

        _spriteBatch.Begin(SpriteSortMode.Immediate, effect: _shadowMapEffect);
        _spriteBatch.Draw(_occluderTexture, new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height), Color.White);
        _spriteBatch.End();

        _graphicsDevice.SetRenderTarget(null);
    }

    private RenderTarget2D CreateShadowMap()
    {
        return new RenderTarget2D(_graphicsDevice, 512, 1);
    }
}