using System;
using System.Timers;

namespace Engine.Utils;

public class FPSCounter
{
    private int _frameCount;
    private long _lastTime;

    public int FPS { get; private set; }

    public FPSCounter()
    {
        _frameCount = 0;
        _lastTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        FPS = 0;
    }

    public void OnDraw()
    {
        var currentTimeMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        _frameCount++;

        if (currentTimeMillis - _lastTime > 1000)
        {
            FPS = _frameCount;
            _frameCount = 0;

            _lastTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}
