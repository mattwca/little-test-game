using Microsoft.Xna.Framework;

namespace Engine.Utils;

public class LinearColourGenerator(Color[] colours, float speed = 0.5f)
{
    public static Color[] DefaultColours = [
        Color.Red,
        Color.Orange,
        Color.Yellow,
        Color.Green,
        Color.Blue,
        Color.Purple,
    ];

    private readonly Color[] _colours = colours;
    private readonly float _speed = speed;
    private float _t = 0f;

    public Color GetCyclingColor(float deltaTime)
    {
        _t = (_t + deltaTime * _speed) % _colours.Length;

        int fromIndex = (int)_t;
        int toIndex = (fromIndex + 1) % _colours.Length;
        float blend = _t - fromIndex;

        return Color.Lerp(_colours[fromIndex], _colours[toIndex], blend);
    }
}