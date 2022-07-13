using Godot;
using System;
using System.Drawing;

public static class Extensions {
    public static Point AddY(this Point point, int y) => new Point(point.X, point.Y + y);
    public static Vector2 ToVector2(this Point point) => new Vector2(point.X, point.Y);
    public static bool IsZero(this Vector2 vector) => vector.x.IsZero() && vector.y.IsZero();
    public static bool IsZero(this float value) => Mathf.Abs(value - 0) <= 0.0000001;
    public static bool Between(this int value, int min, int max) => value >= min && value <= max;
    public static MooreNeighbor Next(this MooreNeighbor neighbor) => (MooreNeighbor)(((int)neighbor + 1) % 8);

    public static Point GetNeighbor(this Point point, MooreNeighbor neighbor)
    {
        var x = 0;
        var y = 0;
        switch (neighbor)
        {
            case MooreNeighbor.UpLeft:
                x = -1;
                y = -1;
                break;
            case MooreNeighbor.Up:
                y = -1;
                break;
            case MooreNeighbor.UpRight:
                x = 1;
                y = -1;
                break;
            case MooreNeighbor.Right:
                x = 1;
                break;
            case MooreNeighbor.DownRight:
                x = 1;
                y = 1;
                break;
            case MooreNeighbor.Down:
                y = 1;
                break;
            case MooreNeighbor.DownLeft:
                x = -1;
                y = 1;
                break;
            case MooreNeighbor.Left:
                x = -1;
                break;
        }

        return new Point(point.X + x, point.Y + y);
    }

    public static MooreNeighbor DetermineNeighbor(this Point point, Point neighbor)
    {
        var x = neighbor.X - point.X;
        var y = neighbor.Y - point.Y;

        if (x == -1)
        {
            if (y == -1)
                return MooreNeighbor.UpLeft;
            if (y == 0)
                return MooreNeighbor.Left;
            if (y == 1)
                return MooreNeighbor.DownLeft;
        }
        else if (x == 0)
        {
            if (y == -1)
                return MooreNeighbor.Up;
            if (y == 1)
                return MooreNeighbor.Down;
        }
        else if (x == 1)
        {
            if (y == -1)
                return MooreNeighbor.UpRight;
            if (y == 0)
                return MooreNeighbor.Right;
            if (y == 1)
                return MooreNeighbor.DownRight;
        }

        throw new Exception($"Unable to determine neighbor: ({point.X},{point.Y}) => ({neighbor.X},{neighbor.Y}");
    }
}