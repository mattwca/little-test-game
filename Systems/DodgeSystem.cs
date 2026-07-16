using System;
using System.Linq;
using Components;
using Engine.Components;
using Engine.ECS;
using Engine.Events;
using Engine.Particles;
using Engine.Physics;
using Events;
using Microsoft.Xna.Framework;

namespace Engine.Systems;

public class DodgeSystem : IUpdateSystem
{
    private readonly EntityManager _entityManager;
    private readonly EventBus _eventBus;

    public DodgeSystem(EntityManager entityManager, EventBus eventBus)
    {
        _entityManager = entityManager;
        _eventBus = eventBus;
    }

    private static string GetDodgeEntityId(string entityId) => $"{entityId}:DodgeEmitter";

    private IParticleEmitterShape GetDodgeParticleShape(Vector2 direction)
    {
        var directionNormal = new Vector2(direction.X, direction.Y);
        directionNormal.Normalize();

        var oppositeDirection = directionNormal * -1;

        var angleDegrees = MathHelper.ToDegrees((float)Math.Atan2(oppositeDirection.X, oppositeDirection.Y));
        var arcMinAngle = angleDegrees - 25f;
        var arcMaxAngle = arcMinAngle + 50f;

        return new ParticleEmitterArc(arcMinAngle, arcMaxAngle);
    }

    private Entity CreateDodgeParticleEmitter(Entity entity, Vector2 direction)
    {
        var entityId = entity.Id;
        var entityPosition = entity.GetComponent<PositionComponent>();
        var emitterShape = GetDodgeParticleShape(direction);

        var dodgeEntity = _entityManager
            .CreateEntity(GetDodgeEntityId(entityId))
            .AddComponent(new PositionComponent(Vector2.Zero))
            .AddComponent(
                new AttachedComponent(entityId, new Vector2(entityPosition.Width / 2f, entityPosition.Height))
            )
            .AddComponent(
                new ParticleEmitterComponent(
                    new ParticleTypeConfig("Particles/playerJump", 8, 8, true),
                    new ParticleSpawnConfig(10, 100, 0.9f, 25f),
                    new ParticleColourConfig(Color.White),
                    emitterShape,
                    ParticleEmitterType.CONTINUOUS
                )
            );

        return dodgeEntity;
    }

    private void DisableDodgeParticleEmitter(Entity entity)
    {
        var dodgeEntity = _entityManager.GetEntity(GetDodgeEntityId(entity.Id));
        var emitterComponent = dodgeEntity!.GetComponent<ParticleEmitterComponent>();

        emitterComponent.Enabled = false;
    }

    private void UpdateAndEnableDodgeParticleEmitter(Entity entity, Vector2 direction)
    {
        var dodgeEntity = _entityManager.GetEntity(GetDodgeEntityId(entity.Id));
        var emitterComponent = dodgeEntity!.GetComponent<ParticleEmitterComponent>();

        emitterComponent.EmitterShape = GetDodgeParticleShape(direction);
        emitterComponent.Enabled = true;
    }

    private void CreateOrUpdateDodgeParticleEmitter(Entity entity, Vector2 direction)
    {
        var dodgeEntityId = GetDodgeEntityId(entity.Id);

        if (_entityManager.HasEntity(dodgeEntityId))
        {
            UpdateAndEnableDodgeParticleEmitter(entity, direction);
            return;
        }

        CreateDodgeParticleEmitter(entity, direction);
    }

    private void ProcessDodgeEvents()
    {
        var dodgeEvents = _eventBus.ReadAll<DodgeEvent>();

        // Start dodges for any entities with a linked dodge event.
        foreach (var dodgeEvent in dodgeEvents)
        {
            var entity = _entityManager.GetEntity(dodgeEvent.EntityId);
            if (entity is null)
            {
                continue;
            }

            var hasComponent = entity.HasComponent<DodgeComponent>();
            if (hasComponent && !entity.GetComponent<DodgeComponent>().CanDodge)
            {
                continue;
            }

            CreateOrUpdateDodgeParticleEmitter(entity, dodgeEvent.Direction);
            entity.ReplaceComponent(new DodgeComponent(dodgeEvent));
        }
    }

    private void TickDodgeComponents(GameTime gameTime)
    {
        var dodgeEntities = _entityManager.GetEntitiesWithComponents(
            typeof(PositionComponent),
            typeof(DodgeComponent),
            typeof(VelocityComponent)
        );

        var collisionEvents = _eventBus.ReadAll<CollisionEvent>();

        foreach (var entity in dodgeEntities)
        {
            var dodgeComponent = entity.GetComponent<DodgeComponent>();
            var velocityComponent = entity.GetComponent<VelocityComponent>();

            // Skip updating inactive dodge components
            if (dodgeComponent.CanDodge)
            {
                continue;
            }

            if (dodgeComponent.CooldownRemainingSecs > 0f && dodgeComponent.TimeRemainingSecs <= 0f)
            {
                dodgeComponent.CooldownRemainingSecs -= gameTime.ElapsedGameTime.TotalSeconds;
                continue;
            }

            dodgeComponent.TimeRemainingSecs -= gameTime.ElapsedGameTime.TotalSeconds;

            // If the dodge timer has finished, or the entity has collided with a physics object, we end the dodge.
            if (dodgeComponent.TimeRemainingSecs < 0f || collisionEvents.Any((e) => e.EntityA == entity.Id))
            {
                dodgeComponent.TimeRemainingSecs = 0f;
                DisableDodgeParticleEmitter(entity);

                dodgeComponent.DodgeEvent.OnFinished?.Invoke();

                continue;
            }

            var delta = dodgeComponent.Direction * 4f;
            // var dodgeValue = Vector2.Lerp(
            //     delta,
            //     Vector2.Zero,
            //     (float)dodgeComponent.TimeRemainingSecs / dodgeComponent.DodgeEvent.TimeSeconds
            // );
            velocityComponent.Velocity += delta;
        }
    }

    public void Update(GameTime gameTime)
    {
        ProcessDodgeEvents();
        TickDodgeComponents(gameTime);
    }
}
