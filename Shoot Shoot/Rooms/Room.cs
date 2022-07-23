using System.Collections.Generic;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using Godot;

public class Room {
    public ColorScheme ColorScheme { get; }
    public Tileset Tileset { get; }
    public List<object> Items { get; set; }
    public Bitmap MinimapImage { get; private set; }
    public List<Door> Doors { get; set; } = new List<Door>();

    public Vector2 Center { get; set; }
    public Vector2 TopLeft => Center - new Vector2(Width/2, Height/2);
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }

    public int?[][] Tiles { get; set; }

    protected RandomNumberGenerator Rand = new RandomNumberGenerator();

    private const int minShapes = 2;
    private const int maxShapes = 8;
    private const int minShapeWidth = 4;

    private static IShape[] Shapes = new IShape[] {
        new RectangleShape(),
        //new EllipseShape(),
        //new TriangleShape(),
    };

    public Room(ColorScheme colorScheme, Tileset tileset)
    {
        ColorScheme = colorScheme;
        Tileset = tileset;
        
        Width = 20;
        Height = 20;
    }

    public virtual void AddEnemies()
    {
        
    }

    public virtual int?[][] GenerateTiles() {
        Rand.Seed = (ulong) DateTime.UtcNow.Ticks;

        Tiles = new int?[Width][];
        for (var x = 0; x < Width; x++)
            Tiles[x] = new int?[Height];

        DrawImageWithShapes(Width, Height);
        CreateExteriorWalls();
        CreateFloor();
        return Tiles;
    }

    protected virtual void DrawImageWithShapes(int maxWidth, int maxHeight) {
        var brush = new SolidBrush(MinimapColors.Floor);
        var bitmap = new Bitmap(maxWidth, maxHeight, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bitmap)) {
            var numberOfShapes = Rand.RandiRange(minShapes, maxShapes);

            var width = maxWidth;
            var height = maxHeight;
            var center = new Point(maxWidth/2, maxHeight/2);
            for (var i = 0; i < numberOfShapes; i++) {
                var shape = Shapes[Rand.RandiRange(0, Shapes.Length - 1)];

                var size = new Size(Rand.RandiRange(4, width), Rand.RandiRange(4, height));
                center = new Point(center.X - size.Width/2, center.Y - size.Height/2);

                center = shape.Draw(center, size, g, brush, Rand);

                width = Math.Min(center.X, maxWidth - center.X);
                height = Math.Min(center.Y, maxHeight - center.Y);

                //GD.Print($"({center.X},{center.Y}) {width}x{height}");

                if (width <= minShapeWidth || height <= minShapeWidth)
                    break;
            }
        }

        MinimapImage = bitmap;
    }

    protected virtual void CreateExteriorWalls() {
        var start = FindStartingPoint(MinimapImage);

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

            if (next.X.Between(0, MinimapImage.Width - 1) && next.Y.Between(0, MinimapImage.Height - 1) &&
                MinimapImage.GetPixel(next.X, next.Y).A > 0)
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
                        Tiles[current.X][current.Y] = DetermineWallType(prev, current, corner.Value, Tileset);
                        prev = current;
                        current = corner.Value;
                    }

                    Tiles[current.X][current.Y] = DetermineWallType(prev, current, next, Tileset);
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
    }

    protected virtual void CreateFloor() {
        for (var x = 0; x < Tiles.Length; x++)
        for (var y = 0; y < Tiles[x].Length; y++) {
            if (Tiles[x][y] == null && MinimapImage.GetPixel(x, y).A > 0) {
                Tiles[x][y] = Tileset.Floor;
            }
        }
    }

    public virtual Door AddDoor(MooreNeighbor direction) {
        var x = MinimapImage.Width / 2;
        var y = MinimapImage.Height / 2;
        bool found = false;
        
        switch (direction) {
            case MooreNeighbor.Up:
                for (y = 0; y < MinimapImage.Height - 1; y++) {
                    if (MinimapImage.GetPixel(x, y).A > 0) {
                        found = true;
                        break;
                    }
                }
                break;
            case MooreNeighbor.Down:
                for (y = MinimapImage.Height - 1; y >= 0; y--) {
                    if (MinimapImage.GetPixel(x, y).A > 0) {
                        found = true;
                        break;
                    }
                }
                break;
            case MooreNeighbor.Left:
                for (x = 0; x < MinimapImage.Width - 1; x++) {
                    if (MinimapImage.GetPixel(x, y).A > 0) {
                        found = true;
                        break;
                    }
                }
                break;
            case MooreNeighbor.Right:
                for (x = MinimapImage.Width - 1; x >= 0; x--) {
                    if (MinimapImage.GetPixel(x, y).A > 0) {
                        found = true;
                        break;
                    }
                }
                break;
            default:
                throw new NotImplementedException($"Add door with direction {direction} not implemented");
        }
        
        if (!found)
            throw new Exception($"Unable to add {direction} door");

        Tiles[x][y] = Tileset.Door;
        var door = new Door{ Position = Center + new Vector2(x - Width/2, y - Height/2), Direction = direction };
        Doors.Add(door);

        GD.Print($"Door local pos {x},{y}  global pos {door.Position.x},{door.Position.y}   center of room {Center.x},{Center.y}");

        return door;
    }

    protected static int DetermineWallType(Point prev, Point current, Point next, Tileset tileset) {
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

    protected static Point FindStartingPoint(Bitmap bitmap)
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