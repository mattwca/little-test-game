using Engine.Components;
using Engine.ECS;
using Engine.Lighting;
using Engine.Rendering;
using Engine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Systems;

public class RenderingSystem : IRenderSystem, IRenderSystemOrder
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ContentManager _contentManager;
    private readonly StateManager _stateManager;

    private readonly LightSystem _lightSystem;
    private readonly SpriteBatch _spriteBatch;
    private readonly Helper _helper;

    private readonly Effect _lightingEffect;

    private readonly SpriteRenderer _spriteRenderer;
    private readonly TileRenderer _tileRenderer;
    private readonly ParticleRenderer _particleRenderer;

    private readonly RenderTarget2D _sceneBuffer;
    private readonly RenderTarget2D _lightingBuffer;

    public int RenderOrder
    {
        get => 1;
    }

    private static readonly BlendState MultiplyBlend = new()
    {
        ColorSourceBlend = Blend.DestinationColor,
        ColorDestinationBlend = Blend.Zero,
    };

    public RenderingSystem(
        GraphicsDevice graphicsDevice,
        ContentManager contentManager,
        StateManager stateManager,
        LightSystem lightSystem,
        SpriteBatch spriteBatch,
        Helper helper,
        SpriteRenderer spriteRenderer,
        TileRenderer tileRenderer,
        ParticleRenderer particleRenderer
    )
    {
        _graphicsDevice = graphicsDevice;
        _contentManager = contentManager;
        _stateManager = stateManager;

        _lightSystem = lightSystem;
        _spriteBatch = spriteBatch;
        _helper = helper;

        _spriteRenderer = spriteRenderer;
        _tileRenderer = tileRenderer;
        _particleRenderer = particleRenderer;

        _lightingEffect = _contentManager.Load<Effect>("Effects/LightingEffect");

        _sceneBuffer = new RenderTarget2D(
            _graphicsDevice,
            _graphicsDevice.Viewport.Width,
            _graphicsDevice.Viewport.Height
        );
        _lightingBuffer = new RenderTarget2D(
            _graphicsDevice,
            _graphicsDevice.Viewport.Width,
            _graphicsDevice.Viewport.Height
        );
    }

    public void Draw(GameTime gameTime)
    {
        if (_stateManager.GetBool("renderLightingMap"))
        {
            DrawDebugMode();
            return;
        }

        DrawWithLighting();
    }

    private void DrawDebugMode()
    {
        RenderSceneBuffer();
        RenderLightingBuffer();

        _graphicsDevice.Clear(Color.White);

        _spriteBatch.Begin();
        _spriteBatch.Draw(_lightingBuffer, ViewportRectangle, Color.White);
        _spriteBatch.End();
    }

    /// <summary>
    /// Draws the current scene with lighting effects.
    /// </summary>
    private void DrawWithLighting()
    {
        RenderSceneBuffer();
        RenderLightingBuffer();

        _spriteBatch.Begin(blendState: BlendState.Opaque);
        _spriteBatch.Draw(_sceneBuffer, ViewportRectangle, Color.White);
        _spriteBatch.End();

        _spriteBatch.Begin(blendState: MultiplyBlend);
        _spriteBatch.Draw(_lightingBuffer, ViewportRectangle, Color.White);
        _spriteBatch.End();
    }

    /// <summary>
    /// Renders the base scene to the scene buffer render texture, including tiles and sprites.
    /// </summary>
    private void RenderSceneBuffer()
    {
        _graphicsDevice.WithRenderTarget(
            _sceneBuffer,
            () =>
            {
                _graphicsDevice.Clear(Color.Transparent);
                _tileRenderer.RenderTiles();
                _spriteRenderer.RenderSprites();
                _particleRenderer.RenderParticles();
            }
        );
    }

    /// <summary>
    /// Renders each of the light sources to the lighting buffer render texture. This is a buffer
    /// which contains the accumulation of each light's contribution to the scene.
    ///
    /// We clear the backbuffer with an ambient "unlit" colour, before rendering each of the lights
    /// to the buffer.
    /// </summary>
    private void RenderLightingBuffer()
    {
        _graphicsDevice.WithRenderTarget(
            _lightingBuffer,
            () =>
            {
                // Ambient base colour for unlit pixels.
                _graphicsDevice.Clear(new Color(0.7f, 0.7f, 0.7f, 1.0f));

                // Render the lighting contributions to the buffer.
                /// - Pixels which aren't obscured by a shadow (are lit) add the light's colour, scaled by its
                ///   falloff curve.
                /// - Pixels which are shadowed, or are outside the light's radius, emit nothing, leaving the
                ///   ambient base colour.
                var visibleLights = _lightSystem.GetVisibleLights();
                foreach (var light in visibleLights)
                {
                    RenderLightingPass(light);
                }
            }
        );
    }

    private void RenderLightingPass(Entity lightEntity)
    {
        var lightPositionComponent = lightEntity.GetComponent<PositionComponent>();
        var lightComponent = lightEntity.GetComponent<LightComponent>();
        var shadowMap = _lightSystem.ShadowMaps[lightEntity.Id];

        var screenSize = new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
        var cameraTransform = _helper.GetCameraTransform();

        _lightingEffect
            .Parameters["LightPosition"]
            .SetValue(Vector2.Transform(lightPositionComponent.Centre, cameraTransform));
        _lightingEffect.Parameters["LightColour"].SetValue(lightComponent.Colour.ToVector4());
        _lightingEffect.Parameters["LightRadius"].SetValue(lightComponent.Radius / _graphicsDevice.Viewport.Height);
        _lightingEffect.Parameters["ScreenSize"].SetValue(screenSize);
        _lightingEffect.Parameters["ShadowMap"].SetValue(shadowMap);

        _spriteBatch.Begin(effect: _lightingEffect, blendState: BlendState.Additive);
        _spriteBatch.Draw(_sceneBuffer, ViewportRectangle, Color.White);
        _spriteBatch.End();
    }

    private Rectangle ViewportRectangle => new(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
}
