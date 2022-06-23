using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace NotRimworld.code
{
    public class Game : Node2D
    {
        public static int Width = 200;
        public static int Height = 200;

        private readonly int[] _obstacleIds = {3,5,6,7,8,9,10,11,12,13,14,15,16};
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

        private List<Player> _selectingPlayers = new List<Player>();
        private List<Player> _selectedPlayers = new List<Player>();

        #region Getters
        private Godot.TileMap _baseMap;
        private Godot.TileMap BaseMap
        {
            get
            {
                if (_baseMap != null) return _baseMap;
                _baseMap = GetNode<Godot.TileMap>("Nav/TileMap");
                return _baseMap;
            }
        }
        #endregion
    
        public override void _Ready()
        {
            _halfCellSize = BaseMap.CellSize / 2;
            _mapSize = new Vector2(16, 16);
            _aStarNode = new AStar2D();
            _cellPath = new List<Vector2>();
            _obstacles = new List<Vector2>();

            // TODO: Generate map

            SetObstacles();
            AddPlayers();
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("right_click"))
            {
                GD.Print("right click");
                if (_selectedPlayers.Count > 0)
                {
                    GD.Print(_selectedPlayers.Count);
                    foreach (var selectedPlayer in _selectedPlayers)
                    {
                        // TODO: Formations
                        selectedPlayer.MoveTo(GetGlobalMousePosition());
                    }
                }
            }
        }

        private void AddPlayers()
        {
            var players = GetNode("Nav/Players");

            var playerScene = GD.Load<PackedScene>("res://Player.tscn");

            for (var i = 0; i < 4; i++)
            {
                var player = playerScene.Instance() as Player;
                player.Position = new Vector2(0, i * 64);
                players.AddChild(player);
            }
        }

        public void SelectPlayer(Player player, bool selected)
        {
            player.Highlight(selected);
            if (selected) 
                _selectingPlayers.Add(player);
            else 
                _selectingPlayers.Remove(player);
        }

        public void SelectionEnded(Player[] players)
        {
            ResetSelection();

            _selectedPlayers.ForEach(p => p.Highlight(false));
            _selectedPlayers = new List<Player>();

            foreach (var player in players)
            {
                player.Highlight(true);
                _selectedPlayers.Add(player);
            }
        }

        public void ResetSelection()
        {
            foreach (var player in _selectingPlayers)
            {
                player.Highlight(false);
            }

            _selectingPlayers = new List<Player>();
        }

        private void SetObstacles()
        {
            _obstacles = new List<Vector2>();

            foreach (var obstacleId in _obstacleIds)
            {
                foreach (Vector2 cell in BaseMap.GetUsedCellsById(obstacleId))
                    _obstacles.Add(cell);
            }

            ConnectAStarWalkableCells(CalculateAStarWalkableCells(_obstacles));
        }

        public List<Vector2> GetPath(Vector2 start, Vector2 end)
        {
            // TODO: if point is obstacle, find nearest accessible neighbor
            return CalculatePath(start, end);
        }

        public List<Vector2> CalculatePath(Vector2 start, Vector2 end)
        {
            RecalculatePath();

            var pathWorld = new List<Vector2>();
            foreach (var cell in _cellPath)
            {
                var cellWorld = BaseMap.MapToWorld(new Vector2(cell.x, cell.y)) + _halfCellSize;
                pathWorld.Add(cellWorld);
            }

            return pathWorld;
        }

        private List<Vector2> CalculateAStarWalkableCells(List<Vector2> obstacleCells)
        {
            var walkableCells = new List<Vector2>();
            for (var y = 0; y < _mapSize.y; y++)
            {
                for (var x = 0; x < _mapSize.x; x++)
                {
                    var cell = new Vector2(x, y);

                    if (!obstacleCells.Contains(cell))
                    {
                        walkableCells.Add(cell);

                        var cellIndex = CalculateCellIndex(cell);
                        _aStarNode.AddPoint(cellIndex, new Vector2(cell.x, cell.y));
                    }
                }
            }

            return walkableCells;
        }

        private void ConnectAStarWalkableCells(List<Vector2> walkableCells)
        {
            foreach (var cell in walkableCells)
            {
                var cellIndex = CalculateCellIndex(cell);

                var neighborCells = new[]
                {
                    new Vector2(cell.x + 1, cell.y),
                    new Vector2(cell.x - 1, cell.y),
                    new Vector2(cell.x, cell.y + 1),
                    new Vector2(cell.x, cell.y - 1)
                };

                foreach (var neighborCell in neighborCells)
                {
                    var neighborCellIndex = CalculateCellIndex(neighborCell);

                    if (!IsCellOutsideMapBounds(neighborCell) && _aStarNode.HasPoint(neighborCellIndex))
                    {
                        _aStarNode.ConnectPoints(cellIndex, neighborCellIndex, false);
                    }
                }
            }
        }

        private void ClearPreviousPathDrawing()
        {
            if (_cellPath != null && _cellPath.Count != 0)
            {
                // TODO: Path drawing
            }
        }

        private void RecalculatePath()
        {
            ClearPreviousPathDrawing();
            var startCellIndex = CalculateCellIndex(_pathStartPosition);
            var endCellIndex = CalculateCellIndex(_pathEndPosition);

            _cellPath.Clear();
            _cellPath.AddRange(_aStarNode.GetPointPath(startCellIndex, endCellIndex));

            Update();
        }

        private int CalculateCellIndex(Vector2 cell)
        {
            return (int)(cell.x + _mapSize.x * cell.y);
        }

        private bool IsCellOutsideMapBounds(Vector2 cell)
        {
            return cell.x < 0 || cell.y < 0 || cell.x >= _mapSize.x || cell.y >= _mapSize.y;
        }

        public Vector2 GetCellPosition(Vector2 cell)
        {
            return BaseMap.MapToWorld(cell) + _halfCellSize;
        }

        public Array GetUsedCellsById(int id) => BaseMap.GetUsedCellsById(id);
        public Vector2 WorldToMap(Vector2 position) => BaseMap.WorldToMap(position);
    }
}
