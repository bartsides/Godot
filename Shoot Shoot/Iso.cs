using Godot;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

public class Iso : TileMap
{
    private const int tileHeight = 64;
    private const int tileWidth = 128;
    private const int tileBoxHeight = 20;
    private int imageHeight = tileHeight + tileBoxHeight;

    private const string filename = @"temp.png";

    public override void _Ready()
    {
        GenerateTileSet();
        PlaceTiles();
    }

    private void GenerateTileSet() {
        TileSet = new TileSet();
        for (var i = 0; i < 1; i++) {
            AddTile();
        }
    }

    private void AddTile() {
        var id = TileSet.GetLastUnusedTileId();
        TileSet.CreateTile(id);

        var texture = GenerateTexture();
        TileSet.TileSetTexture(id, texture);

        // Used for atlases?
        //var region = new Rect2();
        //TileSet.TileSetRegion(id, region);
    }

    private Texture GenerateTexture() {
        DrawBox(System.Drawing.Color.Black, System.Drawing.Color.IndianRed,
            System.Drawing.Color.DarkRed, System.Drawing.Color.Red);

        var i = new Godot.Image();
        i.Load(filename);
        var format = i.GetFormat();
        GD.Print("Format: " + format);
        //i.CreateFromData(tileWidth, imageHeight, false, Godot.Image.Format.Rgba4444, data);

        var t = new ImageTexture();
        t.CreateFromImage(i);

        return t;
    }

    private void DrawBox(System.Drawing.Color outlineColor, System.Drawing.Color topColor, 
        System.Drawing.Color leftColor, System.Drawing.Color rightColor)
    {
        var outline = new Pen(outlineColor);
        var topBrush = new SolidBrush(topColor);
        var leftBrush = new SolidBrush(leftColor);
        var rightBrush = new SolidBrush(rightColor);

        var bitmap = new Bitmap(tileWidth, imageHeight, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bitmap))
        {
            // Top rectangle points
            var top = new Point(tileWidth / 2, 0);
            var right = new Point(tileWidth - 1, tileHeight / 2);
            var bottom = new Point(tileWidth / 2, tileHeight);
            var left = new Point(0, tileHeight / 2);

            // Bottom rectangle points
            var right2 = new Point(tileWidth - 1, tileHeight / 2 + tileBoxHeight);
            var bottom2 = new Point(tileWidth / 2, tileHeight + tileBoxHeight);
            var left2 = new Point(0, tileHeight / 2 + tileBoxHeight);
            
            var rectTypes = new[]
            {
                (byte) PathPointType.Start, (byte) PathPointType.Line, (byte) PathPointType.Line,
                (byte) PathPointType.Line, (byte) PathPointType.Line
            };

            var topRectPoints = new [] {top, right, bottom, left, top};
            var rightRectPoints = new[] {right, right2, bottom2, bottom, right};
            var leftRectPoints = new[] {left, left2, bottom2, bottom, left};

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

        // using (var stream = new MemoryStream()) {
        //     bitmap.Save(stream, ImageFormat.Bmp);
        //     return stream.ToArray();
        // }

        bitmap.Save(filename, ImageFormat.Png);
    }

    private void PlaceTiles() {
        int width = 10;
        int height = 10;
        
        for (var x = -width/2; x < width/2; x++) {
            for (var y = -height/2; y < height/2; y++) {
                SetCell(x, y, 0);
            }
        }
    }
}
