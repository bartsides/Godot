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
        floorTileMap.ShowCollision = true;
    }
    
    public void GenerateLevel(bool regenerateTileset) {
        Reset();

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

        var floorTile = GenerateTile<FloorTile>(tileset);
        var middleWallTile = GenerateTile<MiddleWallTile>(tileset);
        var topWallTile = GenerateTile<TopWallTile>(tileset);
        var topLeftWallTile = GenerateTile<TopLeftWallTile>(tileset);
        var topRightWallTile = GenerateTile<TopRightWallTile>(tileset);
        var leftWallTile = GenerateTile<LeftWallTile>(tileset);
        var rightWallTile = GenerateTile<RightWallTile>(tileset);
        var bottomWallTile = GenerateTile<BottomWallTile>(tileset);
        var bottomRightWallTile = GenerateTile<BottomRightWallTile>(tileset);
        var bottomLeftWallTile = GenerateTile<BottomLeftWallTile>(tileset);

        Tileset = new Tileset {
            Floor = floorTile.Id,
            TopWall = topWallTile.Id,
            TopLeftWall = topLeftWallTile.Id,
            TopRightWall = topRightWallTile.Id,
            BottomWall = bottomWallTile.Id,
            BottomLeftWall = bottomLeftWallTile.Id,
            BottomRightWall = bottomRightWallTile.Id,
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

    private void Reset() {
        // TODO: Reset level
    }
}
