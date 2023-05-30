using Godot;
using System;

public partial class weapon : Node2D
{
	[Export]
	public float Speed { get; set; } = 800;
	private bool debug = false;
	private AnimationPlayer animationPlayer = null;
	private Polygon2D polygon2D = null;
	private AudioStreamPlayer2D audioStreamPlayer2D = null;
	private bullet projectile;
	private Node projectiles;
	private Marker2D projectileStartingPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		polygon2D = GetNode<Polygon2D>("Polygon2D");
		audioStreamPlayer2D = GetNodeOrNull<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		projectile = GetNodeOrNull<bullet>("Projectile");
		projectiles = GetNode<Node>("Projectiles");
		projectileStartingPosition = GetNodeOrNull<Marker2D>("ProjectileStartingPosition");
	}

	public projectile Shoot(uint collisionLayer, uint collisionMask) {
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

		var linearVelocity = GlobalPosition.DirectionTo(GetGlobalMousePosition()) * Speed;
		shot.Fire(linearVelocity, collisionLayer, collisionMask);

		return projectile;
	}

	public void SetOrientation(bool firstTime = false) {
		var facingRight = Position.IsFacingRight();
		Scale = new Vector2(Scale.X, Math.Abs(Scale.Y) * (facingRight ? 1 : -1));
	}
}
