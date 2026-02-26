using Engine.ECS;
using Engine.Components;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Engine.Systems;

public class RenderingSystem : IRenderSystem
{
    private SpriteBatch _spriteBatch;

    public RenderingSystem(SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;
    }

    public void Draw(EntityManager entityManager, float deltaTime)
    {
        var cameraEntity = entityManager.GetEntitiesWithComponent<CameraComponent>().FirstOrDefault();
        var cameraComponent = cameraEntity?.GetComponent<CameraComponent>();

        var entitiesToRender = entityManager.GetEntitiesWithComponent<RenderingComponent>();
        var sortedEntitiesToRender = entitiesToRender.OrderBy(entity => entity.GetComponent<RenderingComponent>()!.Layer).ToList();

        _spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp, transformMatrix: cameraComponent?.Transform);

        sortedEntitiesToRender.ForEach(entity =>
        {
            var renderingComponent = entity.GetComponent<RenderingComponent>();
            _spriteBatch.Draw(renderingComponent.Texture, renderingComponent.Position, renderingComponent.Colour);
        });

        _spriteBatch.End();
    }
}