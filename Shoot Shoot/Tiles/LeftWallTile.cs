using System.Drawing;

public class LeftWallTile : WallTile {
    public override Point[] WallPoints => new [] { 
        TopRight,
        BottomRight,
        new Point(BottomRight.X - EdgeWidth, BottomRight.Y),
        new Point(TopRight.X - EdgeWidth, TopRight.Y),
        TopRight,
    };
}