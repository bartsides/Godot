using System.Collections.Generic;

public interface IRoom {
    int Width { get; }
    int Height { get; }
    ColorScheme ColorScheme { get; }
    Tileset Tileset { get; }
    List<object> Items { get; set; }
    
    void AddEnemies();
    int?[][] GenerateTiles();
}