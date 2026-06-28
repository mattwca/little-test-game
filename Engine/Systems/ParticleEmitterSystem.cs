using System;
using System.Linq;
using Engine.Components;
using Engine.ECS;
using Engine.Particles;
using Microsoft.Xna.Framework;

namespace Engine.Systems;

public class ParticleEmitterSystem : IUpdateSystem
{
    private readonly EntityManager _entityManager;
    private readonly Random _random;

    public ParticleEmitterSystem(EntityManager entityManager)
    {
        _entityManager = entityManager;

        _random = new Random();
    }

    private (Vector2 spawnPosition, Vector2 spawnDirection) GetParticleAttributes(
        ParticleEmitterComponent particleEmitterComponent
    )
    {
        if (particleEmitterComponent.EmitterShape is ParticleEmitterPoint point)
        {
            return (Vector2.Zero, point.Direction);
        }
        else if (particleEmitterComponent.EmitterShape is ParticleEmitterRect rect)
        {
            var spawnX = _random.Next(rect.Rectangle.Left, rect.Rectangle.Right);
            var spawnY = _random.Next(rect.Rectangle.Top, rect.Rectangle.Bottom);

            return (new Vector2(spawnX, spawnY), rect.Direction);
        }
        else if (particleEmitterComponent.EmitterShape is ParticleEmitterCircle circle)
        {
            var spawnAngle = MathHelper.ToRadians(_random.Next(360));
            var spawnRadius = _random.NextSingle() * circle.Radius;

            var spawnDirection = new Vector2(MathF.Sin(spawnAngle), -MathF.Cos(spawnAngle));
            var spawnPosition = spawnDirection * spawnRadius;

            return (spawnPosition, spawnDirection);
        }
        else if (particleEmitterComponent.EmitterShape is ParticleEmitterArc arc)
        {
            var spawnAngle = MathHelper.ToRadians(_random.Next(arc.MinAngle, arc.MaxAngle));
            var spawnDirection = new Vector2(MathF.Sin(spawnAngle), -MathF.Cos(spawnAngle));

            return (Vector2.Zero, spawnDirection);
        }

        throw new Exception("Unknown particle emitter shape");
    }

    private void SpawnParticle(ParticleEmitterComponent particleEmitterComponent)
    {
        var freeParticleIndex = particleEmitterComponent.Particles.ToList().FindIndex(particle => particle.Age <= 0f);
        if (freeParticleIndex == -1)
        {
            return;
        }

        var (spawnPosition, spawnDirection) = GetParticleAttributes(particleEmitterComponent);

        var newParticle = new Particle()
        {
            Position = spawnPosition,
            Age = particleEmitterComponent.MaxAge,
            Velocity = spawnDirection * particleEmitterComponent.Velocity,
        };

        particleEmitterComponent.Particles[freeParticleIndex] = newParticle;
    }

    private void UpdateParticle(ParticleEmitterComponent emitter, int particleIndex, float dt)
    {
        var particle = emitter.Particles[particleIndex];
        if (particle.Age <= 0f)
        {
            return;
        }

        var updatedParticle = new Particle()
        {
            Position = particle.Position + particle.Velocity * dt,
            Age = particle.Age - dt,
            Velocity = particle.Velocity,
        };

        emitter.Particles[particleIndex] = updatedParticle;
    }

    private void UpdateParticleEmitter(Entity particleEmitterEntity, float dt)
    {
        var emitterComponent = particleEmitterEntity.GetComponent<ParticleEmitterComponent>();
        var emitterPosition = particleEmitterEntity.GetComponent<PositionComponent>();

        if (!emitterComponent.Enabled)
        {
            return;
        }

        emitterComponent.Accumulator += emitterComponent.SpawnRate * dt;
        while (emitterComponent.Accumulator >= 1f)
        {
            SpawnParticle(emitterComponent);
            emitterComponent.Accumulator -= 1f;
        }

        for (var i = 0; i < emitterComponent.Particles.Length; i++)
        {
            UpdateParticle(emitterComponent, i, dt);
        }
    }

    public void Update(GameTime gameTime)
    {
        var particleEmitters = _entityManager.GetEntitiesWithComponents(
            typeof(ParticleEmitterComponent),
            typeof(PositionComponent)
        );

        foreach (var emitter in particleEmitters)
        {
            UpdateParticleEmitter(emitter, (float)gameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}
