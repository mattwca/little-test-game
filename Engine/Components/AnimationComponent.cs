using Engine.ECS;

namespace Engine.Components;

public class AnimationComponent : IComponent
{
    public string AnimationKey { get; set; }
    public float FrameTime { get; set; }
    public bool Looping { get; set; }

    public AnimationComponent(string animationKey, float frameTime, bool looping = true)
    {
        AnimationKey = animationKey;
        FrameTime = frameTime;
        Looping = looping;
    }
}