using System.Drawing;
using Godot;

public class BottomRightWallTile : WallTile {
    public override string TileName => nameof(BottomRightWallTile);
    public override int ZIndex => 1;
    protected override Shape2D CollisionShape => new ConvexPolygonShape2D();

    public override Point[] WallPoints => new [] {
        TopLeft,
        TopRight,
        new Point(TopRight.X, TopRight.Y + EdgeWidth),
        new Point(TopLeft.X + EdgeWidth, TopLeft.Y + EdgeWidth),
        new Point(BottomLeft.X + EdgeWidth, BottomLeft.Y),
        BottomLeft,
        TopLeft
    };
    protected override int TextureRotation => -90;
}