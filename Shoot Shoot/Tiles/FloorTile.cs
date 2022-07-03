using DrawingColor = System.Drawing.Color;

public class FloorTile : Tile {
    public FloorTile(int id) : base(id) { }

    public override ColorScheme ColorScheme { get; set; } = new ColorScheme {
        Outline = DrawingColor.Black,
        Top = DrawingColor.IndianRed,
        Light = DrawingColor.Red,
        Dark = DrawingColor.DarkRed
    };
}