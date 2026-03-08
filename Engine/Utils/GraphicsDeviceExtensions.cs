using System;

using Microsoft.Xna.Framework.Graphics;

namespace Engine.Utils;

public static class GraphicsDeviceExtensions
{
    public static void WithRenderTarget(this GraphicsDevice graphicsDevice, RenderTarget2D renderTarget, Action action)
    {
        graphicsDevice.SetRenderTarget(renderTarget);
        action();
        graphicsDevice.SetRenderTarget(null);
    }
}