using System.Drawing;
using System.Drawing.Drawing2D;

public class BottomLeftWallTile : WallTile {
    public override int ZIndex => 1;

    public override Point[] WallPoints => new [] {
        TopLeft,
        TopRight,
        BottomRight,
        new Point(BottomRight.X - EdgeWidth, BottomRight.Y),
        new Point(BottomRight.X - EdgeWidth, TopRight.Y + EdgeWidth),
        new Point(TopLeft.X, TopLeft.Y + EdgeWidth),
        TopLeft,
    };

    public override byte[] WallPointTypes => new [] {
        (byte) PathPointType.Start, (byte) PathPointType.Line, (byte) PathPointType.Line,
        (byte) PathPointType.Line, (byte) PathPointType.Line, (byte) PathPointType.Line,
        (byte) PathPointType.Line
    };
}