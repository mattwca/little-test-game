namespace Engine.Particles;

public class ParticleEmitterArc(int minAngle, int maxAngle) : IParticleEmitterShape
{
    public int MinAngle { get; set; } = minAngle;
    public int MaxAngle { get; set; } = maxAngle;
}
