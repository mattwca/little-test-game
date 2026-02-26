using Engine.ECS;
using Microsoft.Xna.Framework;

namespace Engine.Components;

public class CameraComponent : IComponent
{
    public Vector2 Position { get; set; }
    public float Zoom { get; set; }
    public Matrix Transform => Matrix.CreateTranslation(new Vector3(-Position, 0)) * Matrix.CreateScale(Zoom);

    public CameraComponent(Vector2 position, float zoom)
    {
        Position = position;
        Zoom = zoom;
    }
}