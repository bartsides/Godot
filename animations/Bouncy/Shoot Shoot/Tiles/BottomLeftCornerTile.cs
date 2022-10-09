using System.Drawing;

public class BottomLeftCornerTile : WallTile {
    public override string TileName => nameof(BottomLeftCornerTile);
    public override int ZIndex => 1;

    public override Point[] WallPoints => new [] {
        TopRight,
        new Point(TopRight.X - EdgeWidth, TopRight.Y),
        new Point(TopRight.X - EdgeWidth, TopRight.Y + EdgeWidth),
        new Point(TopRight.X, TopRight.Y + EdgeWidth),
        TopRight
    };
}