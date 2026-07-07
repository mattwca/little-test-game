using Engine.ECS;
using Microsoft.Xna.Framework;

namespace Engine.Lighting;

public class LightComponent(
    Color? colour = null,
    float intensity = 1f,
    float radius = 800f,
    float attenuationConstant = 1f,
    float attenuationLinear = 0f,
    float attenuationQuadratic = 25f,
    float windowExponent = 4f
) : IComponent
{
    public Color Colour { get; set; } = colour ?? Color.White;
    public float Intensity { get; set; } = intensity;
    public float Radius { get; set; } = radius;

    public float AttenuationConstant { get; set; } = attenuationConstant;
    public float AttenuationLinear { get; set; } = attenuationLinear;
    public float AttenuationQuadratic { get; set; } = attenuationQuadratic;
    public float WindowExponent { get; set; } = windowExponent;
}
