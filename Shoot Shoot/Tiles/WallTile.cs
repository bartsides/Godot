using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Godot;

using DrawingColor = System.Drawing.Color;

public class WallTile : Tile
{
    public override string TileName => nameof(WallTile);
    public virtual int WallHeight { get; set; } = TileHeight * 1;
    public virtual int EdgeWidth { get; set; } = TileWidth / 4;
    public virtual Point[] WallPoints { get; set; }
    public virtual byte[] WallPointTypes { get; set; }
    public virtual Brush VoidBrush { get; set; } = Constants.VoidBrush;

    protected override Shape2D CollisionShape => new ConvexPolygonShape2D {
        Points = new [] {
            AddOffset(TopLeft),
            AddOffset(TopRight),
            AddOffset(BottomRight),
            AddOffset(BottomLeft),
        }
    };

    public override ColorScheme ColorScheme { get; set; } = new ColorScheme {
        Outline = DrawingColor.Black,
        Top = DrawingColor.IndianRed,
        Light = DrawingColor.Red,
        Dark = DrawingColor.DarkRed,
    };

    protected override void GenerateTexture()
    {
        var outline = new Pen(ColorScheme.Outline);
        var topBrush = new SolidBrush(ColorScheme.Dark);
        var wallBrush = new SolidBrush(ColorScheme.Light);

        var bitmap = new Bitmap(TileWidth, ImageHeight, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bitmap))
        {
            var voidPoints = new [] { TopLeft, TopRight, BottomRight, BottomLeft, TopLeft };
            var voidPath = new GraphicsPath(voidPoints, GetPathPointTypes(voidPoints));
            g.FillPath(VoidBrush, voidPath);

            if (WallPoints?.Length > 0) {
                if (WallPointTypes == null)
                    WallPointTypes = GetPathPointTypes(WallPoints);
                var wallPath = new GraphicsPath(WallPoints, WallPointTypes);
                g.FillPath(wallBrush, wallPath);
            }
        }

        bitmap.Save(filename, ImageFormat.Png);

        var image = new Godot.Image();
        image.Load(filename);

        var texture = new ImageTexture();
        texture.CreateFromImage(image);
        _texture = texture;
    }
}