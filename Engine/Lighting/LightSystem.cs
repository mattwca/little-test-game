using System;
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

public class LightSystem : IRenderSystem, IRenderSystemOrder
{
    private readonly EntityManager _entityManager;
    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly Renderer _renderer;

    private readonly RenderTarget2D _occluderTexture;
    private readonly Effect _shadowMapEffect;

    private readonly Vector2 _lightPositionOffset = new Vector2(0, 0);

    public Dictionary<string, RenderTarget2D> ShadowMaps { get; }
    public int RenderOrder
    {
        get => 0;
    }

    public LightSystem(
        EntityManager entityManager,
        ContentManager contentManager,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch,
        Renderer renderer
    )
    {
        _entityManager = entityManager;
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        _renderer = renderer;

        ShadowMaps = new Dictionary<string, RenderTarget2D>();

        _occluderTexture = new RenderTarget2D(
            _graphicsDevice,
            _graphicsDevice.PresentationParameters.BackBufferWidth,
            _graphicsDevice.PresentationParameters.BackBufferHeight
        );

        _shadowMapEffect = _contentManager.Load<Effect>("Effects/ShadowMapEffect");
    }

    // public Entity[] GetVisibleLights()
    // {
    //     var lights = _entityManager.GetEntitiesWithComponents(
    //         typeof(LightComponent),
    //         typeof(PositionComponent),
    //         typeof(VisibilityComponent)
    //     );
    //     return lights.Where((light) => light.GetComponent<VisibilityComponent>().IsVisible).ToArray();
    // }

    public Entity[] GetVisibleLights()
    {
        var lights = _entityManager.GetEntitiesWithComponents(
            typeof(LightComponent),
            typeof(PositionComponent),
            typeof(VisibilityComponent)
        );

        var particleEntities = _entityManager
            .GetEntitiesWithComponents(typeof(ParticleEmitterComponent), typeof(PositionComponent))
            .Where(
                (emitter) =>
                {
                    var emitterComponent = emitter.GetComponent<ParticleEmitterComponent>();
                    return emitterComponent.Enabled
                        && emitterComponent.ParticleType.LightingConfig?.LightingOption
                            == ParticleLightingOption.EmitsLight;
                }
            )
            .SelectMany(
                (emitter) =>
                {
                    var emitterComponent = emitter.GetComponent<ParticleEmitterComponent>();
                    var emitterPosition = emitter.GetComponent<PositionComponent>();

                    var particleIndex = 0;
                    var particleEntities = emitterComponent.Particles.Aggregate(
                        new List<Entity>(),
                        (prev, particle) =>
                        {
                            if (particle.Age <= 0f)
                            {
                                particleIndex++;
                                return prev;
                            }

                            var particleId = $"{emitter.Id}-particle:{particleIndex}";
                            var positionComponent = new PositionComponent(particle.Position);

                            var (
                                _,
                                Intensity,
                                Radius,
                                AttenuationConstant,
                                AttenuationLinear,
                                AttenuationQuadratic,
                                WindowExponent
                            ) = emitterComponent.ParticleType.LightingConfig!;

                            var lightComponent = new LightComponent(
                                colour: particle.Colour,
                                intensity: Intensity,
                                radius: Radius,
                                attenuationConstant: AttenuationConstant,
                                attenuationLinear: AttenuationLinear,
                                attenuationQuadratic: AttenuationQuadratic
                            );

                            var particleEntity = new Entity(particleId);
                            particleEntity.AddComponent(positionComponent).AddComponent(lightComponent);

                            particleIndex++;

                            return [.. prev, particleEntity];
                        }
                    );

                    return particleEntities;
                }
            )
            .ToList();

        return [.. lights, .. particleEntities];
    }

    public void Draw(GameTime gameTime)
    {
        // Update the light texture render target
        RenderOccluders();

        var visibleLights = GetVisibleLights();
        foreach (var light in visibleLights)
        {
            var positionComponent = light.GetComponent<PositionComponent>();
            var lightComponent = light.GetComponent<LightComponent>();

            RenderShadowMap(light.Id, positionComponent, lightComponent);
        }
    }

    private void RenderOccluders()
    {
        _graphicsDevice.WithRenderTarget(
            _occluderTexture,
            () =>
            {
                _graphicsDevice.Clear(Color.Transparent);
                _renderer.Render(true);
            }
        );
    }

    private void RenderShadowMap(string entityId, PositionComponent lightPosition, LightComponent lightComponent)
    {
        var cameraEntity = _entityManager.GetEntitiesWithComponent<CameraComponent>().FirstOrDefault();
        var cameraComponent = cameraEntity?.GetComponent<CameraComponent>();

        _shadowMapEffect
            .Parameters["LightPosition"]
            .SetValue(Vector2.Transform(lightPosition.Centre + _lightPositionOffset, cameraComponent!.Transform));
        _shadowMapEffect
            .Parameters["Resolution"]
            .SetValue(new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height));
        _shadowMapEffect.Parameters["LightRadius"].SetValue(lightComponent.Radius / _graphicsDevice.Viewport.Height);

        if (!ShadowMaps.ContainsKey(entityId))
        {
            ShadowMaps.Add(entityId, CreateShadowMap());
        }

        _graphicsDevice.SetRenderTarget(ShadowMaps[entityId]);
        _graphicsDevice.Clear(Color.Transparent);

        _spriteBatch.Begin(SpriteSortMode.Immediate, effect: _shadowMapEffect);
        _spriteBatch.Draw(
            _occluderTexture,
            new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height),
            Color.White
        );
        _spriteBatch.End();

        _graphicsDevice.SetRenderTarget(null);
    }

    private RenderTarget2D CreateShadowMap()
    {
        return new RenderTarget2D(_graphicsDevice, 512, 1);
    }
}
