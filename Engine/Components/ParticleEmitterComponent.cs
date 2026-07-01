using Engine.ECS;
using Engine.Particles;
using Microsoft.Xna.Framework;

namespace Engine.Components;

public enum ParticleEmitterType
{
    CONTINUOUS,
    BURST,
}

public record ParticleRotation(float MinAngle, float MaxAngle, float Speed);

public record ParticleSpawnConfig(
    int ParticleCount,
    float Velocity = 50f,
    float LifespanSeconds = 10f,
    float? SpawnRate = null,
    ParticleRotation? Rotation = null
);

public enum ParticleLightingOption
{
    CastsShadow,
    EmitsLight,
}

public record ParticleLightingConfig(ParticleLightingOption LightingOption, float? LightRadius = null);

public record ParticleTypeConfig(
    string ParticleTexture,
    bool FadeOut = true,
    ParticleLightingConfig? LightingConfig = null
);

public record ParticleColourConfig(Color StartColour, Color? EndColour = null);

public record ParticleRenderConfig(float? OverrideDepthZ = null);

/// <summary>
/// A particle emitter component. Emits a continuous stream of particles, or a single burst.
///
/// Various properties of the spawned particles can be configured, including lifetime, speed,
/// number, colour(s), and how they interact with the lighting system.
///
/// When a "burst" particle emitter has finished, the associated entity is deleted.
/// </summary>
public class ParticleEmitterComponent(
    ParticleTypeConfig particleType,
    ParticleSpawnConfig spawnConfig,
    ParticleColourConfig colourConfig,
    IParticleEmitterShape emitterShape,
    ParticleEmitterType emitterType = ParticleEmitterType.CONTINUOUS,
    ParticleRenderConfig? renderConfig = null,
    bool enabled = true
) : IComponent
{
    public ParticleTypeConfig ParticleType { get; set; } = particleType;
    public ParticleSpawnConfig SpawnConfig { get; set; } = spawnConfig;
    public ParticleColourConfig ColourConfig { get; set; } = colourConfig;
    public IParticleEmitterShape EmitterShape { get; set; } = emitterShape;
    public ParticleEmitterType EmitterType { get; set; } = emitterType;
    public ParticleRenderConfig RenderConfig { get; set; } = renderConfig;
    public Particle[] Particles { get; } = new Particle[spawnConfig.ParticleCount];
    public bool Enabled { get; set; } = enabled;

    /// <summary>
    /// (Internal state) Spawn accumulator.
    /// </summary>
    public float Accumulator { get; set; } = 0f;

    /// <summary>
    /// (Internal state) Flag indicating whether a burst emitter has fired.
    /// </summary>
    public bool HasFired { get; set; } = false;
}
