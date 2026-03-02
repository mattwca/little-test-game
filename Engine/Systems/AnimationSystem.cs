using Engine.Components;
using Engine.ECS;

namespace Engine.Systems;

public class AnimationSystem : IUpdateSystem
{
    private readonly EntityManager _entityManager;

    public AnimationSystem(EntityManager entityManager)
    {
        _entityManager = entityManager;
    }

    public void Update(float deltaTime)
    {
        var entitiesWithAnimation = _entityManager.GetEntitiesWithComponent<AnimationComponent>();
        
        foreach (var entity in entitiesWithAnimation)
        {
            var animationComponent = entity.GetComponent<AnimationComponent>();

        }
    }
}