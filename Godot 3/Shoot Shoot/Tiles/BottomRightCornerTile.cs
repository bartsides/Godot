using System.Drawing;

public class BottomRightCornerTile : WallTile {
    public override string TileName => nameof(BottomRightCornerTile);
    public override int ZIndex => 1;

    public override Point[] WallPoints => new [] {
        TopLeft,
        new Point(TopLeft.X + EdgeWidth, TopLeft.Y),
        new Point(TopLeft.X + EdgeWidth, TopLeft.Y + EdgeWidth),
        new Point(TopLeft.X, TopLeft.Y + EdgeWidth),
        TopLeft
    };
    protected override int TextureRotation => -90;
}