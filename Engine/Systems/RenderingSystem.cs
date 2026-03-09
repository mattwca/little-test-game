using System.Linq;

using Engine.Components;
using Engine.ECS;
using Engine.Lighting;
using Engine.Rendering;
using Engine.Utils;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Systems;

public class RenderingSystem : IRenderSystem
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly EntityManager _entityManager;
    private readonly ContentManager _contentManager;
    private readonly LightSystem _lightSystem;
    private readonly SpriteBatch _spriteBatch;
    private readonly Helper _helper;

    private readonly Effect _shadowEffect;

    private readonly SpriteRenderer _spriteRenderer;
    private readonly TileRenderer _tileRenderer;
    
    private readonly RenderTarget2D _renderedFrame;

    public RenderingSystem(GraphicsDevice graphicsDevice, EntityManager entityManager, ContentManager contentManager, LightSystem lightSystem, SpriteBatch spriteBatch, Helper helper, SpriteRenderer spriteRenderer, TileRenderer tileRenderer)
    {
        _graphicsDevice = graphicsDevice;
        _entityManager = entityManager;
        _contentManager = contentManager;
        _lightSystem = lightSystem;
        _spriteBatch = spriteBatch;
        _helper = helper;

        _spriteRenderer = spriteRenderer;
        _tileRenderer = tileRenderer;

        _shadowEffect = _contentManager.Load<Effect>("Effects/ShadowEffect");
        _renderedFrame = new RenderTarget2D(_graphicsDevice, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
    }

    public void Draw(float deltaTime)
    {
        _graphicsDevice.WithRenderTarget(_renderedFrame, () => {
            _graphicsDevice.Clear(Color.Transparent);
            _spriteRenderer.RenderSprites();
        });

        _tileRenderer.RenderTiles();

        var visibleLights = _lightSystem.GetVisibleLights();
        foreach (var visibleLight in visibleLights) {
            RenderShadowPass(visibleLight);
        }
    }

    private void RenderShadowPass(Entity lightEntity)
    {
        var lightPositionComponent = lightEntity.GetComponent<PositionComponent>();
        var lightComponent = lightEntity.GetComponent<LightComponent>();
        var shadowMap = _lightSystem.ShadowMaps[lightEntity.Id];

        var screenSize = new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
        var cameraTransform = _helper.GetCameraTransform();

        _shadowEffect.Parameters["LightPosition"].SetValue(Vector2.Transform(lightPositionComponent.Centre, cameraTransform));
        _shadowEffect.Parameters["LightColour"].SetValue(lightComponent.Colour.ToVector4());
        _shadowEffect.Parameters["ScreenSize"].SetValue(screenSize);
        _shadowEffect.Parameters["ShadowMap"].SetValue(shadowMap);

        _spriteBatch.Begin(effect: _shadowEffect);
        _spriteBatch.Draw(_renderedFrame, new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height), Color.White);
        _spriteBatch.End();
    }
}