using System.Collections.Generic;
using Engine.ECS;
using Microsoft.Xna.Framework;

namespace Engine.Components;

public struct Particle
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Rotation { get; set; }
    public float Age { get; set; }
    public bool Visible { get; set; }
}

public class ParticleEmitterComponent(
    string particleTexture,
    float spawnRate,
    float maxAge,
    Vector2 direction,
    bool enabled = true,
    int particleCount = 100,
    bool castsShadows = true
) : IComponent
{
    public string ParticleTexture { get; set; } = particleTexture;
    public float SpawnRate { get; set; } = spawnRate;
    public float MaxAge { get; set; } = maxAge;
    public Vector2 Direction { get; set; } = direction;
    public bool Enabled { get; set; } = enabled;
    public Particle[] Particles { get; } = new Particle[particleCount];
    public float Accumulator { get; set; } = 0f;
}
