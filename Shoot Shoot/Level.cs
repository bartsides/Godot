using Godot;

public class Level : Navigation2D
{
    public int FloorNumber { get; set; }
    public ColorScheme ColorScheme { get; set; }
    public Tileset Tileset { get; set; }
    private TileMap floorTileMap;
    
    public override void _Ready()
    {
        floorTileMap = GetNode<TileMap>("FloorTileMap");
    }
    
    public void GenerateLevel(bool regenerateTileset) {
        Reset(regenerateTileset);
        if (regenerateTileset)
            GenerateTileSet();

        // TODO: Incorporate FloorNumber into level generation
        var startingRoom = new StartingRoom(ColorScheme, Tileset);
        AddRoomTiles(startingRoom, Vector2.Zero - new Vector2(startingRoom.Width / 2, startingRoom.Height / 2));
    }

    private void AddRoomTiles(IRoom room, Vector2 topLeft) {
        var tiles = room.GenerateTiles();
        for (var row = 0; row < tiles.Length; row++) {
            for (var col = 0; col < tiles[row].Length; col++) {
                var tile = tiles[row][col];
                if (tile == null) 
                    continue;
                var tileLoc = topLeft + new Vector2(row, col);
                floorTileMap.SetCell((int)tileLoc.x, (int)tileLoc.y, tile.Value);
            }
        }
    }

    private void GenerateTileSet() {
        var tileset = new TileSet();
        floorTileMap.TileSet = tileset;

        var id = tileset.GetLastUnusedTileId();
        tileset.CreateTile(id);
        var floorTile = new FloorTile(id);
        floorTile.Setup(tileset);

        id = tileset.GetLastUnusedTileId();
        tileset.CreateTile(id);
        var wallTile = new WallTile(id);
        wallTile.Setup(tileset);

        Tileset = new Tileset {
            FloorTile = floorTile.Id,
            TopWall = wallTile.Id,
            TopLeftWall = wallTile.Id,
            TopRightWall = wallTile.Id,
            BottomWall = wallTile.Id,
            BottomLeftWall = wallTile.Id,
            BottomRightWall = wallTile.Id,
            LeftWall = wallTile.Id,
            RightWall = wallTile.Id
        };
    }

    private void Reset(bool clearTileset) {
        // TODO: Reset level
    }
}
