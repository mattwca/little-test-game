using Engine.ECS;

using Microsoft.Xna.Framework;

namespace Engine.Components;

public class AnimationComponent : IComponent
{
    public string AnimationKey { get; set; }
    public float FrameTime { get; set; }
    public bool Looping { get; set; }
    public bool Enabled { get; set; }

    public AnimationComponent(string animationKey, float frameTime, bool looping = true, bool enabled = false)
    {
        AnimationKey = animationKey;
        FrameTime = frameTime;
        Looping = looping;
        Enabled = enabled;
    }
}