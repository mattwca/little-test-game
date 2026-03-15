using System.Collections.Generic;

namespace Engine.Animation;

public class AnimationManager
{
    private readonly Dictionary<string, Animation> _animations = new Dictionary<string, Animation>();

    public void AddAnimation(Animation animation)
    {
        _animations[animation.AnimationKey] = animation;
    }

    public Animation? GetAnimation(string key)
    {
        return _animations.TryGetValue(key, out var animation) ? animation : null;
    }
}