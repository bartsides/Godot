using System.Drawing;
using System.Collections.Generic;

public abstract class LevelTemplate {
    public virtual List<Brush> FloorBrushes { get; set; }
    public virtual Brush TopBrush { get; set; }
    public virtual List<Brush> WallBrushes { get; set; }

    protected static Brush CreateBrush(string file, int col = 0, int row = 0, int cellSize = 64) {
        //Godot.GD.Print($"Creating brush {col}, {row}");
        int x = col * cellSize;
        int y = row * cellSize;

        var bitmap = (Bitmap) System.Drawing.Image.FromFile(file);
        return new TextureBrush(bitmap.Clone(new Rectangle(x, y, cellSize, cellSize), bitmap.PixelFormat));
    }

    protected static Brush CreateBrush(string file) {
        //Godot.GD.Print($"Creating brush from file");

        var bitmap = (Bitmap) System.Drawing.Image.FromFile(file);
        return new TextureBrush(bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat));
    }
}