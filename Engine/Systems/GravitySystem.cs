using Engine.Components;
using Engine.ECS;
using Microsoft.Xna.Framework;

namespace Engine.Systems;

public class GravitySystem : IUpdateSystem
{
    private readonly EntityManager _entityManager;

    private const float GRAVITY = 1500f;

    public GravitySystem(EntityManager entityManager)
    {
        _entityManager = entityManager;
    }

    public void Update(GameTime gameTime)
    {
        var entitiesToUpdate = _entityManager.GetEntitiesWithComponents(
            typeof(HeightComponent),
            typeof(PositionComponent)
        );

        foreach (var entity in entitiesToUpdate)
        {
            var heightComponent = entity.GetComponent<HeightComponent>();
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            heightComponent.ZVelocity -= GRAVITY * dt;
            heightComponent.Z += heightComponent.ZVelocity * dt;

            if (heightComponent.Z <= 0f)
            {
                heightComponent.Z = 0f;
                heightComponent.ZVelocity = 0f;
            }
        }
    }
}
