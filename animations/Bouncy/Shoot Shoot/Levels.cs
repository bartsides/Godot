using System.Collections.Generic;
using System.Drawing;

public static class Levels {
    public static class Level1 {
        public static List<Brush> WallBrushes => new List<Brush>{
            CreateBrush("assets/tilesheet.png", 0, 0),
        };
    }

    private static Brush CreateBrush(string file, int col, int row, int cellSize = 128) {
        int x = col * cellSize;
        int y = row * cellSize;

        var bitmap = (Bitmap) System.Drawing.Image.FromFile(file);
        return new TextureBrush(bitmap.Clone(new Rectangle(x, y, cellSize, cellSize), bitmap.PixelFormat));
    }
}