using Engine.Particles;
using Microsoft.Xna.Framework;

public class ParticleEmitterRect : IParticleEmitterShape
{
    public Rectangle Rectangle { get; set; }
    public Vector2 Direction { get; set; }
}
