using System.Drawing;
using System.Drawing.Drawing2D;

public class BottomRightWallTile : WallTile {
    public override int ZIndex => 1;

    public override Point[] WallPoints => new [] {
        TopLeft,
        TopRight,
        new Point(TopRight.X, TopRight.Y + EdgeWidth),
        new Point(TopLeft.X + EdgeWidth, TopLeft.Y + EdgeWidth),
        new Point(BottomLeft.X + EdgeWidth, BottomLeft.Y),
        BottomLeft,
        TopLeft
    };

    public override byte[] WallPointTypes => new [] {
        (byte) PathPointType.Start, (byte) PathPointType.Line, (byte) PathPointType.Line,
        (byte) PathPointType.Line, (byte) PathPointType.Line, (byte) PathPointType.Line,
        (byte) PathPointType.Line
    };
}