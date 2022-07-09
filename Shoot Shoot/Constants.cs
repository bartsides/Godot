using System.Drawing;
using System.Drawing.Drawing2D;

using DrawingColor = System.Drawing.Color;

public static class Constants {
    public static byte[] RectPointTypes => new[] {
            (byte) PathPointType.Start, (byte) PathPointType.Line, (byte) PathPointType.Line,
            (byte) PathPointType.Line, (byte) PathPointType.Line
        };
    
    public static DrawingColor VoidColor = DrawingColor.FromArgb(255, 37, 37, 37);
    public static SolidBrush VoidBrush = new SolidBrush(VoidColor);
}