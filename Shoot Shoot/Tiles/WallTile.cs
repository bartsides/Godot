using Godot;

using DrawingColor = System.Drawing.Color;

public class WallTile : Tile
{
    public override int TileBoxHeight => 60;
    public override int TileTopPadding => 0;
    protected override Shape2D CollisionShape => new ConvexPolygonShape2D{
        Points = new Vector2[] {
            Top2.ToVector2(),
            Right2.ToVector2(),
            Bottom2.ToVector2(),
            Left2.ToVector2(),
            Top2.ToVector2(),
        }
    };
    protected override Transform2D CollisionTransform => new Transform2D(0, new Vector2(0, -TileHeight));
    //protected override Vector2? CollisionOffset => new Vector2(0, -TileHeight - TileBoxHeight);

    public WallTile(int id) : base(id) { }

    public override ColorScheme ColorScheme { get; set; } = new ColorScheme {
        Outline = DrawingColor.Black,
        Top = DrawingColor.IndianRed,
        Light = DrawingColor.Red,
        Dark = DrawingColor.DarkRed
    };
}