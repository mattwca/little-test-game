using Engine.ECS;
using Microsoft.Xna.Framework;

namespace Engine.Components;

public class AttachedComponent(string trackingEntityId, Vector2 offset) : IComponent
{
    public string TrackingEntityId { get; set; } = trackingEntityId;
    public Vector2 Offset { get; set; } = offset;
}
