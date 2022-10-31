using Godot;
using System.Collections.Generic;
using System.Drawing;

public class ShootShoot : Node2D
{
	public int FloorNumber { get; set; } = 0;
	private Level level;

	public override void _Ready()
	{
		level = GetNode<Level>("Level");
		NextLevel();
	}

	private void NextLevel() {
		var floorTileMap = GetNode("Level").GetNode<TileMap>("FloorTileMap");
		floorTileMap.Clear();

		var player = GetNode("Level").GetNode<Player>("Player");
		player.Position = Vector2.Zero;

		FloorNumber++;

		level.FloorNumber = FloorNumber;
		level.GenerateLevel(new List<Brush>{Levels.WallBrushes[FloorNumber-1]});
	}

	public void LevelCompleted() {
		NextLevel();
	}
}
