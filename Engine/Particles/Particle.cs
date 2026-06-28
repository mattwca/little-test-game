using Microsoft.Xna.Framework;

namespace Engine.Particles;

public struct Particle
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Rotation { get; set; }
    public float Age { get; set; }
}
