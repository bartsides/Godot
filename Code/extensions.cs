using Godot;
using System;
using System.Drawing;

public static class extensions {
    public static bool IsZero(this Vector2 vector) => vector.X.IsZero() && vector.Y.IsZero();
    
    public static bool IsZero(this float value) => Mathf.Abs(value - 0) <= 0.0000001;

    public static bool Between(this int value, int min, int max) => value >= min && value <= max;

    public static direction Next(this direction neighbor) => (direction)(((int)neighbor + 1) % 8);
    public static direction Prev(this direction neighbor) {
        var i = (int)neighbor - 1;
        if (i < 0)
            i = 7;
        return (direction)(i % 8);
    }
    public static direction Opposite(this direction neighbor) => (direction)(((int)neighbor + 4) % 8);

    public static Point AddY(this Point point, int y) => new Point(point.X, point.Y + y);
    public static Vector2 ToVector2(this Point point) => new Vector2(point.X, point.Y);

    public static Point GetNeighbor(this Point point, direction neighbor)
    {
        var x = 0;
        var y = 0;
        switch (neighbor)
        {
            case direction.UpLeft:
                x = -1;
                y = -1;
                break;
            case direction.Up:
                y = -1;
                break;
            case direction.UpRight:
                x = 1;
                y = -1;
                break;
            case direction.Right:
                x = 1;
                break;
            case direction.DownRight:
                x = 1;
                y = 1;
                break;
            case direction.Down:
                y = 1;
                break;
            case direction.DownLeft:
                x = -1;
                y = 1;
                break;
            case direction.Left:
                x = -1;
                break;
        }

        return new Point(point.X + x, point.Y + y);
    }

    public static direction DetermineNeighbor(this Point point, Point neighbor)
    {
        var x = neighbor.X - point.X;
        var y = neighbor.Y - point.Y;

        if (x == -1)
        {
            if (y == -1)
                return direction.UpLeft;
            if (y == 0)
                return direction.Left;
            if (y == 1)
                return direction.DownLeft;
        }
        else if (x == 0)
        {
            if (y == -1)
                return direction.Up;
            if (y == 1)
                return direction.Down;
        }
        else if (x == 1)
        {
            if (y == -1)
                return direction.UpRight;
            if (y == 0)
                return direction.Right;
            if (y == 1)
                return direction.DownRight;
        }

        throw new Exception($"Unable to determine neighbor: ({point.X},{point.Y}) => ({neighbor.X},{neighbor.Y})");
    }

    public static direction GetDirection(this Vector2 velocity) {
        var movementAngle = Mathf.RadToDeg(velocity.Angle());
		if (movementAngle >= -45 && movementAngle <= 45) {
			return direction.Right;
        }
        else if (movementAngle >= -180 && movementAngle <= -135 ||
                 movementAngle >= 135 && movementAngle <= 180) {
            return direction.Left;
        }
		else if (movementAngle > 45 && movementAngle < 135) { 
			return direction.Down;
        }
		
        // -45 -135
        return direction.Up;
    }

    public static bool IsFacingRight(this Vector2 dir) => IsAngleBetween(dir, -90, 90);

    private static bool IsAngleBetween(Vector2 direction, float min, float max) {
        var angle = Mathf.RadToDeg(direction.Angle());
        return angle >= min && angle <= max;
    }

    public static T Duplicate<T>(this Resource resource) where T : Resource {
        return (T) resource.Duplicate();
    }

    public static T Duplicate<T>(this Node node) where T : Node {
        return (T) node.Duplicate();
    }
}