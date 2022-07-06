using Godot;
using System.Drawing;

public static class Extensions {
    public static Vector2 ToVector2(this Point point) => new Vector2(point.X, point.Y);
    public static bool IsZero(this Vector2 vector) => vector.x.IsZero() && vector.y.IsZero();
    public static bool IsZero(this float value) => Mathf.Abs(value - 0) <= 0.0000001;
}