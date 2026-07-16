using Engine.ECS;

namespace Engine.Components;

public class AnimationComponent : IComponent
{
    public string AnimationKey { get; set; }
    public float FramesPerSecond { get; set; }
    public bool Looping { get; set; }
    public float PlaybackSpeed { get; set; }
    public bool Enabled { get; set; }

    public float FrameTime { get; set; } = 0f;
    public int FrameIndex { get; set; } = 0;

    public AnimationComponent(
        string animationKey,
        float framesPerSecond,
        bool looping = true,
        float playbackSpeed = 1f,
        bool enabled = false
    )
    {
        AnimationKey = animationKey;
        FramesPerSecond = framesPerSecond;
        Looping = looping;
        PlaybackSpeed = playbackSpeed;
        Enabled = enabled;
    }
}
