using Godot;

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
		
		var nextLevel = Levels.LevelTemplates[FloorNumber];

		FloorNumber++;

		level.FloorNumber = FloorNumber;
		level.GenerateLevel(nextLevel);
	}

	public void LevelCompleted() {
		NextLevel();
	}
}
