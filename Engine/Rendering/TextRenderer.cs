using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering;

public class TextRenderer
{
    private readonly ContentManager _contentManager;
    private readonly SpriteBatch _spriteBatch;
    private readonly Texture2D _textTexture;

    private readonly Dictionary<char, Point> _charMap = new()
    {
        { 'A', new Point(0, 0) },
        { 'B', new Point(1, 0) },
        { 'C', new Point(2, 0) },
        { 'D', new Point(3, 0) },
        { 'E', new Point(4, 0) },
        { 'F', new Point(5, 0) },
        { 'G', new Point(6, 0) },
        { 'H', new Point(7, 0) },
        { 'I', new Point(8, 0) },
        { 'J', new Point(9, 0) },
        { 'K', new Point(10, 0) },
        { 'L', new Point(11, 0) },
        { 'M', new Point(12, 0) },
        { 'N', new Point(13, 0) },
        { 'O', new Point(14, 0) },
        { 'P', new Point(15, 0) },
        { 'Q', new Point(16, 0) },
        { 'R', new Point(17, 0) },
        { 'S', new Point(18, 0) },
        { 'T', new Point(19, 0) },
        { 'U', new Point(0, 1) },
        { 'V', new Point(1, 1) },
        { 'W', new Point(2, 1) },
        { 'X', new Point(3, 1) },
        { 'Y', new Point(4, 1) },
        { 'Z', new Point(5, 1) },
        { '!', new Point(0, 3) },
        { '?', new Point(1, 3) },
        { ':', new Point(2, 3) },
        { ';', new Point(3, 3) },
        { ',', new Point(4, 3) },
        { '.', new Point(5, 3) },
        { '-', new Point(6, 3) },
        { '_', new Point(7, 3) },
        { '1', new Point(0, 5) },
        { '2', new Point(1, 5) },
        { '3', new Point(2, 5) },
        { '4', new Point(3, 5) },
        { '5', new Point(4, 5) },
        { '6', new Point(5, 5) },
        { '7', new Point(6, 5) },
        { '8', new Point(7, 5) },
        { '9', new Point(8, 5) },
        { '0', new Point(9, 5) },
    };

    private const int CHAR_SIZE = 8;
    private const int CHAR_SPACING = 2;

    public TextRenderer(ContentManager contentManager, SpriteBatch spriteBatch)
    {
        _contentManager = contentManager;
        _spriteBatch = spriteBatch;

        _textTexture = _contentManager.Load<Texture2D>("font");
    }

    public void RenderString(string text, Vector2 position, int scale = 1)
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        var advance = (CHAR_SIZE * scale) + CHAR_SPACING;

        for (var i = 0; i < text.Length; i++)
        {
            var character = text.ToUpper()[i];
            if (_charMap.TryGetValue(character, out var charMapEntry))
            {
                var sourceRect = new Rectangle(
                    new Point(charMapEntry.X * CHAR_SIZE, charMapEntry.Y * CHAR_SIZE),
                    new Point(8, 8)
                );
                var destinationRect = new Rectangle(
                    new Point((int)position.X + i * advance, (int)position.Y),
                    new Point(CHAR_SIZE * scale, CHAR_SIZE * scale)
                );

                _spriteBatch.Draw(_textTexture, destinationRect, sourceRect, Color.White);
            }
        }

        _spriteBatch.End();
    }
}
