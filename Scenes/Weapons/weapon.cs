using Godot;

public partial class weapon : Node2D
{
	private bool debug = false;
	public float Speed = 800;
	private AnimatedSprite2D animatedSprite = null;
	private AnimationPlayer animationPlayer = null;
	private Polygon2D polygon2D = null;
	private AudioStreamPlayer2D audioStreamPlayer2D = null;
	private bullet projectile;
	private Node projectiles;
	private Marker2D projectileStartingPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		polygon2D = GetNode<Polygon2D>("Polygon2D");
		audioStreamPlayer2D = GetNodeOrNull<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		projectile = GetNodeOrNull<bullet>("Projectile");
		projectiles = GetNode<Node>("Projectiles");
		projectileStartingPosition = GetNodeOrNull<Marker2D>("ProjectileStartingPosition");
	}

	public projectile Shoot(Vector2 direction, uint collisionLayer, uint collisionMask) {
		// if (animatedSprite != null) {
		// 	animatedSprite.Play("shoot");
		// }

		if (animationPlayer != null) {
			animationPlayer.Play("Shoot");
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
		shot.Fire(linearVelocity, collisionLayer, collisionMask);

		return projectile;
	}

	public void SetOrientation() {
		var facingRight = !Position.IsFacingRight();
		animatedSprite.FlipV = facingRight;
		//polygon2D.ApplyScale(new Vector2(1, facingRight ? 1 : -1)); // TODO fix this
	}
}
