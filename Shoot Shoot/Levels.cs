using System.Collections.Generic;
using System.Drawing;

public static class Levels {
    public static List<Brush> WallBrushes => new List<Brush>{
        CreateBrush("assets/Wall_Sheet.png", 0, 0),
        CreateBrush("assets/Wall_Sheet.png", 1, 0),
        CreateBrush("assets/Wall_Sheet.png", 2, 0),
        CreateBrush("assets/Wall_Sheet.png", 3, 0),
        CreateBrush("assets/Wall_Sheet.png", 4, 0),
        //CreateBrush("assets/Wall_Sheet.png", 5, 0),
        // CreateBrush("assets/Wall_Sheet.png", 6, 0),
        // CreateBrush("assets/Wall_Sheet.png", 7, 0),
        // CreateBrush("assets/Wall_Sheet.png", 8, 0),
        // CreateBrush("assets/Wall_Sheet.png", 9, 0),
    };

    private static Brush CreateBrush(string file, int col, int row, int cellSize = 64) {
        Godot.GD.Print($"Creating brush {col}, {row}");
        int x = col * cellSize;
        int y = row * cellSize;

        var bitmap = (Bitmap) System.Drawing.Image.FromFile(file);
        return new TextureBrush(bitmap.Clone(new Rectangle(x, y, cellSize, cellSize), bitmap.PixelFormat));
    }
}