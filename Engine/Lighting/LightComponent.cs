using Engine.ECS;
using Microsoft.Xna.Framework;

namespace Engine.Lighting;

public class LightComponent(Color? colour = null, float intensity = 1f, float radius = 800f) : IComponent
{
    public Color Colour { get; set; } = colour ?? Color.White;
    public float Intensity { get; set; } = intensity;
    public float Radius { get; set; } = radius;
}
