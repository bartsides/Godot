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
		FloorNumber++;

		level.FloorNumber = FloorNumber;
		level.GenerateLevel(true);
	}
}
