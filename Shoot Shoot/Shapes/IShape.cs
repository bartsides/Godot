using System.Drawing;
using Godot;

public interface IShape {
    // Returns next center point
    Point Draw(Point topLeft, Size size, Graphics g, Brush brush, RandomNumberGenerator rand);
}