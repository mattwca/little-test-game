using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Engine.Physics;

public struct RectangleF(float x, float y, float width, float height)
{
    public float X { get; set; } = x;
    public float Y { get; set; } = y;
    public float Width { get; set; } = width;
    public float Height { get; set; } = height;

    public readonly float Left => X;
    public readonly float Right => X + Width;
    public readonly float Top => Y;
    public readonly float Bottom => Y + Height;

    public readonly Vector2 Centre => new(X + Width / 2f, Y + Height / 2f);

    public readonly bool Intersects(RectangleF rectangle)
    {
        if (rectangle.Left < Right && Left < rectangle.Right && rectangle.Top < Bottom)
        {
            return Top < rectangle.Bottom;
        }

        return false;
    }

    public static RectangleF Intersect(RectangleF rect1, RectangleF rect2)
    {
        if (rect1.Intersects(rect2))
        {
            float intersectMinX = Math.Max(rect1.X, rect2.X);
            float intersectMinY = Math.Max(rect1.Y, rect2.Y);
            float intersectMaxX = Math.Min(rect1.X + rect1.Width, rect2.X + rect2.Width);
            float intersectMaxY = Math.Min(rect1.Y + rect1.Height, rect2.Y + rect2.Height);

            var result = new RectangleF(
                intersectMinX,
                intersectMinY,
                intersectMaxX - intersectMinX,
                intersectMaxY - intersectMinY
            );
            return result;
        }

        return new RectangleF(0f, 0f, 0f, 0f);
    }

    public readonly Rectangle ToRectangle()
    {
        return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
    }
}