using Godot;
using System;

public partial class bullet : projectile
{
	private const bool debug = false;

	private AnimatedSprite2D animatedSprite;

	public override void _Ready()
	{
		base._Ready();
		Damage = 20;
		MaxLifetime = 2f;
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	// public override bool HandleHitEnemy(Enemy node)
	// {
	//     if (!Active) return false;

	//     if (!base.HandleHitEnemy(node))
	//         return false;

	//     if (animatedSprite != null)
	//         animatedSprite.Play("Hit");

	//     return true;
	// }

	public void Fire(Vector2 velocity, uint collisionLayer, uint collisionMask) {
		Velocity = velocity;
		LookAt(GlobalPosition + velocity);
		CollisionLayer = collisionLayer;
		CollisionMask = collisionMask;
		Visible = true;
		Active = true;

		if (debug) GD.Print($"Collision mask of bullet: {CollisionMask}");
	}
}
