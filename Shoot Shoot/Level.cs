using Godot;
using System;
using System.Collections.Generic;

public class Level : Navigation2D
{
    public int FloorNumber { get; set; }
    public ColorScheme ColorScheme { get; set; }
    public Tileset Tileset { get; set; }
    private TileMap floorTileMap;
    private const int NumberOfRooms = 2;
    private PackedScene RoomScene;
    private Node2D RoomsNode { get; set; }
    private List<Room> Rooms { get; set; } = new List<Room>();
    
    public override void _Ready()
    {
        floorTileMap = GetNode<TileMap>("FloorTileMap");
        RoomScene = GD.Load<PackedScene>("res://Shoot Shoot/Room.tscn");
        RoomsNode = GetNode<Node2D>("Rooms");

        //floorTileMap.ShowCollision = true;
    }
    
    public void GenerateLevel(bool regenerateTileset) {
        Reset();

        if (regenerateTileset)
            GenerateTileSet();

        var room = GetNextRoom();
        MooreNeighbor direction = MooreNeighbor.Up;

        for (var i = 0; i < NumberOfRooms; i++) {
            // Determine next direction
            room = AddRoom(room, direction);
        }

        foreach (var r in Rooms) {
            AddRoomTiles(r);
        }
    }

    private Room AddRoom(Room prev, MooreNeighbor direction) {
        var room = GetNextRoom();

        Vector2 diff;
        if (direction == MooreNeighbor.Up)
            diff = new Vector2(0, -(room.Height + 2));
        else if (direction == MooreNeighbor.Down)
            diff = new Vector2(0, room.Height + 2);
        else if (direction == MooreNeighbor.Left)
            diff = new Vector2(-(room.Width + 2), 0);
        else if (direction == MooreNeighbor.Right)
            diff = new Vector2(room.Width + 2, 0);
        else
            throw new NotImplementedException($"Add room with direction {direction} not implemented");

        var door = prev.AddDoor(direction);
        
        room.Center = prev.Center + diff;
        var nextDoor = room.AddDoor(direction.Opposite());

        AddHallway(door, nextDoor);

        room.AddEnemies();
        return room;
    }

    private Room GetNextRoom() {
        var room = RoomScene.Instance<Room>();
        room.ColorScheme = ColorScheme;
        room.Tileset = Tileset;
        room.GenerateTiles();

        RoomsNode.AddChild(room);
        Rooms.Add(room);
        return room;
    }

    private void AddRoomTiles(Room room) {
        GD.Print("Adding room tiles");
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
        // Only handles up
        GD.Print($"Adding hallway from {start.Position.x},{start.Position.y} to {end.Position.x},{end.Position.y}");
        var pos = start.Position + new Vector2(0, -1);
        while (pos.x != end.Position.y && pos.y != end.Position.y) {
            //GD.Print($"Position {pos.x},{pos.y}");
            floorTileMap.SetCell((int)pos.x, (int)pos.y, Tileset.Floor);
            floorTileMap.SetCell((int)pos.x - 1, (int)pos.y, Tileset.LeftWall);
            floorTileMap.SetCell((int)pos.x + 1, (int)pos.y, Tileset.RightWall);
            pos += new Vector2(0, -1);
        }
    }

    private void Reset() {
        // TODO: Reset level
    }
}
