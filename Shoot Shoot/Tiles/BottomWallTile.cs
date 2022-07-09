using Godot;
using System.Drawing;

public class BottomWallTile : WallTile {
    public override int ZIndex => 1;

    public override Point[] WallPoints => new [] {
        TopLeft,
        TopRight,
        new Point(TopRight.X, TopRight.Y + EdgeWidth),
        new Point(TopLeft.X, TopRight.Y + EdgeWidth),
        TopLeft
    };

    private int yOffset = -TileHeight/4;

    protected override Shape2D CollisionShape => new ConvexPolygonShape2D {
        Points = new [] {
            TopLeft.AddY(yOffset).ToVector2(),
            BottomLeft.AddY(yOffset).ToVector2(),
            BottomRight.AddY(yOffset).ToVector2(),
            TopRight.AddY(yOffset).ToVector2(),
        }
    };
}