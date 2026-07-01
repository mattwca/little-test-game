using Engine.Components;
using Engine.ECS;
using Microsoft.Xna.Framework;

namespace Engine.Systems;

public class AttachedSystem : IUpdateSystem
{
    private readonly EntityManager _entityManager;

    public AttachedSystem(EntityManager entityManager)
    {
        _entityManager = entityManager;
    }

    public void Update(GameTime gameTime)
    {
        var entitiesToUpdate = _entityManager.GetEntitiesWithComponents(
            typeof(AttachedComponent),
            typeof(PositionComponent)
        );

        foreach (var entity in entitiesToUpdate)
        {
            var attachedComponent = entity.GetComponent<AttachedComponent>();
            var trackedEntity = _entityManager.GetEntity(attachedComponent.TrackingEntityId);

            if (trackedEntity is null || !trackedEntity.HasComponent<PositionComponent>())
            {
                return;
            }

            var trackedEntityPosition = trackedEntity.GetComponent<PositionComponent>();
            var attachedEntityPosition = entity.GetComponent<PositionComponent>();
            attachedEntityPosition.Position = trackedEntityPosition.Position + attachedComponent.Offset;

            var attachedEntityRenderer = entity.GetComponentOptional<RenderingComponent>();
            if (attachedEntityRenderer is not null)
            {
                attachedEntityRenderer.DepthHeightOverride = trackedEntityPosition.Position.Y;
            }
        }
    }
}
