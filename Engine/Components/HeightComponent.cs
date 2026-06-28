using Engine.ECS;

namespace Engine.Components;

public class HeightComponent : IComponent
{
    public float Z { get; set; }
    public float ZVelocity { get; set; }
    public bool Grounded => Z <= 0f;
    public bool WasGrounded { get; set; } = true;
}
