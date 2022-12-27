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
    protected virtual int TextureRotation { get; set; } = 0;

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
        Light = DrawingColor.Black,
        Dark = DrawingColor.DarkRed,
    };

    public override void SetBrush(Brush brush)
    {
        VoidBrush = brush;
    }

    public override void SetSecondBrush(Brush secondBrush)
    {
        TopBrush = secondBrush;
    }

    protected override void GenerateTexture()
    {
        var outline = new Pen(ColorScheme.Outline);
        if (TopBrush == null)
            TopBrush = new SolidBrush(ColorScheme.Light);
        
        if (TopBrush is TextureBrush textureBrush && TextureRotation != 0) {
            textureBrush = (TextureBrush) textureBrush.Clone();
            textureBrush.RotateTransform(TextureRotation);
            TopBrush = textureBrush;
        }

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
                g.FillPath(TopBrush, wallPath);
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