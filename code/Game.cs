using Godot;
using System.Collections.Generic;

public class Game : Node2D
{
    public static int Width = 200;
    public static int Height = 200;

    private readonly int[] _obstacleIds = {0};
    private const float BaseLineWidth = 3.0f;
    private readonly Color _drawColor = new Color(0, 0, 0);

    [Export]
    private Vector2 _mapSize;

    private Vector2 _halfCellSize;
    private Vector2 _pathStartPosition;
    private Vector2 _pathEndPosition;
    private AStar2D _aStarNode;
    private List<Vector2> _cellPath;
    private List<Vector2> _obstacles;

    #region Getters
    private Godot.TileMap _baseMap;
    private Godot.TileMap BaseMap
    {
        get
        {
            if (_baseMap != null) return _baseMap;
            _baseMap = GetNode<Godot.TileMap>("BaseMap");
            return _baseMap;
        }
    }

    private Godot.TileMap _itemsMap;
    private Godot.TileMap ItemsMap
    {
        get
        {
            if (_itemsMap != null) return _itemsMap;
            _itemsMap = GetNode<Godot.TileMap>("ItemsMap");
            return _itemsMap;
        }
    }
    #endregion

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this._halfCellSize = BaseMap.CellSize / 2;
        this._mapSize = new Vector2(16, 16);
        this._aStarNode = new AStar2D();
        this._cellPath = new List<Vector2>();
        this._obstacles = new List<Vector2>();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
