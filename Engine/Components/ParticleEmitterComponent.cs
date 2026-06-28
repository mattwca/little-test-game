using Engine.ECS;
using Engine.Particles;
using Microsoft.Xna.Framework;

namespace Engine.Components;

public enum ParticleEmitterType
{
    CONTINUOUS,
    BURST,
}

public class ParticleEmitterComponent(
    string particleTexture,
    float spawnRate,
    float maxAge,
    IParticleEmitterShape emitterShape,
    float velocity,
    bool enabled = true,
    int particleCount = 100,
    bool fadeOut = true,
    bool castsShadows = true,
    int fixedParticleCount = 0,
    ParticleEmitterType emitterType = ParticleEmitterType.CONTINUOUS
) : IComponent
{
    public string ParticleTexture { get; set; } = particleTexture;
    public float SpawnRate { get; set; } = spawnRate;

    /// <summary>
    /// Max age (in seconds) for each particle.
    /// </summary>
    public float MaxAge { get; set; } = maxAge;
    public IParticleEmitterShape EmitterShape { get; set; } = emitterShape;
    public float Velocity { get; set; } = velocity;
    public bool Enabled { get; set; } = enabled;
    public Particle[] Particles { get; } = new Particle[particleCount];
    public bool FadeOut { get; set; } = fadeOut;
    public bool CastsShadows { get; set; } = castsShadows;
    public int FixedParticleCount { get; set; } = fixedParticleCount;
    public ParticleEmitterType EmitterType { get; set; } = emitterType;

    public float Accumulator { get; set; } = 0f;
    public bool HasFired { get; set; } = false;
}
