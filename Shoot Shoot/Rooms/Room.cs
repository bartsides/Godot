using System.Collections.Generic;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using Godot;

public class Room : Node2D {
    private bool debug = true;
    public ColorScheme ColorScheme { get; set; }
    public Tileset Tileset { get; set; }
    public List<object> Items { get; set; }
    public Bitmap MinimapImage { get; private set; }
    public List<Door> Doors { get; set; } = new List<Door>();

    public Vector2 Center { get; set; } = Vector2.Zero;
    public Vector2 TopLeft => Center - new Vector2(Width/2, Height/2);
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }
    public Level Level { get; set; }
    
    public int?[][] Tiles { get; set; }
    private List<Vector2> SpawnableTiles = new List<Vector2>();

    protected RandomNumberGenerator Rand = new RandomNumberGenerator();

    private const int minShapes = 2;
    private const int maxShapes = 8;
    private const int minShapeWidth = 4;

    private PackedScene EnemyScene;
    public List<Enemy> Enemies { get; set; }

    private static IShape[] Shapes = new IShape[] {
        new RectangleShape(),
        //new EllipseShape(),
        //new TriangleShape(),
    };

    public Room() {
        Width = 20;
        Height = 20;
        EnemyScene = GD.Load<PackedScene>("res://Shoot Shoot/Enemy.tscn");
        Rand.Seed = (ulong) DateTime.UtcNow.Ticks;
    }

    public virtual void AddEnemies()
    {
        var numEnemies = Rand.RandiRange(1, 10);
        if (numEnemies == 0) return;

        for (var i = 0; i < numEnemies; i++) {
            if (SpawnableTiles.Count < 1) {
                GD.Print($"No spawnable points. Skipping spawning {numEnemies - i}");
                return;
            }
            var enemy = EnemyScene.Instance<Enemy>();
            var spawnableTile = SpawnableTiles[Rand.RandiRange(0, SpawnableTiles.Count-1)];
            enemy.Position = Level.FloorTileMap.MapToWorld(TopLeft + spawnableTile);
            GD.Print($"Spawned enemy ({enemy.Position.x},{enemy.Position.y})");
            AddChild(enemy);
        }
    }

    public virtual int?[][] GenerateTiles() {
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
        var numberOfShapes = Rand.RandiRange(minShapes, maxShapes);

        var width = maxWidth;
        var height = maxHeight;
        var center = new Point(maxWidth/2, maxHeight/2);
        
        MinimapImage = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(MinimapImage)) {
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
                        SetWall(prev, current, corner.Value);
                        prev = current;
                        current = corner.Value;
                    }

                    SetWall(prev, current, next);
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

    protected void SetWall(Point prev, Point current, Point next) {
        Tiles[current.X][current.Y] = DetermineWallType(prev, current, next, Tileset);
        MinimapImage.SetPixel(current.X, current.Y, MinimapColors.Wall);
    }

    protected virtual void CreateFloor() {
        for (var x = 0; x < Tiles.Length; x++)
        for (var y = 0; y < Tiles[x].Length; y++) {
            if (Tiles[x][y] == null && MinimapImage.GetPixel(x, y).A > 0) {
                Tiles[x][y] = Tileset.Floor;
                SpawnableTiles.Add(new Vector2(x, y));
            }
        }
    }

    public virtual Door AddDoor(MooreNeighbor direction) {
        if (debug) GD.Print($"Adding door {direction}");
        var location = FindDoorLocation(direction);
        var x = (int)location.x;
        var y = (int)location.y;

        var door = new Door { 
            Position = Center + new Vector2(x - Width/2, y - Height/2), 
            Direction = direction 
        };
        Doors.Add(door);
        
        // Set tiles
        Tiles[x][y] = Tileset.Door;
        switch (direction) {
            case MooreNeighbor.Up:
                Tiles[x-1][y] = Tileset.TopWall;
                Tiles[x+1][y] = Tileset.TopWall;
                break;
            case MooreNeighbor.Down:
                Tiles[x-1][y] = Tileset.BottomLeftWall;
                Tiles[x+1][y] = Tileset.BottomRightWall;
                break;
            case MooreNeighbor.Left:
            case MooreNeighbor.Right:
            default:
                break;
        }

        //GD.Print($"Door local pos {location.x},{location.y}  global pos {door.Position.x},{door.Position.y}   center of room {Center.x},{Center.y}");

        return door;
    }

    private Vector2 FindDoorLocation(MooreNeighbor direction) {
        var x = MinimapImage.Width / 2;
        var y = MinimapImage.Height / 2;

        switch (direction) {
            case MooreNeighbor.Up:
                for (y = 0; y < MinimapImage.Height - 1; y++) {
                    if (MinimapImage.GetPixel(x, y).A > 0) {
                        return new Vector2(x, y);
                    }
                }
                break;
            case MooreNeighbor.Down:
                for (y = MinimapImage.Height - 1; y >= 0; y--) {
                    if (MinimapImage.GetPixel(x, y).A > 0) {
                        return new Vector2(x, y);
                    }
                }
                break;
            case MooreNeighbor.Left:
                for (x = 0; x < MinimapImage.Width - 1; x++) {
                    if (MinimapImage.GetPixel(x, y).A > 0) {
                        return new Vector2(x, y);
                    }
                }
                break;
            case MooreNeighbor.Right:
                for (x = MinimapImage.Width - 1; x >= 0; x--) {
                    if (MinimapImage.GetPixel(x, y).A > 0) {
                        return new Vector2(x, y);
                    }
                }
                break;
            default:
                throw new NotImplementedException($"Add door with direction {direction} not implemented");
        }

        throw new Exception($"Unable to add {direction} door");
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