using Godot;
using System.Collections.Generic;

public class TileMap : Godot.TileMap
{
	private const int ObstacleId = 0;
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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this._halfCellSize = this.CellSize / 2;
		this._mapSize = new Vector2(16, 16);
		this._aStarNode = new AStar2D();
		this._cellPath = new List<Vector2>();
		this._obstacles = new List<Vector2>();

		Godot.Collections.Array obstaclesArray = GetUsedCellsById(ObstacleId);
		for (int i = 0; i < obstaclesArray.Count; i++)
		{
			this._obstacles.Add((Vector2)obstaclesArray[i]);
		}

		List<Vector2> walkableCells = CalculateAStarWalkableCells(this._obstacles);
		ConnectAStarWalkableCells(walkableCells);
	}

	public override void _Draw()
	{
		if (this._cellPath != null && this._cellPath.Count != 0)
		{
			Vector2 startCell = this._cellPath[0];
			Vector2 endCell = this._cellPath[this._cellPath.Count - 1];

			Vector2 lastCell = MapToWorld(new Vector2(startCell.x, startCell.y)) + this._halfCellSize;

			for (int i = 1; i < this._cellPath.Count; i++)
			{
				Vector2 currentCell = MapToWorld(new Vector2(this._cellPath[i].x, this._cellPath[i].y)) + this._halfCellSize;
				DrawLine(lastCell, currentCell, _drawColor, BaseLineWidth, true);
				DrawCircle(currentCell, BaseLineWidth * 2.0f, _drawColor);

				lastCell = currentCell;
			}
		}
	}

	public List<Vector2> GetPath(Vector2 start, Vector2 end)
	{
		// TODO: if point is obstacle, find nearest accessible neighbor
		return CalculatePath(start, end);
	}

	public List<Vector2> CalculatePath(Vector2 start, Vector2 end)
	{
		ChangePathStartPosition(WorldToMap(start));
		ChangePathEndPosition(WorldToMap(end));
		RecalculatePath();

		List<Vector2> pathWorld = new List<Vector2>();
		foreach (Vector2 cell in this._cellPath)
		{
			Vector2 cellWorld = MapToWorld(new Vector2(cell.x, cell.y)) + this._halfCellSize;
			pathWorld.Add(cellWorld);
		}

		return pathWorld;
	}

	private List<Vector2> CalculateAStarWalkableCells(List<Vector2> obstacleCells)
	{
		List<Vector2> walkableCells = new List<Vector2>();
		for (int y = 0; y < this._mapSize.y; y++)
		{
			for (int x = 0; x < this._mapSize.x; x++)
			{
				Vector2 cell = new Vector2(x, y);

				if (!obstacleCells.Contains(cell))
				{
					walkableCells.Add(cell);

					int cellIndex = CalculateCellIndex(cell);
					_aStarNode.AddPoint(cellIndex, new Vector2(cell.x, cell.y));
				}
			}
		}

		return walkableCells;
	}

	private void ConnectAStarWalkableCells(List<Vector2> walkableCells)
	{
		foreach (Vector2 cell in walkableCells)
		{
			int cellIndex = CalculateCellIndex(cell);

			List<Vector2> neighborCells = new List<Vector2>();
			neighborCells.Add(new Vector2(cell.x + 1, cell.y));
			neighborCells.Add(new Vector2(cell.x - 1, cell.y));
			neighborCells.Add(new Vector2(cell.x, cell.y + 1));
			neighborCells.Add(new Vector2(cell.x, cell.y - 1));

			foreach (Vector2 neighborCell in neighborCells)
			{
				int neighborCellIndex = CalculateCellIndex(neighborCell);

				if (!IsCellOutsideMapBounds(neighborCell) && this._aStarNode.HasPoint(neighborCellIndex))
				{
					this._aStarNode.ConnectPoints(cellIndex, neighborCellIndex, false);
				}
			}
		}
	}

	private void ClearPreviousPathDrawing()
	{
		if (this._cellPath != null && this._cellPath.Count != 0)
		{
			Vector2 startCell = this._cellPath[0];
			Vector2 endCell = this._cellPath[this._cellPath.Count - 1];

			SetCell((int)startCell.x, (int)startCell.y, -1);
			SetCell((int)endCell.x, (int)endCell.y, -1);
		}
	}

	private void RecalculatePath()
	{
		ClearPreviousPathDrawing();
		int startCellIndex = CalculateCellIndex(this._pathStartPosition);
		int endCellIndex = CalculateCellIndex(this._pathEndPosition);

		this._cellPath.Clear();
		Vector2[] cellPathArray = this._aStarNode.GetPointPath(startCellIndex, endCellIndex);
		for (int i = 0; i < cellPathArray.Length; i++)
		{
			this._cellPath.Add(cellPathArray[i]);
		}

		Update();
	}

	private void ChangePathStartPosition(Vector2 newPathStartPosition)
	{
		if (!this._obstacles.Contains(newPathStartPosition) && !IsCellOutsideMapBounds(newPathStartPosition))
		{
			SetCell((int)this._pathStartPosition.x, (int)this._pathStartPosition.y, -1);
			SetCell((int)newPathStartPosition.x, (int)newPathStartPosition.y, 1);
			this._pathStartPosition = newPathStartPosition;

			if (this._pathEndPosition == null && !this._pathEndPosition.Equals(this._pathStartPosition))
			{
				RecalculatePath();
			}
		}
	}

	private void ChangePathEndPosition(Vector2 newPathEndPosition)
	{
		if (!this._obstacles.Contains(newPathEndPosition) && !IsCellOutsideMapBounds(newPathEndPosition))
		{
			SetCell((int)this._pathStartPosition.x, (int)this._pathStartPosition.y, -1);
			SetCell((int)newPathEndPosition.x, (int)newPathEndPosition.y, 2);
			this._pathEndPosition = newPathEndPosition;

			if (!this._pathStartPosition.Equals(newPathEndPosition))
			{
				RecalculatePath();
			}
		}
	}

	private int CalculateCellIndex(Vector2 cell)
	{
		return (int)(cell.x + this._mapSize.x * cell.y);
	}

	private bool IsCellOutsideMapBounds(Vector2 cell)
	{
		return cell.x < 0 || cell.y < 0 || cell.x >= this._mapSize.x || cell.y >= this._mapSize.y;
	}

	public Vector2 GetCellPosition(Vector2 cell)
	{
		return MapToWorld(cell) + _halfCellSize;
	}
}
