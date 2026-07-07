using System.Linq;
using Engine.Components;
using Engine.ECS;
using Microsoft.Xna.Framework;

/// <summary>
/// Movement system responsible for updating the position of all non-collideable entities that have a
/// Position and Velocity component.
/// </summary>
public class MovementSystem : IUpdateSystem
{
    private readonly EntityManager _entityManager;

    public MovementSystem(EntityManager entityManager)
    {
        _entityManager = entityManager;
    }

    public void Update(GameTime gameTime)
    {
        var entitiesToMove = _entityManager.Entities.Where(entity =>
            entity.HasComponents(typeof(PositionComponent), typeof(VelocityComponent))
            && !entity.HasComponent<BoundingBoxComponent>()
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
