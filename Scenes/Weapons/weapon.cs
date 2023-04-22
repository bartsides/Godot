using Godot;

public partial class weapon : Node2D
{
	private bool debug = false;
	public float Speed = 800;
	private AnimatedSprite2D animatedSprite = null;
	private AudioStreamPlayer2D audioStreamPlayer2D = null;
	private bullet projectile;
	private Node projectiles;
	private Marker2D projectileStartingPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		audioStreamPlayer2D = GetNodeOrNull<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		projectile = GetNodeOrNull<bullet>("Projectile");
		projectiles = GetNode<Node>("Projectiles");
		projectileStartingPosition = GetNodeOrNull<Marker2D>("ProjectileStartingPosition");
	}

	public projectile Shoot(Vector2 direction, uint collisionMask) {
		if (animatedSprite != null) {
			animatedSprite.Play("shoot");
		}

		if (audioStreamPlayer2D != null) {
			if (debug) GD.Print("Gun sound");
			audioStreamPlayer2D.Play();
		}

		if (projectile == null) return null;
		
		var shot = (bullet)projectile.Duplicate();
		shot.GlobalPosition = projectileStartingPosition.GlobalPosition;
		projectiles.AddChild(shot);
		var linearVelocity = direction * Speed;
		shot.Fire(GetLocalMousePosition(), linearVelocity, collisionMask);
		return projectile;
	}

	public void SetOrientation() {
		animatedSprite.FlipV = !Position.IsFacingRight();
	}
}
