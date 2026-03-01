using Engine.Components;
using Engine.ECS;

namespace Engine.Systems;

public class AnimationSystem : IUpdateSystem
{
    public void Update(EntityManager entityManager, float deltaTime)
    {
        var entitiesWithAnimation = entityManager.GetEntitiesWithComponent<AnimationComponent>();

        foreach (var entity in entitiesWithAnimation)
        {
            var animationComponent = entity.GetComponent<AnimationComponent>();

        }
    }
}