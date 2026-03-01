using Microsoft.Xna.Framework;

namespace Engine.Lighting;

public class LightComponent
{
    public Color Colour { get; set; }
    public float Intensity { get; set; }

    public LightComponent(Color colour, float intensity)
    {
        Colour = colour;
        Intensity = intensity;
    }
}