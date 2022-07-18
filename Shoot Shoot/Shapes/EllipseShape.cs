using System.Drawing;
using Godot;

public class EllipseShape : IShape {
    public Point Draw(Point topLeft, Size size, Graphics g, Brush brush, RandomNumberGenerator rand) {
        var newSize = new Size(size.Width, size.Height);
        var newCenter = new Point(topLeft.X, topLeft.Y - size.Height/2);
        g.FillEllipse(brush, new Rectangle(newCenter, size));
        return topLeft;
    }
}