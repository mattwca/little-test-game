using System;

using Engine.Animation;
using Engine.Components;
using Engine.ECS;

using Microsoft.Xna.Framework;

namespace Engine.Systems;

public class AnimationSystem : IUpdateSystem
{
    private readonly EntityManager _entityManager;
    private readonly AnimationManager _animationManager;

    public AnimationSystem(EntityManager entityManager, AnimationManager animationManager)
    {
        _entityManager = entityManager;
        _animationManager = animationManager;
    }

    public void Update(GameTime gameTime)
    {
        var entitiesWithAnimation = _entityManager.GetEntitiesWithComponents(typeof(AnimationComponent), typeof(RenderingComponent));

        foreach (var entity in entitiesWithAnimation)
        {
            var animationComponent = entity.GetComponent<AnimationComponent>();
            var renderingComponent = entity.GetComponent<RenderingComponent>();
            var animation = _animationManager.GetAnimation(animationComponent.AnimationKey)!;

            var increment = 10 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            animationComponent.FrameTime += increment;

            var currentFrameIndex = animationComponent.Enabled
                ? (int)animationComponent.FrameTime % animation.FrameCount
                : 0;

            var xCoord = animation.FrameWidth * currentFrameIndex;
            var yCoord = 0;

            var sourceRectangle = new Rectangle(
                xCoord,
                yCoord,
                animation.FrameWidth,
                animation.FrameHeight
            );

            renderingComponent.SourceRectangle = sourceRectangle;
        }
    }
}