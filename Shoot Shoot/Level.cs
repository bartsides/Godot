using Godot;

public class Level : Navigation2D
{
    public int FloorNumber { get; set; }
    public ColorScheme ColorScheme { get; set; }
    public Tileset Tileset { get; set; }
    private TileMap floorTileMap;
    private const int NumberOfRooms = 2;
    
    public override void _Ready()
    {
        floorTileMap = GetNode<TileMap>("FloorTileMap");
        //floorTileMap.ShowCollision = true;
    }
    
    public void GenerateLevel(bool regenerateTileset) {
        Reset();

        if (regenerateTileset)
            GenerateTileSet();

        var startingRoom = new Room(ColorScheme, Tileset);
        startingRoom.Center = Vector2.Zero;
        startingRoom.GenerateTiles();
        var door = startingRoom.AddDoor(MooreNeighbor.Up);
        AddRoomTiles(startingRoom);

        var room = new Room(ColorScheme, Tileset);
        room.Center = startingRoom.Center - new Vector2(0, room.Height + 2);
        room.GenerateTiles();
        var door2 = room.AddDoor(MooreNeighbor.Down);
        AddRoomTiles(room);

        AddHallway(door, door2);
    }

    private void AddRoomTiles(Room room) {
        for (var row = 0; row < room.Tiles.Length; row++) {
            for (var col = 0; col < room.Tiles[row].Length; col++) {
                var tile = room.Tiles[row][col];
                if (tile == null)
                    continue;
                var tileLoc = room.TopLeft + new Vector2(row, col);
                floorTileMap.SetCell((int)tileLoc.x, (int)tileLoc.y, tile.Value);
            }
        }
    }

    private void GenerateTileSet() {
        var tileset = new TileSet();
        floorTileMap.TileSet = tileset;

        var floorTile = GenerateTile<FloorTile>(tileset);
        var middleWallTile = GenerateTile<MiddleWallTile>(tileset);
        var topWallTile = GenerateTile<TopWallTile>(tileset);
        var topLeftWallTile = GenerateTile<TopLeftCornerTile>(tileset);
        var topRightWallTile = GenerateTile<TopRightCornerTile>(tileset);
        var leftWallTile = GenerateTile<LeftWallTile>(tileset);
        var rightWallTile = GenerateTile<RightWallTile>(tileset);
        var bottomWallTile = GenerateTile<BottomWallTile>(tileset);
        var bottomRightWallTile = GenerateTile<BottomRightWallTile>(tileset);
        var bottomRightCornerTile = GenerateTile<BottomRightCornerTile>(tileset);
        var bottomLeftWallTile = GenerateTile<BottomLeftWallTile>(tileset);
        var bottomLeftCornerTile = GenerateTile<BottomLeftCornerTile>(tileset);

        Tileset = new Tileset {
            Floor = floorTile.Id,
            TopWall = topWallTile.Id,
            TopLeftCorner = topLeftWallTile.Id,
            TopRightCorner = topRightWallTile.Id,
            BottomWall = bottomWallTile.Id,
            BottomLeftWall = bottomLeftWallTile.Id,
            BottomLeftCorner = bottomLeftCornerTile.Id,
            BottomRightWall = bottomRightWallTile.Id,
            BottomRightCorner = bottomRightCornerTile.Id,
            LeftWall = leftWallTile.Id,
            RightWall = rightWallTile.Id,
            MiddleWall = middleWallTile.Id,
            Door = floorTile.Id
        };
        
        ResourceSaver.Save("GeneratedTileset.tres", tileset);
    }

    private TTile GenerateTile<TTile>(TileSet tileset) where TTile : Tile, new() {
        var id = tileset.GetLastUnusedTileId();
        tileset.CreateTile(id);
        var tile = new TTile() { Id = id };
        tile.Setup(tileset);
        return tile;
    }

    private void AddHallway(Door start, Door end) {
        GD.Print($"Adding hallway from {start.Position.x},{start.Position.y} to {end.Position.x},{end.Position.y}");
        var pos = start.Position + new Vector2(0, -1);
        while (pos.x != end.Position.y &&pos.y != end.Position.y) {
            GD.Print($"Position {pos.x},{pos.y}");
            floorTileMap.SetCell((int)pos.x, (int)pos.y, Tileset.Floor);
            floorTileMap.SetCell((int)pos.x - 1, (int)pos.y, Tileset.LeftWall);
            floorTileMap.SetCell((int)pos.x + 1, (int)pos.y, Tileset.RightWall);
            pos += new Vector2(0, -1);
        }

        floorTileMap.SetCell((int)end.Position.x - 1, (int)end.Position.y, Tileset.BottomLeftWall);
        floorTileMap.SetCell((int)end.Position.x + 1, (int)end.Position.y, Tileset.BottomRightWall);
    }

    private void Reset() {
        // TODO: Reset level
    }
}
