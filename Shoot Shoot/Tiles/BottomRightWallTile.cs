using System.Drawing;

public class BottomRightWallTile : WallTile {
    public override Point[] WallPoints => new [] {
        TopLeft,
        new Point(TopLeft.X + EdgeWidth, TopLeft.Y),
        new Point(TopLeft.X + EdgeWidth, TopLeft.Y + EdgeWidth),
        new Point(TopLeft.X, TopLeft.Y + EdgeWidth),
        TopLeft
    };
}