using System.Collections.Generic;

public class StartingRoom : IRoom
{
    public ColorScheme ColorScheme { get; }
    public Tileset Tileset { get; }
    public List<object> Items { get; set; }

    public int Width { get; }
    public int Height { get; }

    public StartingRoom(ColorScheme colorScheme, Tileset tileset)
    {
        ColorScheme = colorScheme;
        Tileset = tileset;
        Width = 10;
        Height = 10;
    }

    public void AddEnemies()
    {
        
    }

    public int?[][] GenerateTiles() {
        var result = new int?[Width][];

        for (var x = 0; x < Width; x++) {
            result[x] = new int?[Height];
            
            for (var y = 0; y < Height; y++) {
                AddTile(result, x, y);
            }
        }

        // Add door
        result[Width/2][0] = Tileset.Door;
        result[Width/2][Height-1] = Tileset.Door;

        // Add middle wall
        result[2][2] = Tileset.TopWall;

        return result;
    }

    private void AddTile(int?[][] result, int x, int y) {
        var isWall = false;
        int tile;
        if (x == 0) {
            isWall = true;
            if (y == 0)
                tile = Tileset.TopLeftWall;
            else if (y == Height - 1)
                tile = Tileset.BottomLeftWall;
            else
                tile = Tileset.LeftWall;
        }
        else if (x == Width - 1) {
            isWall = true;
            if (y == 0)
                tile = Tileset.TopRightWall;
            else if (y == Height - 1)
                tile = Tileset.BottomRightWall;
            else
                tile = Tileset.RightWall;
        }
        else if (y == 0) {
            isWall = true;
            tile = Tileset.TopWall;
        }
        else if (y == Height - 1) {
            isWall = true;
            tile = Tileset.BottomWall;
        }
        else
            tile = Tileset.Floor;

        if (isWall) {
            result[x][y] = tile;
        }
        else {
            result[x][y] = tile;
        }
    }
}