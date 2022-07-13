using System.Collections.Generic;

public class Room : IRoom {
    public ColorScheme ColorScheme { get; }
    public Tileset Tileset { get; }
    public List<object> Items { get; set; }

    public int Width { get; }
    public int Height { get; }

    public Room(ColorScheme colorScheme, Tileset tileset)
    {
        ColorScheme = colorScheme;
        Tileset = tileset;
        
        Width = 20;
        Height = 20;
    }

    public void AddEnemies()
    {
        
    }

    public int?[][] GenerateTiles() {
        return RoomGenerator.Generate(Width, Height, Tileset);
    }
}