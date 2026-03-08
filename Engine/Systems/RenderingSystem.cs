using System.Linq;

using Engine.Components;
using Engine.ECS;
using Engine.Lighting;
using Engine.Rendering;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Systems;

public class RenderingSystem : IRenderSystem
{
    // private readonly ContentManager _contentManager;
    // private readonly SpriteBatch _spriteBatch;
    // private readonly EntityManager _entityManager;
    // private readonly LightSystem _lightSystem;
    private readonly GraphicsDevice _graphicsDevice;

    // private readonly Effect _spriteEffect;
    // private readonly Effect _shadowEffect;

    private SpriteRenderer _spriteRenderer;
    private TileRenderer _tileRenderer;
    
    // private readonly RenderTarget2D _renderedFrame;
    // private readonly RenderTarget2D _shadows;

    // private readonly SpriteRenderer _spriteRenderer;

    public RenderingSystem(GraphicsDevice graphicsDevice, SpriteRenderer spriteRenderer, TileRenderer tileRenderer)
    {
        _graphicsDevice = graphicsDevice;
        _spriteRenderer = spriteRenderer;
        _tileRenderer = tileRenderer;
    }

    // public RenderingSystem(ContentManager contentManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, EntityManager entityManager, SpriteRenderer spriteRenderer, LightSystem lightSystem)
    // {
    //     _contentManager = contentManager;
    //     _graphicsDevice = graphicsDevice;
    //     _spriteBatch = spriteBatch;
    //     _entityManager = entityManager;
    //     _spriteRenderer = spriteRenderer;
    //     _lightSystem = lightSystem;

    //     _spriteEffect = _contentManager.Load<Effect>("Effects/SpriteEffect");
    //     _shadowEffect = _contentManager.Load<Effect>("Effects/ShadowEffect");

    //     _renderedFrame = new RenderTarget2D(_graphicsDevice, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
    // }

    public void Draw(float deltaTime)
    {
        _graphicsDevice.Clear(Color.CornflowerBlue);
        _tileRenderer.RenderTiles();
        _spriteRenderer.RenderSprites();

        // _graphicsDevice.SetRenderTarget(_renderedFrame);
        // _graphicsDevice.Clear(Color.Transparent);
        // RenderSprites();

        // _graphicsDevice.SetRenderTarget(null);
        // _graphicsDevice.Clear(Color.CornflowerBlue);
        // RenderTiles();
        // RenderShadowPass();
    }

    // private void RenderTiles()
    // {
    //     var cameraEntity = _entityManager.GetEntitiesWithComponent<CameraComponent>().FirstOrDefault();
    //     var cameraComponent = cameraEntity?.GetComponent<CameraComponent>();

    //     _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: cameraComponent?.Transform);

    //     var tiledRenderEntities = _entityManager.GetEntitiesWithComponents(typeof(PositionComponent), typeof(TiledRenderingComponent));

    //     foreach (var entity in tiledRenderEntities)
    //     {
    //         var tiledRenderComponent = entity.GetComponent<TiledRenderingComponent>();
    //         var positionComponent = entity.GetComponent<PositionComponent>();

    //         for (int x = 0; x < tiledRenderComponent.TilesX; x++)
    //         {
    //             for (int y = 0; y < tiledRenderComponent.TilesY; y++)
    //             {
    //                 var worldPosition = positionComponent.Position + tiledRenderComponent.Offset;
    //                 var tilePosition = worldPosition + new Vector2(x * tiledRenderComponent.TileSize, y * tiledRenderComponent.TileSize);
    //                 _spriteBatch.Draw(tiledRenderComponent.Texture, tilePosition, tiledRenderComponent.Colour);
    //             }
    //         }
    //     }

    //     _spriteBatch.End();
    // }

    // private void RenderSprites()
    // {   
        // _graphicsDevice.Clear(Color.Transparent);

        // var cameraEntity = _entityManager.GetEntitiesWithComponent<CameraComponent>().FirstOrDefault();
        // var cameraComponent = cameraEntity?.GetComponent<CameraComponent>();

        // var entitiesToRender = _entityManager.GetEntitiesWithComponents(typeof(RenderingComponent), typeof(PositionComponent));

        // _spriteBatch.Begin(
        //     sortMode: SpriteSortMode.BackToFront,
        //     samplerState: SamplerState.PointClamp,
        //     effect: _spriteEffect,
        //     transformMatrix: cameraComponent!.Transform
        // );

        // _spriteBatch.Draw(_lightSystem._shadowMapRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        // entitiesToRender.ForEach(entity =>
        // {
        //     var positionComponent = entity.GetComponent<PositionComponent>();
        //     var renderingComponents = entity.GetComponents<RenderingComponent>();
        //     foreach (var component in renderingComponents)
        //     {
        //         var worldPosition = positionComponent.Position + component.Offset;

        //         _spriteBatch.Draw(
        //             component.Texture,
        //             worldPosition,
        //             null,
        //             component.Colour,
        //             0f,
        //             scale: component.Scale,
        //             layerDepth: Math.Clamp(component.Layer / 100f, 0f, 1f),
        //             origin: Vector2.Zero,
        //             effects: (component.FlipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None) | (component.FlipY ? SpriteEffects.FlipVertically : SpriteEffects.None)
        //         );
        //     }
        // });

        // _spriteBatch.End();
    // }

    private void RenderShadowPass()
    {
        // var cameraEntity = _entityManager.GetEntitiesWithComponent<CameraComponent>().FirstOrDefault();
        // var cameraComponent = cameraEntity?.GetComponent<CameraComponent>();

        // var lightEntity = _entityManager.GetEntityWithComponent<LightComponent>()!;
        // var lightPosition = lightEntity.GetComponent<PositionComponent>().Centre;
        // var lightComponent = lightEntity.GetComponent<LightComponent>();

        // var screenSize = new Vector2(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);

        // _shadowEffect.Parameters["LightPosition"].SetValue(Vector2.Transform(lightPosition, cameraComponent!.Transform));
        // _shadowEffect.Parameters["LightColour"].SetValue(lightComponent.Colour.ToVector4());
        // _shadowEffect.Parameters["ScreenSize"].SetValue(screenSize);
        // _shadowEffect.Parameters["ShadowMap"].SetValue(_lightSystem._shadowMapRenderTarget);

        // _spriteBatch.Begin(effect: _shadowEffect);
        // _spriteBatch.Draw(_renderedFrame, new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height), Color.White);
        // _spriteBatch.End();
    }
}