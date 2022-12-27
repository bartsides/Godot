using Godot;
using System.Drawing;

public class BottomWallTile : WallTile {
    public override string TileName => nameof(BottomWallTile);
    public override int ZIndex => 1;

    public override Point[] WallPoints => new [] {
        TopLeft,
        TopRight,
        new Point(TopRight.X, TopRight.Y + EdgeWidth),
        new Point(TopLeft.X, TopRight.Y + EdgeWidth),
        TopLeft
    };

    protected int yOffset = TileHeight/4;

    protected override Shape2D CollisionShape => new ConvexPolygonShape2D {
        Points = new [] {
            BottomLeft.ToVector2(),
            TopLeft.ToVector2(),
            TopRight.ToVector2(),
            BottomRight.ToVector2(),
            BottomLeft.ToVector2(),
        }
    };
    protected override int TextureRotation => -90;
}