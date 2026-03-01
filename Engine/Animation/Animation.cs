namespace Engine.Animation;

public class Animation
{
    public string AnimationKey { get; }
    public int FrameCount { get; }
    public int FrameWidth { get; }
    public int FrameHeight { get; }

    // TODO: actual steps

    public Animation(string animationKey, int frameCount, int frameWidth, int frameHeight)
    {
        AnimationKey = animationKey;
        FrameCount = frameCount;
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;
    }
}