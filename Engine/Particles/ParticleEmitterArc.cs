namespace Engine.Particles;

public class ParticleEmitterArc(float minAngle, float maxAngle) : IParticleEmitterShape
{
    public float MinAngle { get; set; } = minAngle;
    public float MaxAngle { get; set; } = maxAngle;
}
