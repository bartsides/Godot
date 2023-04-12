using Godot;

public class Gun : Node2D
{
	private bool debug = false;
	public float Speed = 800;
	private AnimatedSprite animatedSprite = null;
	private AudioStreamPlayer2D audioStreamPlayer2D = null;
	private Bullet bullet;
	private Node2D bullets;
	private Position2D bulletPosition = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		bullet = GetNode<Bullet>("Bullet");
		bullets = GetNode<Node2D>("Bullets");
		bulletPosition = GetNode<Position2D>("BulletPosition");
	}

	public Projectile Shoot(Vector2 direction, uint collisionMask) {
		if (animatedSprite != null) {
			animatedSprite.Play("Shoot");
		}

		if (audioStreamPlayer2D != null) {
			if (debug) GD.Print("Gun sound");
			audioStreamPlayer2D.Play();
		}

		var shot = (Bullet)bullet.Duplicate();
		bullets.AddChild(shot);
		var linearVelocity = direction * Speed;
		shot.Fire(bulletPosition.Position, GetGlobalMousePosition(), linearVelocity, collisionMask);
		return bullet;
	}

	public void SetOrientation(Vector2 direction) {
		animatedSprite.FlipV = !direction.IsFacingRight();
	}
}
