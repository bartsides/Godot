using Godot;
using System;

public partial class enemy : CharacterBody2D
{
	private TileMap tileMap;
	private NavigationAgent2D navAgent;
	protected virtual int Speed { get; set; } = 100;
	protected virtual int DistanceToTarget { get; set; } = 32;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tileMap = GetParent().GetParent().GetNodeOrNull<TileMap>("TileMap");
		navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		navAgent.SetNavigationMap(tileMap.GetNavigationMap(0));

		MoveTowardsPlayer();
	}

	public override void _PhysicsProcess(double delta) {
		if (navAgent.DistanceToTarget() <= DistanceToTarget) return;

		Velocity = (navAgent.GetNextPathPosition() - GlobalPosition).Normalized() * Speed;
		MoveAndSlide();
	}

	private void MoveTowardsPlayer() {
		var player = GetParent().GetParent().GetNode<player>("Player");
		navAgent.TargetPosition = player.GlobalPosition;
		navAgent.TargetDesiredDistance = DistanceToTarget;
		GD.Print("moving towards player");
	}
}
