using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Godot;

public class RectangleShape : IShape {
    public Point Draw(Point topLeft, Size size, Graphics g, Brush brush, RandomNumberGenerator rand) {
        var rect = new Rectangle(topLeft, size);
        g.FillRectangle(brush, rect);
        
        // Find next point
        var points = new List<Point>();

        var bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
        using (var graphics = Graphics.FromImage(bitmap)) {
            graphics.DrawRectangle(Pens.Black, new Rectangle(new Point(), size));
        }

        for (var x = 0; x < size.Width; x++) {
            for (var y = 0; y < size.Height; y++) {
                if (bitmap.GetPixel(x, y).A > 0) {
                    points.Add(new Point(x, y));
                }
            }
        }

        var nextPoint = points[rand.RandiRange(0, points.Count - 1)];
        return new Point(topLeft.X + nextPoint.X, topLeft.Y + nextPoint.Y);
    }
}