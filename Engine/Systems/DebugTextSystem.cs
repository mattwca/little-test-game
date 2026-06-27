using System.Collections.Generic;
using Engine.ECS;
using Engine.Rendering;
using Engine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Systems;

public interface IDebugText
{
    public Point Position { get; set; }
    public string Text { get; set; }
}

public class DebugTextSystem : IRenderSystem, IUpdateSystem
{
    private readonly StateManager _stateManager;
    private readonly TextRenderer _textRenderer;
    private readonly List<IDebugText> _textToRender;

    private readonly FPSCounter _fpsCounter;

    public DebugTextSystem(StateManager stateManager, TextRenderer textRenderer, FPSCounter fpsCounter)
    {
        _stateManager = stateManager;
        _textRenderer = textRenderer;
        _fpsCounter = fpsCounter;
    }

    public void Update(GameTime gameTime) { }

    public void Draw(GameTime gameTime)
    {
        _fpsCounter.OnDraw();

        if (!_stateManager.GetBool("debugModeEnabled"))
        {
            return;
        }

        _textRenderer.RenderString($"FPS: {_fpsCounter.FPS.ToString()}", new Vector2(10, 10));
    }
}
