using Godot;
using System.Collections.Generic;

namespace NotRimworld.code
{
    public class TileMapBrain : Godot.TileMap
    {
        [Export]
        private Vector2 _mapSize;

        private Vector2 _halfCellSize;
        private Vector2 _pathStartPosition;
        private Vector2 _pathEndPosition;
        private AStar2D _aStarNode;
        private List<Vector2> _cellPath;
        private List<Vector2> _obstacles;

        public override void _Ready()
        {
            base._Ready();
            
            this._halfCellSize = this.CellSize / 2;
            this._mapSize = new Vector2(Game.Width, Game.Height);
            this._aStarNode = new AStar2D();
            this._cellPath = new List<Vector2>();
            this._obstacles = new List<Vector2>();
        }
    }
}
