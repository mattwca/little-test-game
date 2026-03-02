using Microsoft.Xna.Framework;

using Engine.ECS;

namespace Engine.Lighting;

public class LightComponent : IComponent
{
    public Color Colour { get; set; }
    public float Intensity { get; set; }

    public LightComponent(Color colour, float intensity)
    {
        Colour = colour;
        Intensity = intensity;
    }

}