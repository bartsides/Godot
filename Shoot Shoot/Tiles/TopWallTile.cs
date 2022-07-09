using System.Drawing;

public class TopWallTile : WallTile {
    public override Point[] WallPoints => new [] {
        BottomLeft.AddY(-WallHeight),
        BottomLeft.AddY(-WallHeight - EdgeWidth),
        BottomRight.AddY(-WallHeight - EdgeWidth),
        BottomRight.AddY(-WallHeight),
        BottomLeft.AddY(-WallHeight),
    };
}