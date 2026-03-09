using Engine.ECS;

namespace Engine.Components;

public class VisibilityComponent(bool isVisible = false) : IComponent
{
    public bool IsVisible { get; set; } = isVisible;
}