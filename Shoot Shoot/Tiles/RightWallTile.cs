using System.Drawing;

public class RightWallTile : WallTile {
    public override Point[] WallPoints => new [] { 
        TopLeft,
        BottomLeft,
        new Point(BottomLeft.X + EdgeWidth, BottomRight.Y),
        new Point(TopLeft.X + EdgeWidth, TopRight.Y),
        TopLeft,
    };
}