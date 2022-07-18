using System.Drawing;

public class MiddleWallTile : WallTile {
    public override string TileName => nameof(MiddleWallTile);
    public override int ZIndex => 1;

    public override Point[] WallPoints => new [] {
        TopLeft.AddY(-WallHeight),
        TopRight.AddY(-WallHeight),
        BottomRight.AddY(-WallHeight),
        BottomLeft.AddY(-WallHeight),
        TopLeft.AddY(-WallHeight)
    };
}