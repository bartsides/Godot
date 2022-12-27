using Godot;
using System;
using System.Linq;

public class Enemy : RigidBody2D
{
	public decimal Health { get; private set; }
	private Level level;
	private Vector2[] path = new Vector2[]{};
	private int speed = 100;
	private int maxVelocity = 500;
	private float timeSinceLastReassess = 0;
	private float maxTimeSinceLastReassess = 100;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		level = GetParent().GetParent().GetParent().GetParent<Level>();
		Health = 40;
		SetProcess(true);
	}

	public override void _IntegrateForces(Physics2DDirectBodyState state)
	{
		base._IntegrateForces(state);
	}

	public override void _Process(float delta)
	{
		timeSinceLastReassess += delta;

		if (path.Length < 1 || timeSinceLastReassess > maxTimeSinceLastReassess) {
			timeSinceLastReassess = 0;
			path = level.GetSimplePath(Position, level.GetNode<Player>("Player").Position);
		}
		
		if (path != null && path.Length > 0) {
			Move(speed * delta);
		}
	}

	private void Move(float speed) {
		var dist = Position.DistanceTo(path[0]);
		if (dist < 10)
			path = path.Skip(1).Take(path.Length - 1).ToArray();
		else 
			Position = Position.LinearInterpolate(path[0], speed / dist);
	}

	public void Hit(decimal damage) {
		Health -= damage;
		if (Health <= 0)
			Die();
	}

	private void Die() {
		((Room)GetParent().GetParent()).EnemyKilled(this);
	}
}
