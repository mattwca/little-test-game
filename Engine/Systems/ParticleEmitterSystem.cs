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
    private readonly EntityCleanupSystem _cleanupSystem;

    private readonly Random _random;

    public ParticleEmitterSystem(EntityManager entityManager, EntityCleanupSystem cleanupSystem)
    {
        _entityManager = entityManager;
        _cleanupSystem = cleanupSystem;

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
            Age = particleEmitterComponent.SpawnConfig.LifespanSeconds,
            Velocity = spawnDirection * particleEmitterComponent.SpawnConfig.Velocity,
            Colour = particleEmitterComponent.ColourConfig.StartColour,
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

        // var rotation = 0f;
        // if (emitter.RotationBounds is not null) { }

        var updatedColour = emitter.ColourConfig.StartColour;
        if (emitter.ColourConfig.EndColour is not null)
        {
            updatedColour = Color.Lerp(
                emitter.ColourConfig.StartColour,
                emitter.ColourConfig.EndColour.Value,
                1 - (particle.Age / emitter.SpawnConfig.LifespanSeconds)
            );
        }

        var updatedParticle = new Particle()
        {
            Position = particle.Position + particle.Velocity * dt,
            Age = particle.Age - dt,
            Velocity = particle.Velocity,
            Colour = updatedColour,
        };

        emitter.Particles[particleIndex] = updatedParticle;
    }

    private void UpdateParticleEmitter(Entity particleEmitterEntity, float dt)
    {
        var emitterComponent = particleEmitterEntity.GetComponent<ParticleEmitterComponent>();
        var emitterPosition = particleEmitterEntity.GetComponent<PositionComponent>();

        if (!emitterComponent.Enabled)
        {
            emitterComponent.HasFired = false;
            return;
        }

        if (emitterComponent.EmitterType == ParticleEmitterType.CONTINUOUS)
        {
            if (emitterComponent.SpawnConfig.SpawnRate is null)
            {
                throw new Exception("Continuous emitter must have a spawn rate");
            }

            emitterComponent.Accumulator += emitterComponent.SpawnConfig.SpawnRate.Value * dt;
            while (emitterComponent.Accumulator >= 1f)
            {
                SpawnParticle(emitterComponent);
                emitterComponent.Accumulator -= 1f;
            }
        }
        else if (emitterComponent.EmitterType == ParticleEmitterType.BURST)
        {
            if (!emitterComponent.HasFired)
            {
                for (var i = 0; i < emitterComponent.Particles.Length; i++)
                {
                    SpawnParticle(emitterComponent);
                }

                emitterComponent.HasFired = true;
            }
            else
            {
                // Check whether all particles have died - if they have, remove the emitter.
                if (emitterComponent.Particles.All((particle) => particle.Age <= 0))
                {
                    _cleanupSystem.MarkForCleanup(particleEmitterEntity.Id);
                    return;
                }
            }
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
