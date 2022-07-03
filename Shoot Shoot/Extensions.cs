using Godot;
using System.Drawing;

public static class Extensions {
    public static Vector2 ToVector2(this Point point) => new Vector2(point.X, point.Y);
}