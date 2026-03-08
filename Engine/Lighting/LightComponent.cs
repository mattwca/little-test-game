using Microsoft.Xna.Framework;

using Engine.ECS;

namespace Engine.Lighting;

public class LightComponent : IComponent
{
    public Color Colour { get; set; }
    public float Intensity { get; set; }

    public LightComponent(Color? colour = null, float intensity = 1f)
    {
        Colour = colour ?? Color.White;
        Intensity = intensity;
    }

}