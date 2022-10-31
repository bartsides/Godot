using System.Drawing;
using Godot;

public class TopWallTile : WallTile {
    public override string TileName => nameof(TopWallTile);
    // public override Brush VoidBrush { 
    //     get 
    //     {
    //         return new TextureBrush((Bitmap) System.Drawing.Image.FromFile("assets/obey.png"));
    //     }
    // }

    public override Point[] WallPoints => new [] {
        BottomLeft.AddY(-WallHeight),
        BottomLeft.AddY(-WallHeight - EdgeWidth),
        BottomRight.AddY(-WallHeight - EdgeWidth),
        BottomRight.AddY(-WallHeight),
        BottomLeft.AddY(-WallHeight),
    };
}