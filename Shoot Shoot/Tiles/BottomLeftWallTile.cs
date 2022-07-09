using System.Drawing;

public class BottomLeftWallTile : WallTile {
    public override Point[] WallPoints => new [] {
        TopRight,
        new Point(TopRight.X - EdgeWidth, TopRight.Y),
        new Point(TopRight.X - EdgeWidth, TopRight.Y + EdgeWidth),
        new Point(TopRight.X, TopRight.Y + EdgeWidth),
        TopRight
    };
}