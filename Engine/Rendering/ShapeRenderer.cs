using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering;

public class ShapeRenderer
{
    private readonly SpriteBatch _spriteBatch;
    private readonly Texture2D _pixelTexture;

    public ShapeRenderer(ContentManager content, SpriteBatch spriteBatch)
    {
        this._spriteBatch = spriteBatch;
        this._pixelTexture = content.Load<Texture2D>("pixel");
    }

    public void RenderSquare(int x, int y, int width, int height, Color color)
    {
        // Top
        _spriteBatch.Draw(_pixelTexture, new Rectangle(x, y, width, 1), color);

        // Bottom
        _spriteBatch.Draw(_pixelTexture, new Rectangle(x, y + height, width, 1), color);

        // Left
        _spriteBatch.Draw(_pixelTexture, new Rectangle(x, y, 1, height), color);

        // Right
        _spriteBatch.Draw(_pixelTexture, new Rectangle(x + width, y, 1, height), color);
    }

    public void RenderSquare(Rectangle square, Color color)
    {
        this.RenderSquare(square.X, square.Y, square.Width, square.Height, color);
    }
}