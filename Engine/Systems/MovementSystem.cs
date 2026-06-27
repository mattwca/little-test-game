using Engine.Components;
using Engine.ECS;
using Microsoft.Xna.Framework;

public class MovementSystem : IUpdateSystem
{
    private readonly EntityManager _entityManager;

    public MovementSystem(EntityManager entityManager)
    {
        _entityManager = entityManager;
    }

    public void Update(GameTime gameTime)
    {
        var entitiesToMove = _entityManager.GetEntitiesWithComponents(
            typeof(PositionComponent),
            typeof(VelocityComponent)
        );

        foreach (var entity in entitiesToMove)
        {
            var positionComponent = entity.GetComponent<PositionComponent>();
            var velocityComponent = entity.GetComponent<VelocityComponent>();

            var movementVector = velocityComponent.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 100;
            positionComponent.Position += movementVector;
        }
    }
}
