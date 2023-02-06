using Godot;
using System.Collections.Generic;
using System.Drawing;

public class LevelTemplate : Node2D
{
    public virtual List<Brush> FloorBrushes { get; set; }
    public virtual Brush TopBrush { get; set; }
    public virtual List<Brush> WallBrushes { get; set; }

    public override void _Ready() {
        // TODO: Move to getting images from godot scenes
        TopBrush = CreateBrush("assets/tiles/topwall/brick1.png");
        FloorBrushes = new List<Brush> {
            CreateBrush("assets/tiles/floor/stone1.png"),
            CreateBrush("assets/tiles/floor/stone2.png"),
            CreateBrush("assets/tiles/floor/stone3.png"),
        };
        WallBrushes = new List<Brush> {
            CreateBrush("assets/tiles/wall/brick1.png"),
            CreateBrush("assets/tiles/wall/brick2.png"),
            CreateBrush("assets/tiles/wall/brick3.png"),
            CreateBrush("assets/tiles/wall/brick4.png"),
            //CreateBrush("assets/tiles/wall/brick5.png"),
        };
    }

    protected static Brush CreateBrush(string file, int col = 0, int row = 0, int cellSize = 64) {
        int x = col * cellSize;
        int y = row * cellSize;

        var bitmap = (Bitmap) System.Drawing.Image.FromFile(file);
        return new TextureBrush(bitmap.Clone(new Rectangle(x, y, cellSize, cellSize), bitmap.PixelFormat));
    }

    protected static Brush CreateBrush(string file) {
        var bitmap = (Bitmap) System.Drawing.Image.FromFile(file);
        return new TextureBrush(bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat));
    }
}