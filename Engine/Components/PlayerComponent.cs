using Engine.ECS;
using Microsoft.Xna.Framework;

namespace Engine.Components;

public class PlayerComponent : IComponent
{
    public Vector2 Direction { get; set; }
}
