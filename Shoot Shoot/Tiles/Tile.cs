using Godot;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

public abstract class Tile {
    public int Id { get; }
    public int TileHeight = 64;
    public int TileWidth = 128;
    protected const int DefaultTileBoxHeight = 10;
    public virtual int TileBoxHeight { get; set; } = DefaultTileBoxHeight;
    protected const int DefaultOffsetY = 50;
    public static Vector2 Offset { get; set; } = new Vector2(0, -DefaultOffsetY);
    public virtual int TileTopPadding { get; set; } = DefaultOffsetY;
    public int ImageHeight => TileTopPadding + TileHeight + TileBoxHeight;

    protected virtual Shape2D CollisionShape { get; set; } = null;
    protected virtual Transform2D CollisionTransform { get; set; } = new Transform2D(0, Vector2.Zero);

    public abstract ColorScheme ColorScheme { get; set; }

    protected const string filename = @"temp.png";

    protected TilePaths? _TilePaths = null;
    public TilePaths TilePaths { 
        get {
            if (_TilePaths == null)
                GenerateTilePaths();
            return _TilePaths.Value;
        }
    }

    protected Texture _texture = null;
    public Texture Texture {
        get {
            if (_texture == null)
                GenerateTexture();
            return _texture;
        }
    }

    // Top rectangle points
    protected Point Top => new Point(TileWidth / 2, TileTopPadding);
    protected Point Right => new Point(TileWidth - 1, TileHeight / 2 + TileTopPadding);
    protected Point Bottom => new Point(TileWidth / 2, TileHeight + TileTopPadding);
    protected Point Left => new Point(0, TileHeight / 2 + TileTopPadding);

    // Bottom rectangle points
    protected Point Top2 => new Point(Top.X, Top.Y + TileBoxHeight);
    protected Point Right2 => new Point(Right.X, Right.Y + TileBoxHeight);
    protected Point Bottom2 => new Point(Bottom.X, Bottom.Y + TileBoxHeight);
    protected Point Left2 => new Point(Left.X, Left.Y + TileBoxHeight);

    public Tile(int id) {
        Id = id;
    }

    public virtual void Setup(TileSet tileset) {
        tileset.TileSetTexture(Id, Texture);
        tileset.TileSetTextureOffset(Id, Offset);
        if (CollisionShape != null) {
            tileset.TileAddShape(Id, CollisionShape, CollisionTransform);
            tileset.TileSetShape(this.Id, Id, CollisionShape);
            GD.Print(GD.Var2Str(CollisionShape));
        }
    }

    protected virtual void GenerateTexture() {
        var outline = new Pen(ColorScheme.Outline);
        var topBrush = new SolidBrush(ColorScheme.Top);
        var leftBrush = new SolidBrush(ColorScheme.Light);
        var rightBrush = new SolidBrush(ColorScheme.Dark);

        var bitmap = new Bitmap(TileWidth, ImageHeight, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bitmap))
        {
            var rectTypes = new[]
            {
                (byte) PathPointType.Start, (byte) PathPointType.Line, (byte) PathPointType.Line,
                (byte) PathPointType.Line, (byte) PathPointType.Line
            };

            var topRectPoints = new [] {Top, Right, Bottom, Left, Top};
            var rightRectPoints = new[] {Right, Right2, Bottom2, Bottom, Right};
            var leftRectPoints = new[] {Left, Left2, Bottom2, Bottom, Left};

            var topPath = new GraphicsPath(topRectPoints, rectTypes);
            var rightPath = new GraphicsPath(rightRectPoints, rectTypes);
            var leftPath = new GraphicsPath(leftRectPoints, rectTypes);

            // Draw faces
            g.FillPath(topBrush, topPath);
            g.FillPath(rightBrush, rightPath);
            g.FillPath(leftBrush, leftPath);

            // Draw outlines
            g.DrawPath(outline, topPath);
            g.DrawPath(outline, rightPath);
            g.DrawPath(outline, leftPath);
        }

        bitmap.Save(filename, ImageFormat.Png);

        var image = new Godot.Image();
        image.Load(filename);

        var texture = new ImageTexture();
        texture.CreateFromImage(image);
        _texture = texture;
    }

    private void GenerateTilePaths() {
        // Top rectangle points
        var top = new Point(TileWidth / 2, 0);
        var right = new Point(TileWidth - 1, TileHeight / 2);
        var bottom = new Point(TileWidth / 2, TileHeight);
        var left = new Point(0, TileHeight / 2);

        // Bottom rectangle points
        var right2 = new Point(TileWidth - 1, TileHeight / 2 + TileBoxHeight);
        var bottom2 = new Point(TileWidth / 2, TileHeight + TileBoxHeight);
        var left2 = new Point(0, TileHeight / 2 + TileBoxHeight);
        
        var rectTypes = new[]
        {
            (byte) PathPointType.Start, (byte) PathPointType.Line, (byte) PathPointType.Line,
            (byte) PathPointType.Line, (byte) PathPointType.Line
        };

        var topRectPoints = new [] {top, right, bottom, left, top};
        var rightRectPoints = new[] {right, right2, bottom2, bottom, right};
        var leftRectPoints = new[] {left, left2, bottom2, bottom, left};

        _TilePaths = new TilePaths {
            Top = new GraphicsPath(topRectPoints, rectTypes),
            Right = new GraphicsPath(rightRectPoints, rectTypes),
            Left = new GraphicsPath(leftRectPoints, rectTypes)
        };
    }
}