using System.Drawing;

public class TopLeftCornerTile : WallTile {
    public override string TileName => nameof(TopLeftCornerTile);
    public override Point[] WallPoints => new [] {
        new Point(BottomRight.X - EdgeWidth, BottomRight.Y - WallHeight - EdgeWidth),
        new Point(BottomRight.X, BottomRight.Y - WallHeight - EdgeWidth),
        new Point(BottomRight.X, BottomRight.Y),
        new Point(BottomRight.X - EdgeWidth, BottomRight.Y),
        new Point(BottomRight.X - EdgeWidth, BottomRight.Y - WallHeight - EdgeWidth),
    };
}