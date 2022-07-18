using System.Drawing;
using Godot;

public class BottomLeftWallTile : WallTile {
    public override string TileName => nameof(BottomLeftWallTile);
    public override int ZIndex => 1;
    protected override Shape2D CollisionShape => new ConvexPolygonShape2D();

    public override Point[] WallPoints => new [] {
        TopLeft,
        TopRight,
        BottomRight,
        new Point(BottomRight.X - EdgeWidth, BottomRight.Y),
        new Point(BottomRight.X - EdgeWidth, TopRight.Y + EdgeWidth),
        new Point(TopLeft.X, TopLeft.Y + EdgeWidth),
        TopLeft,
    };
}