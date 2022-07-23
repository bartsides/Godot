public class StartingRoom : Room
{
    public override int Width => 10;
    public override int Height => 10;

    public StartingRoom(ColorScheme colorScheme, Tileset tileset) : base(colorScheme, tileset)
    {
    }
}