using Microsoft.Xna.Framework.Input;

namespace Engine.Utils;

public class KeyboardHandler
{
    private KeyboardState _previousKeyboardState;
    private KeyboardState _currentKeyboardState;

    public void OnUpdate()
    {
        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();
    }

    public bool WasKeyPressed(Keys key)
    {
        var wasPressed = _previousKeyboardState.IsKeyDown(key);
        var isPressed = _currentKeyboardState.IsKeyDown(key);
        return !wasPressed && isPressed;
    }

    public bool WasKeyReleased(Keys key)
    {
        var wasPressed = _previousKeyboardState.IsKeyDown(key);
        var isReleased = _previousKeyboardState.IsKeyUp(key);
        return wasPressed && isReleased;
    }
}
