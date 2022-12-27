using System.Collections.Generic;
using System.Drawing;

public class Level1 : LevelTemplate
{
    public override List<Brush> FloorBrushes => new List<Brush> {
        CreateBrush("assets/tiles/floor/stone1.png"),
        CreateBrush("assets/tiles/floor/stone2.png"),
        CreateBrush("assets/tiles/floor/stone3.png"),
    };

    public override Brush TopBrush => CreateBrush("assets/tiles/topwall/brick1.png");

    public override List<Brush> WallBrushes => new List<Brush> {
        CreateBrush("assets/tiles/wall/brick1.png"),
        CreateBrush("assets/tiles/wall/brick2.png"),
        CreateBrush("assets/tiles/wall/brick3.png"),
        CreateBrush("assets/tiles/wall/brick4.png"),
        //CreateBrush("assets/tiles/wall/brick5.png"),
    };
}