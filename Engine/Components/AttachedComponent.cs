using Engine.ECS;
using Microsoft.Xna.Framework;

namespace Engine.Components;

public record AttachedComponent(string TrackingEntityId, Vector2 Offset, bool CopyFlip = false) : IComponent;
