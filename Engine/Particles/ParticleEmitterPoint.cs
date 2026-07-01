using Microsoft.Xna.Framework;

namespace Engine.Particles;

public class ParticleEmitterPoint(Vector2 direction) : IParticleEmitterShape
{
    public Vector2 Direction { get; set; } = direction;
}
