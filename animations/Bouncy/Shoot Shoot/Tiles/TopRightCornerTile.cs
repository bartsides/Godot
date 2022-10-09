using System.Drawing;

public class TopRightCornerTile : WallTile {
    public override string TileName => nameof(TopRightCornerTile);
    public override Point[] WallPoints => new [] {
        new Point(BottomLeft.X + EdgeWidth, BottomLeft.Y - WallHeight - EdgeWidth),
        new Point(BottomLeft.X, BottomLeft.Y - WallHeight - EdgeWidth),
        new Point(BottomLeft.X, BottomLeft.Y),
        new Point(BottomLeft.X + EdgeWidth, BottomLeft.Y),
        new Point(BottomLeft.X + EdgeWidth, BottomLeft.Y - WallHeight - EdgeWidth),
    };
}