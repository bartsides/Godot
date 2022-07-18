using System.Drawing;
using System.Drawing.Drawing2D;
using Godot;

public class TriangleShape : IShape {
    public Point Draw(Point topLeft, Size size, Graphics g, Brush brush, RandomNumberGenerator rand) {
        var halfWidth = size.Width/2;
        var halfHeight = size.Height/2;
        var points = new [] {
            new Point(topLeft.X + halfWidth, 0),
            new Point(topLeft.X + size.Width, topLeft.Y + size.Height),
            new Point(topLeft.X, topLeft.Y + size.Height),
            new Point(topLeft.X + halfWidth, 0),
        };

        foreach(var point in points) {
            GD.Print($"{point.X},{point.Y}");
        }

        var types = new [] {
            (byte) PathPointType.Start,
            (byte) PathPointType.Line,
            (byte) PathPointType.Line,
            (byte) PathPointType.Line,
        };
        g.FillPath(brush, new GraphicsPath(points, types));
        return topLeft;
    }
}