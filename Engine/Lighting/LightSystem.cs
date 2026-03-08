using System.Collections.Generic;
using System.Linq;

using Engine.Components;
using Engine.ECS;
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

    private readonly LinearColourGenerator _colourGenerator;

    private readonly Dictionary<string, RenderTarget2D> _shadowMaps;

    private readonly RenderTarget2D _lightRenderTarget;
    public readonly RenderTarget2D _shadowMapRenderTarget;
    private readonly Effect _shadowMapEffect;

    public LightSystem(EntityManager entityManager, ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _entityManager = entityManager;
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;

        _colourGenerator = new LinearColourGenerator(LinearColourGenerator.DefaultColours);

        _shadowMaps = new Dictionary<string, RenderTarget2D>();

        _lightRenderTarget = new RenderTarget2D(
            _graphicsDevice,
            _graphicsDevice.PresentationParameters.BackBufferWidth,
            _graphicsDevice.PresentationParameters.BackBufferHeight
        );

        // _shadowMapRenderTarget = new RenderTarget2D(
        //     _graphicsDevice,
        //     512,
        //     1
        // );

        _shadowMapEffect = _contentManager.Load<Effect>("Effects/ShadowMapEffect");
    }

    public void Draw(float deltaTime)
    {
        var lightEntity = _entityManager.GetEntitiesWithComponents(typeof(LightComponent), typeof(PositionComponent)).FirstOrDefault();
        if (lightEntity == null)
            return;

        // Update the light texture render target
        RenderLightTexture(lightEntity);
        RenderShadowMap(lightEntity.GetComponent<PositionComponent>());
    }

    public void Update(float deltaTime)
    {
        // TODO

        var lightEntity = _entityManager.GetEntityWithComponent<LightComponent>()!;
        var lightComponent = lightEntity.GetComponent<LightComponent>();
        var renderingComponent = lightEntity.GetComponent<RenderingComponent>();

        var colour = _colourGenerator.GetCyclingColor(deltaTime);
        lightComponent.Colour = colour;
        renderingComponent.Colour = colour;
    }

    private void RenderLightTexture(Entity lightEntity)
    {
        var cameraEntity = _entityManager.GetEntityWithComponent<CameraComponent>();
        var cameraComponent = cameraEntity!.GetComponent<CameraComponent>();

        _graphicsDevice.SetRenderTarget(_lightRenderTarget);
        _graphicsDevice.Clear(Color.Transparent);
        _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp, transformMatrix: cameraComponent?.Transform);

        var entitiesToRender = _entityManager.GetEntitiesWithComponents(typeof(RenderingComponent), typeof(PositionComponent));

        if (entitiesToRender.Contains(lightEntity))
        {
            entitiesToRender.Remove(lightEntity);
        }

        entitiesToRender.ForEach(entity =>
        {
            var positionComponent = entity.GetComponent<PositionComponent>();
            var renderingComponents = entity.GetComponents<RenderingComponent>();
            foreach (var component in renderingComponents)
            {
                var worldPosition = positionComponent.Position + component.Offset;

                _spriteBatch.Draw(
                    component.Texture,
                    worldPosition,
                    null,
                    component.Colour,
                    0f,
                    scale: component.Scale,
                    layerDepth: component.Layer / 100f,
                    origin: Vector2.Zero,
                    effects: (component.FlipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None) | (component.FlipY ? SpriteEffects.FlipVertically : SpriteEffects.None)
                );
            }
        });

        _spriteBatch.End();
        _graphicsDevice.SetRenderTarget(null);
    }

    private void RenderShadowMap(PositionComponent lightPosition)
    {
        var cameraEntity = _entityManager.GetEntitiesWithComponent<CameraComponent>().FirstOrDefault();
        var cameraComponent = cameraEntity?.GetComponent<CameraComponent>();

        _shadowMapEffect.Parameters["LightPosition"].SetValue(Vector2.Transform(lightPosition.Centre, cameraComponent!.Transform));
        _shadowMapEffect.Parameters["Resolution"].SetValue(new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height));

        _graphicsDevice.SetRenderTarget(_shadowMapRenderTarget);
        _graphicsDevice.Clear(Color.White);

        _spriteBatch.Begin(SpriteSortMode.Immediate, effect: _shadowMapEffect);
        _spriteBatch.Draw(_lightRenderTarget, new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height), Color.White);
        _spriteBatch.End();

        _graphicsDevice.SetRenderTarget(null);
    }
}