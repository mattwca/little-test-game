using Engine.ECS;

namespace Engine.Components;

public class VisibilityComponent(bool isVisible = false, float offset = 0f) : IComponent
{
    /// <summary>
    /// Indicates whether the entity this component is attached to is visible or not.
    /// </summary>
    public bool IsVisible { get; set; } = isVisible;

    /// <summary>
    /// The offset to use when checking visibility. If the distance from the viewport edge
    /// to the entity is less than the offset, it's considered still visible.
    /// </summary>
    public float Offset { get; set; } = offset;
}