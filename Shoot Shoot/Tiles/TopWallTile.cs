using System.Drawing;
using Godot;

public class TopWallTile : WallTile {
    public override string TileName => nameof(TopWallTile);
    public override Point[] WallPoints => new [] {
        BottomLeft.AddY(-WallHeight),
        BottomLeft.AddY(-WallHeight - EdgeWidth),
        BottomRight.AddY(-WallHeight - EdgeWidth),
        BottomRight.AddY(-WallHeight),
        BottomLeft.AddY(-WallHeight),
    };
    protected override int TextureRotation => 90;
}