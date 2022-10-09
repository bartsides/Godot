using Godot;
using System;
using System.Drawing;

public static class Extensions {
    public static Point AddY(this Point point, int y) => new Point(point.X, point.Y + y);
    public static Vector2 ToVector2(this Point point) => new Vector2(point.X, point.Y);
    public static bool IsZero(this Vector2 vector) => vector.x.IsZero() && vector.y.IsZero();
    public static bool IsZero(this float value) => Mathf.Abs(value - 0) <= 0.0000001;
    public static bool Between(this int value, int min, int max) => value >= min && value <= max;
    public static Direction Next(this Direction neighbor) => (Direction)(((int)neighbor + 1) % 8);
    public static Direction Opposite(this Direction neighbor) => (Direction)(((int)neighbor + 4) % 8);

    public static Point GetNeighbor(this Point point, Direction neighbor)
    {
        var x = 0;
        var y = 0;
        switch (neighbor)
        {
            case Direction.UpLeft:
                x = -1;
                y = -1;
                break;
            case Direction.Up:
                y = -1;
                break;
            case Direction.UpRight:
                x = 1;
                y = -1;
                break;
            case Direction.Right:
                x = 1;
                break;
            case Direction.DownRight:
                x = 1;
                y = 1;
                break;
            case Direction.Down:
                y = 1;
                break;
            case Direction.DownLeft:
                x = -1;
                y = 1;
                break;
            case Direction.Left:
                x = -1;
                break;
        }

        return new Point(point.X + x, point.Y + y);
    }

    public static Direction DetermineNeighbor(this Point point, Point neighbor)
    {
        var x = neighbor.X - point.X;
        var y = neighbor.Y - point.Y;

        if (x == -1)
        {
            if (y == -1)
                return Direction.UpLeft;
            if (y == 0)
                return Direction.Left;
            if (y == 1)
                return Direction.DownLeft;
        }
        else if (x == 0)
        {
            if (y == -1)
                return Direction.Up;
            if (y == 1)
                return Direction.Down;
        }
        else if (x == 1)
        {
            if (y == -1)
                return Direction.UpRight;
            if (y == 0)
                return Direction.Right;
            if (y == 1)
                return Direction.DownRight;
        }

        throw new Exception($"Unable to determine neighbor: ({point.X},{point.Y}) => ({neighbor.X},{neighbor.Y}");
    }
}