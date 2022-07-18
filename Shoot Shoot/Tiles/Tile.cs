using Godot;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;

public class Tile {
    public int Id { get; set; }

    protected static int TileHeight { get; set; } = 64;
    protected static int TileWidth { get; set; } = 64;

    protected const int DefaultTileBoxHeight = 0;
    protected const int DefaultOffsetY = 50;
    protected const string filename = @"temp.png";

    public virtual string TileName { get; set; } = "Tile";
    public virtual int ZIndex { get; set; } = 0;
    public virtual int TileBoxHeight { get; set; } = DefaultTileBoxHeight;
    public virtual Vector2 Offset { get; set; } = new Vector2(0, -DefaultOffsetY);
    public virtual int TileTopPadding { get; set; } = DefaultOffsetY;

    protected virtual Shape2D CollisionShape { get; set; } = null;
    protected virtual Transform2D CollisionTransform { get; set; } = new Transform2D(0, Vector2.Zero);
    public virtual ColorScheme ColorScheme { get; set; }

    public int ImageHeight => TileTopPadding + TileHeight + TileBoxHeight;

    protected Texture _texture = null;
    public Texture Texture {
        get {
            if (_texture == null)
                GenerateTexture();
            return _texture;
        }
    }

    // Top rectangle points
    protected Point TopLeft => new Point(0, TileTopPadding);
    protected Point TopRight => new Point(TileWidth - 1, TileTopPadding);
    protected Point BottomLeft => new Point(0, TileHeight + TileTopPadding);
    protected Point BottomRight => new Point(TileWidth - 1, TileHeight + TileTopPadding);

    public Tile() { }

    public virtual void Setup(TileSet tileset) {
        tileset.TileSetTexture(Id, Texture);
        tileset.TileSetTextureOffset(Id, Offset);
        tileset.TileSetZIndex(Id, ZIndex);
        tileset.TileSetName(Id, TileName);

        if (CollisionShape != null)
            tileset.TileAddShape(Id, CollisionShape, CollisionTransform);
    }

    protected virtual void GenerateTexture() {
        var outline = new Pen(ColorScheme.Outline);
        var topBrush = new SolidBrush(ColorScheme.Top);
        var leftBrush = new SolidBrush(ColorScheme.Light);
        var rightBrush = new SolidBrush(ColorScheme.Dark);

        var bitmap = new Bitmap(TileWidth, ImageHeight, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bitmap))
        {
            var points = new [] { TopLeft, TopRight, BottomRight, BottomLeft, TopLeft };
            var path = new GraphicsPath(points, GetPathPointTypes(points));

            g.FillPath(topBrush, path);
            g.DrawPath(outline, path);
        }

        bitmap.Save(filename, ImageFormat.Png);

        var image = new Godot.Image();
        image.Load(filename);

        var texture = new ImageTexture();
        texture.CreateFromImage(image);
        _texture = texture;
    }

    protected Vector2 AddOffset(Point point) {
        return new Vector2(point.X + Offset.x, point.Y + Offset.y);
    }

    protected static byte[] GetPathPointTypes(Point[] points) {
        var result = new List<byte> { (byte) PathPointType.Start };
        for (var i = 0; i < points.Length - 1; i++)
            result.Add((byte) PathPointType.Line);
        return result.ToArray();
    }
}