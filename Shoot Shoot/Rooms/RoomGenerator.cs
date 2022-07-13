using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Godot;

public static class RoomGenerator {
    private static readonly Pen pen = Pens.Black;

    public static int?[][] Generate(int width, int height, Tileset tileset) {
        var result = new int?[width][];
        for (var x = 0; x < width; x++) {
            result[x] = new int?[height];
        }

        var bitmap = DrawImageWithShapes(width, height);
        result = CreateExteriorWalls(bitmap, result, tileset);
        return result;
    }

    private static int?[][] CreateExteriorWalls(Bitmap bitmap, int?[][] result, Tileset tileset) {
        var start = FindStartingPoint(bitmap);

        var current = start;
        var prev = current;
        var neighbor = MooreNeighbor.UpLeft;
        var next = current.GetNeighbor(neighbor);
        var lastNeighborPoint = next;

        // Skip first and process it last
        var firstProcessed = false;
        var exit = false;


        while (true)
        {
            if (firstProcessed && current == start)
                exit = true;

            if (next.X.Between(0, bitmap.Width - 1) && next.Y.Between(0, bitmap.Height - 1) &&
                bitmap.GetPixel(next.X, next.Y).A > 0)
            {
                if (firstProcessed) {
                    MooreNeighbor? nextNeighbor = null;
                    Point? corner = null;

                    if (neighbor == MooreNeighbor.DownRight) {
                        corner = current.GetNeighbor(MooreNeighbor.Right);
                        //nextNeighbor = MooreNeighbor.Down;
                    }
                    else if (neighbor == MooreNeighbor.DownLeft) {
                        corner = current.GetNeighbor(MooreNeighbor.Down);
                        //nextNeighbor = MooreNeighbor.Left;
                    }
                    else if (neighbor == MooreNeighbor.UpLeft) {
                        corner = current.GetNeighbor(MooreNeighbor.Left);
                        //nextNeighbor = MooreNeighbor.Up;
                    }
                    else if (neighbor == MooreNeighbor.UpRight) {
                        corner = current.GetNeighbor(MooreNeighbor.Up);
                        //nextNeighbor = MooreNeighbor.Right;
                    }

                    if (corner.HasValue) {
                        //neighbor = nextNeighbor.Value;

                        result[current.X][current.Y] = DetermineWallType(prev, current, corner.Value, tileset);

                        //GD.Print($"Filling top right corner ({prev.X},{prev.Y}) ({current.X},{current.Y}) ({corner.X},{corner.Y}) ({next.X},{next.Y})");
                        prev = current;
                        current = corner.Value;
                    }

                    result[current.X][current.Y] = DetermineWallType(prev, current, next, tileset);
                }

                firstProcessed = true;

                // Found next pixel
                prev = current;
                current = next;
                neighbor = current.DetermineNeighbor(lastNeighborPoint);
                lastNeighborPoint = next;
                next = current.GetNeighbor(neighbor);
            }
            else
            {
                neighbor = neighbor.Next();
                lastNeighborPoint = next;
                next = current.GetNeighbor(neighbor);
            }

            if (exit) break;
        }



        return result;
    }

    private static int DetermineWallType(Point prev, Point current, Point next, Tileset tileset) {
        // GD.Print($"{prev.X},{prev.Y} - {current.X},{current.Y} - {next.X},{next.Y}");
        var before = current.DetermineNeighbor(prev);
        var after = current.DetermineNeighbor(next);

        if (before == MooreNeighbor.Up) {
            if (after == MooreNeighbor.Right) return tileset.TopWall;
            if (after == MooreNeighbor.Down) return tileset.RightWall;
            if (after == MooreNeighbor.Left) return tileset.BottomRightCorner;
        }
        else if (before == MooreNeighbor.Right) {
            if (after == MooreNeighbor.Down) return tileset.BottomRightWall;
            if (after == MooreNeighbor.Left) return tileset.BottomWall;
            if (after == MooreNeighbor.Up) return tileset.BottomLeftCorner;
        }
        else if (before == MooreNeighbor.Down) {
            if (after == MooreNeighbor.Left) return tileset.BottomLeftWall;
            if (after == MooreNeighbor.Up) return tileset.LeftWall;
            if (after == MooreNeighbor.Right) return tileset.TopLeftWall;
        }
        else if (before == MooreNeighbor.Left) {
            if (after == MooreNeighbor.Up) return tileset.TopLeftWall;
            if (after == MooreNeighbor.Right) return tileset.TopWall;
            if (after == MooreNeighbor.Down) return tileset.TopRightWall;
        }

        return tileset.MiddleWall;
    }

    private static Point FindStartingPoint(Bitmap bitmap)
    {
        for (var x = 0; x < bitmap.Width; x++) {
            for (var y = 0; y < bitmap.Height; y++) {
                if (bitmap.GetPixel(x, y).A <= 0)
                    continue;
                
                return new Point(x, y);
            }
        }

        throw new Exception("Unable to find starting point");
    }

    private static Bitmap DrawImageWithShapes(int width, int height) {
        var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bitmap)) {
            var points = new Point[]
            {
                new Point(3, 3), new Point(7, 4), new Point(5, 6), new Point(4, 9), new Point(3, 3)
            };
            var types = new[]
            {
                (byte) PathPointType.Start, (byte) PathPointType.Line, (byte) PathPointType.Line,
                (byte) PathPointType.Line, (byte) PathPointType.Line
            };
            g.DrawPath(pen, new GraphicsPath(points, types));

            g.DrawEllipse(pen, new Rectangle(4, 4, 15, 15));
        }

        return bitmap;
    }
}