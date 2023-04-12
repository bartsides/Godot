using Godot;

public class ShootShoot : Node2D
{
	public int FloorNumber { get; set; } = 0;
	public Level Level { get; private set; }

	public override void _Ready()
	{
		Level = GetNode<Level>("Level");
		NextLevel();
	}

	private void NextLevel() {
		var floorTileMap = GetNode("Level").GetNode<TileMap>("FloorTileMap");
		floorTileMap.Clear();

		var player = GetNode("Level").GetNode<Player>("Player");
		player.Position = Vector2.Zero;
		
		FloorNumber++;
		
		var nextLevel = GetNode("Levels").GetNode<LevelTemplate>($"Level{FloorNumber}");


		Level.FloorNumber = FloorNumber;
		Level.GenerateLevel(nextLevel);
	}

	public void LevelCompleted() {
		NextLevel();
	}
}
