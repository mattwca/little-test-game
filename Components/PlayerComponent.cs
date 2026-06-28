using Engine.ECS;
using Microsoft.Xna.Framework;

namespace Components;

public class PlayerComponent : IComponent
{
    public Vector2 Direction { get; set; }
}
