using Godot;
using System;
using System.Drawing;
using System.Drawing.Imaging;

public static class RoomGenerator {
    private const int minShapes = 2;
    private const  int maxShapes = 8;
    private const int minShapeWidth = 4;
    private static readonly Pen pen = Pens.Black;
    private static readonly Brush brush = Brushes.Black;
    private static RandomNumberGenerator rand = new RandomNumberGenerator();
    private static IShape[] Shapes = new IShape[] {
        new RectangleShape(),
        //new EllipseShape(),
        //new TriangleShape(),
    };

    public static int?[][] Generate(int width, int height, Tileset tileset) {
        rand.Seed = (ulong) DateTime.UtcNow.Ticks;

        var result = new int?[width][];
        for (var x = 0; x < width; x++)
            result[x] = new int?[height];

        var bitmap = DrawImageWithShapes(width, height);
        result = CreateExteriorWalls(bitmap, result, tileset);
        result = CreateFloor(bitmap, result, tileset);
        result = AddDoor(bitmap, result, tileset, MooreNeighbor.Up);
        return result;
    }

    private static Bitmap DrawImageWithShapes(int maxWidth, int maxHeight) {
        var bitmap = new Bitmap(maxWidth, maxHeight, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bitmap)) {
            var numberOfShapes = rand.RandiRange(minShapes, maxShapes);

            var width = maxWidth;
            var height = maxHeight;
            var center = new Point(maxWidth/2, maxHeight/2);
            for (var i = 0; i < numberOfShapes; i++) {
                var shape = Shapes[rand.RandiRange(0, Shapes.Length - 1)];

                var size = new Size(rand.RandiRange(4, width), rand.RandiRange(4, height));
                center = new Point(center.X - size.Width/2, center.Y - size.Height/2);

                center = shape.Draw(center, size, g, brush, rand);

                width = Math.Min(center.X, maxWidth - center.X);
                height = Math.Min(center.Y, maxHeight - center.Y);

                //GD.Print($"({center.X},{center.Y}) {width}x{height}");

                if (width <= minShapeWidth || height <= minShapeWidth)
                    break;
            }
        }

        return bitmap;
    }

    private static int?[][] CreateExteriorWalls(Bitmap bitmap, int?[][] result, Tileset tileset) {
        var start = FindStartingPoint(bitmap);

        var current = start;
        var prev = current;
        var neighbor = MooreNeighbor.UpLeft;
        var startingNeighbor = neighbor;
        var next = current.GetNeighbor(neighbor);
        var lastNeighborPoint = next;

        // Skip first and process it last
        var startProcessedNum = 0;
        var exit = false;

        while (true)
        {
            if (startProcessedNum > 1) exit = true;

            if (next.X.Between(0, bitmap.Width - 1) && next.Y.Between(0, bitmap.Height - 1) &&
                bitmap.GetPixel(next.X, next.Y).A > 0)
            {
                if (startProcessedNum > 0) {
                    Point? corner = null;

                    // Add corners when needed
                    if (neighbor == MooreNeighbor.DownRight) {
                        corner = current.GetNeighbor(MooreNeighbor.Right);
                    }
                    else if (neighbor == MooreNeighbor.DownLeft) {
                        corner = current.GetNeighbor(MooreNeighbor.Down);
                    }
                    else if (neighbor == MooreNeighbor.UpLeft) {
                        corner = current.GetNeighbor(MooreNeighbor.Left);
                    }
                    else if (neighbor == MooreNeighbor.UpRight) {
                        corner = current.GetNeighbor(MooreNeighbor.Up);
                    }

                    if (corner.HasValue) {
                        result[current.X][current.Y] = DetermineWallType(prev, current, corner.Value, tileset);
                        prev = current;
                        current = corner.Value;
                    }

                    result[current.X][current.Y] = DetermineWallType(prev, current, next, tileset);
                }

                if (current == start)
                    startProcessedNum++;

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

    private static int?[][] CreateFloor(Bitmap bitmap, int?[][] result, Tileset tileset) {
        for (var x = 0; x < result.Length; x++)
        for (var y = 0; y < result[x].Length; y++) {
            if (result[x][y] == null && bitmap.GetPixel(x, y).A > 0) {
                result[x][y] = tileset.Floor;
            }
        }
        return result;
    }

    private static int?[][] AddDoor(Bitmap bitmap, int?[][] result, Tileset tileset, MooreNeighbor direction) {
        var x = bitmap.Width / 2;
        var y = bitmap.Height / 2;
        switch (direction) {
            case MooreNeighbor.Up:
                for (y = 0; y < bitmap.Height - 1; y++) {
                    if (bitmap.GetPixel(x, y).A > 0) {
                        result[x][y] = tileset.Door;
                        return result;
                    }
                }
                break;
            case MooreNeighbor.Down:
                for (y = bitmap.Height - 1; y >= 0; y--) {
                    if (bitmap.GetPixel(x, y).A > 0) {
                        result[x][y] = tileset.Door;
                        return result;
                    }
                }
                break;
            case MooreNeighbor.Left:
                for (x = 0; x < bitmap.Width - 1; x++) {
                    if (bitmap.GetPixel(x, y).A > 0) {
                        result[x][y] = tileset.Door;
                        return result;
                    }
                }
                break;
            case MooreNeighbor.Right:
                for (x = bitmap.Width - 1; x >= 0; x--) {
                    if (bitmap.GetPixel(x, y).A > 0) {
                        result[x][y] = tileset.Door;
                        return result;
                    }
                }
                break;
            default:
                throw new NotImplementedException($"Add door with direction {direction} not implemented");
        }
        throw new Exception($"Unable to add {direction} door");
    }

    private static int DetermineWallType(Point prev, Point current, Point next, Tileset tileset) {
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
            if (after == MooreNeighbor.Right) return tileset.TopLeftCorner;
        }
        else if (before == MooreNeighbor.Left) {
            if (after == MooreNeighbor.Up) return tileset.TopWall;
            if (after == MooreNeighbor.Right) return tileset.TopWall;
            if (after == MooreNeighbor.Down) return tileset.TopRightCorner;
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
}